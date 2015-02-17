// // -----------------------------------------------------------------------
// <copyright file="BackgroundMultiLogWriter.cs" company="Adap.tv">
// Copyright (c) 2015 Adap.tv.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LogJam.Writer
{
	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Threading;

	using LogJam.Trace;
	using LogJam.Util;


	/// <summary>
	/// Provides synchronized <see cref="ILogWriter{TEntry}"/>s that write to corresponding <see cref="ILogWriter{TEntry}"/> and
	/// <see cref="IMultiLogWriter"/> instances on a single background thread.  This implementation minimizes the performance
	/// impact of writing to logs by allowing clients to "send and forget", until a queue is full.  In normal cases all logged
	/// entries are guaranteed to be written to the background log writers, however abnormal termination of an application can result
	/// in queued entries not being written.
	/// </summary>
	internal sealed class BackgroundMultiLogWriter : IMultiLogWriter, IStartable, IDisposable
	{
		/// <summary>
		/// The default max length for the blocking queue for log writers.
		/// </summary>
		public const int DefaultMaxQueueLength = 512;

		private readonly ITracerFactory _setupTracerFactory;
		private readonly Tracer _tracer;

		// The log writers that are wrapped, and are invoked in the background thread.
		private readonly ISet<ILogWriter> _innerLogWriters;
		// The proxy writers wrapping the _innerLogWriters
		private readonly List<ILogWriter> _proxyWriters;
		// Queued actions to invoke on the background thread.
		private readonly ConcurrentQueue<Action> _backgroundActionQueue;

		private bool _isDisposed;

		private BackgroundThread _backgroundThread;
 

		internal BackgroundMultiLogWriter(ITracerFactory setupTracerFactory)
		{
			Contract.Requires<ArgumentNullException>(setupTracerFactory != null);

			_setupTracerFactory = setupTracerFactory;
			_tracer = setupTracerFactory.TracerFor(this);

			_innerLogWriters = new HashSet<ILogWriter>();
			_proxyWriters = new List<ILogWriter>();
			_backgroundActionQueue = new ConcurrentQueue<Action>();

			_isDisposed = false;
			_backgroundThread = null;
		}

		/// <summary>
		/// Finalizer, used to ensure that queued logs get flushed during finalization.
		/// </summary>
		~BackgroundMultiLogWriter()
		{
			Dispose(false);
		}

		/// <summary>
		/// Creates and returns a proxy <see cref="ILogWriter{TEntry}"/> that is synchronized,
		/// and that implements blocking queue functionality for background logging.
		/// </summary>
		/// <typeparam name="TEntry">The log entry type.</typeparam>
		/// <param name="innerLogWriter">A <see cref="ILogWriter{TEntry}"/> that is written to in a background thread.</param>
		/// <param name="maxQueueLength">The max length for the queue.  If more than this number of log entries is queued, the writer will block.</param>
		/// <returns></returns>
		public IQueueLogWriter<TEntry> CreateProxyWriterFor<TEntry>(ILogWriter<TEntry> innerLogWriter, int maxQueueLength = DefaultMaxQueueLength)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(innerLogWriter != null);
			Contract.Requires<ArgumentException>(maxQueueLength > 0);

			lock (this)
			{
				EnsureNotDisposed();
				OperationNotSupportedWhenStarted("CreateProxyWriterFor(ILogWriter<TEntry>)");

				var proxyLogWriter = CreateBlockingQueueLogWriter(innerLogWriter, maxQueueLength);
				_innerLogWriters.Add(innerLogWriter);
				_proxyWriters.Add(proxyLogWriter);
				return proxyLogWriter;
			}
		}

		private BlockingQueueLogWriter<TEntry> CreateBlockingQueueLogWriter<TEntry>(ILogWriter<TEntry> innerLogWriter, int maxQueueLength)
			where TEntry : ILogEntry
		{
			return new BlockingQueueLogWriter<TEntry>(innerLogWriter, this, maxQueueLength);
		}

		public IMultiLogWriter CreateProxyWriterFor(IMultiLogWriter innerMultiWriter, int maxQueueLength = DefaultMaxQueueLength)
		{
			Contract.Requires<ArgumentNullException>(innerMultiWriter != null);
			Contract.Requires<ArgumentException>(maxQueueLength > 0);

			lock (this)
			{
				EnsureNotDisposed();
				OperationNotSupportedWhenStarted("CreateProxyWriterFor(IMultiLogWriter)");

				// Use dynamic for run-time generic type resolution
				var multiLogWriter = new MultiLogWriter(true, _setupTracerFactory);
				foreach (dynamic logWriter in innerMultiWriter)
				{
					multiLogWriter.AddLogWriter(CreateBlockingQueueLogWriter(logWriter, maxQueueLength));
				}

				_innerLogWriters.Add(innerMultiWriter);
				_proxyWriters.Add(multiLogWriter);
				return multiLogWriter;
			}
		}

		#region IMultiLogWriter

		public bool GetLogWriter<TEntry>(out ILogWriter<TEntry> logWriter) where TEntry : ILogEntry
		{
			lock (this)
			{
				// Return all logwriters of the specified type
				ILogWriter<TEntry>[] logWriters = _proxyWriters.OfType<ILogWriter<TEntry>>()
					// In addition, get all logwriters of the specified type, that are obtained from IMultiLogWriters
					.Concat(_proxyWriters.OfType<IMultiLogWriter>().SelectMany(m => m).OfType<ILogWriter<TEntry>>()).ToArray();
				if (logWriters.Length == 1)
				{
					logWriter = logWriters[0];
					return true;
				}
				else if (logWriters.Length == 0)
				{
					logWriter = null;
					return false;
				}
				else
				{
					logWriter = new FanOutLogWriter<TEntry>(logWriters);
					return true;
				}
			}
		}

		public bool Enabled { get { return _isStarted; } }

		public bool IsSynchronized { get { return true; } }

		public IEnumerator<ILogWriter> GetEnumerator()
		{
			return _proxyWriters.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
		#region IStartable

		public void Start()
		{
			lock (this)
			{
				if (_isStarted)
				{
					return;
				}
				EnsureNotDisposed();
				Debug.Assert(! IsBackgroundThreadRunning);

				_isStarted = true;
				_backgroundThread = new Thread(BackgroundThreadProc);
				_backgroundThread.Name = "BackgroundMultiLogWriter.BackgroundThread";
			}

			_proxyWriters.SafeStart(_setupTracerFactory);
			_backgroundThread.Start();
		}

		public void Stop()
		{
			lock (this)
			{
				if (! _isStarted)
				{
					return;
				}

				var backgroundThread = _backgroundThread;
				if (backgroundThread != null)
				{
					_proxyWriters.SafeStop(_setupTracerFactory);
					_isStarted = false;
					backgroundThread.Join();
				}

				_isStarted = false;
			}
		}

		public void Dispose()
		{
			lock (this)
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		private void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			var backgroundThread = _backgroundThread;
			if (backgroundThread != null)
			{
				_proxyWriters.SafeStop(_setupTracerFactory);
				_proxyWriters.SafeDispose(_setupTracerFactory);
				QueueBackgroundAction(() => _proxyWriters.Clear());

				_isDisposed = true;
				_isStarted = false;
				backgroundThread.Join();
			}

			_isDisposed = true;
			_isStarted = false;			
		}

		public bool IsStarted
		{
			get { return _isStarted; }
		}

		#endregion

		internal bool IsBackgroundThreadRunning
		{
			get
			{
				var backgroundThread = _backgroundThread;
				if (backgroundThread == null)
				{
					return false;
				}
				else if (! backgroundThread.IsAlive)
				{
					_backgroundThread = null;
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		private void EnsureNotDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().GetCSharpName());
			}
		}

		private void OperationNotSupportedWhenStarted(string method)
		{
			if (_isStarted)
			{
				throw new LogWriterException(string.Format("{0} not supported when instance is started.", method), this);
			}
		}

		private void QueueBackgroundAction(Action backgroundAction)
		{
			_backgroundActionQueue.Enqueue(backgroundAction);
		}


		/// <summary>
		/// The set of operations that are executed on the background thread.  All these methods must be valid <see cref="Action"/>s.
		/// </summary>
		private interface IBackgroundThreadLogWriterActions
		{
			void StartInnerWriter();

			void DequeAndWriteEntry();

			void StopInnerWriter();

			void DisposeInnerWriter();
		}

		/// <summary>
		/// A proxy <see cref="ILogWriter{TEntry}"/> implementation that queues elements so they can be written on a background thread.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		private class BlockingQueueLogWriter<TEntry> : IQueueLogWriter<TEntry>, IBackgroundThreadLogWriterActions, IDisposable
			where TEntry : ILogEntry
		{

			private readonly BackgroundMultiLogWriter _parent;
			private readonly ILogWriter<TEntry> _innerLogWriter;
			private readonly ConcurrentQueue<TEntry> _queue;
			private readonly SemaphoreSlim _entriesInQueue;
			private bool _isStarted;
			private bool _isDisposed;

			internal BlockingQueueLogWriter(ILogWriter<TEntry> innerLogWriter, BackgroundMultiLogWriter parent, int maxQueueLength)
			{
				Contract.Requires<ArgumentNullException>(innerLogWriter != null);
				Contract.Requires<ArgumentNullException>(parent != null);
				Contract.Requires<ArgumentException>(maxQueueLength > 0);

				_innerLogWriter = innerLogWriter;
				_parent = parent;
				_queue = new ConcurrentQueue<TEntry>();
				_entriesInQueue = new SemaphoreSlim(maxQueueLength);
				_isStarted = _innerLogWriter.Enabled;
				_isDisposed = false;
			}

			public void Write(ref TEntry entry)
			{
				if (! _isStarted)
				{
					return;
				}

				// Blocks if maxQueueLength is exceeded
				_entriesInQueue.Wait();

				// TODO: Evaluate writing my own lockless ConcurrentQueue implementation, which could be slightly more performant for 
				// reads and writes by using ref parameters for value types, and a contiguous block of entries.
				_queue.Enqueue(entry);

				_parent.QueueBackgroundAction(DequeAndWriteEntry);
			}

			public bool Enabled { get { return _isStarted; } }

			public bool IsSynchronized { get { return true; } }

			public bool IsEmpty
			{
				get { return _queue.IsEmpty; }
			}

			public bool TryDequeue(out TEntry logEntry)
			{
				bool success = _queue.TryDequeue(out logEntry);
				if (success)
				{
					_entriesInQueue.Release();
				}
				return success;
			}

			public void Start()
			{
				lock (this)
				{
					if (_isStarted)
					{
						return;
					}
					if (_isDisposed)
					{
						throw new ObjectDisposedException(GetType().GetCSharpName());
					}
					_isStarted = true;
				}

				_parent.QueueBackgroundAction(StartInnerWriter);
			}

			public void Stop()
			{
				lock (this)
				{
					if (! _isStarted)
					{
						return;
					}
					_isStarted = false;
				}

				// Blocks if maxQueueLength is exceeded
				_entriesInQueue.Wait();

				_parent.QueueBackgroundAction(StopInnerWriter);
			}

			public bool IsStarted { get { return _isStarted; } }

			public void Dispose()
			{
				lock (this)
				{
					if (_isDisposed)
					{
						return;
					}
					_isDisposed = true;
				}

				Stop();

				if (!(_innerLogWriter is IDisposable))
				{
					return;
				}

				// Blocks if maxQueueLength is exceeded
				_entriesInQueue.Wait();

				_parent.QueueBackgroundAction(DisposeInnerWriter);
			}

			#region IBackgroundThreadLogWriterActions

			public void StartInnerWriter()
			{
				var startableInnerLogWriter = _innerLogWriter as IStartable;
				if ((startableInnerLogWriter != null) && (!startableInnerLogWriter.IsStarted))
				{
					// Start is delegated on the foreground thread
					startableInnerLogWriter.SafeStart(_parent._setupTracerFactory);
				}
			}

			public void DequeAndWriteEntry()
			{
				TEntry logEntry;
				bool success = TryDequeue(out logEntry);
				if (success)
				{
					_innerLogWriter.Write(ref logEntry);
				}
			}

			public void StopInnerWriter()
			{
				(_innerLogWriter as IStartable).SafeStop(_parent._setupTracerFactory);
				_entriesInQueue.Release();
			}

			public void DisposeInnerWriter()
			{
				(_innerLogWriter as IDisposable).SafeDispose(_parent._setupTracerFactory);
				_entriesInQueue.Release();
			}

			#endregion

		}


		/// <summary>
		/// Encapsulates the background thread for a <see cref="BackgroundMultiLogWriter"/> instance.
		/// </summary>
		private class BackgroundThread
		{

			private readonly Tracer _tracer;
			// Queued actions to invoke on the background thread.
			private readonly ConcurrentQueue<Action> _backgroundActionQueue;
			private volatile bool _isStarted;
			private Thread _thread;

			public BackgroundThread(ITracerFactory setupTracerFactory, ConcurrentQueue<Action> backgroundActionQueue)
			{
				Contract.Requires<ArgumentNullException>(setupTracerFactory != null);
				Contract.Requires<ArgumentNullException>(backgroundActionQueue != null);

				_tracer = setupTracerFactory.TracerFor(this);
				_backgroundActionQueue = backgroundActionQueue;
				_isStarted = false;
			}

			public void Start()
			{
				_thread = new Thread(BackgroundThreadProc);
				_thread.Name = "BackgroundMultiLogWriter.BackgroundThread";
			}

			public void Stop()
			{
				
			}

			/// <summary>
			/// ThreadProc for the background thread.  At any one time, there should be 0 or 1 background threads for each <see cref="BackgroundMultiLogWriter"/>
			/// instance.
			/// </summary>
			private void BackgroundThreadProc()
			{
				_tracer.Info("Started background thread.");

				SpinWait spinWait = new SpinWait();
				while (true)
				{
					Action action;
					if (_backgroundActionQueue.TryDequeue(out action))
					{
						try
						{
							action();
						}
						catch (Exception excp)
						{
							_tracer.Error(excp, "Exception caught in background thread.");
						}

						spinWait.Reset();
					}
					else if (spinWait.NextSpinWillYield && ! _isStarted)
					{
						// No queued actions, and logwriter is stopped: Time to exit the background thread
						break;
					}
					else
					{
						spinWait.SpinOnce();
					}
				}

				_tracer.Info("Exiting background thread.");
				_backgroundThread = null;
			}

		}

	}

}