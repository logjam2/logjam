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
	using System.Threading;

	using LogJam.Trace;
	using LogJam.Util;


	/// <summary>
	/// Provides multiple synchronized <see cref="ILogWriter{TEntry}"/>s and <see cref="IMultiLogWriter"/>s that write to 
	/// corresponding <see cref="ILogWriter{TEntry}"/> and <see cref="IMultiLogWriter"/> instances on a single background thread.
	/// This implementation minimizes the performance impact of writing to logs by allowing clients to "send and forget", 
	/// until the queue is full.  In normal cases all logged entries are guaranteed to be written to the background log writers,
	/// however abnormal termination of an application can result in queued entries not being written.
	/// </summary>
	internal sealed class BackgroundMultiLogWriter : IMultiLogWriter, IStartable, IDisposable
	{
		/// <summary>
		/// The default max length for the blocking queue for log writers.
		/// </summary>
		public const int DefaultMaxQueueLength = 512;

		private readonly ITracerFactory _setupTracerFactory;
		private readonly Tracer _tracer;

		// The proxy writers wrapping the inner LogWriters
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

			_proxyWriters = new List<ILogWriter>();
			_backgroundActionQueue = new ConcurrentQueue<Action>();

			_isDisposed = false;
			_backgroundThread = null;
		}

		internal BackgroundMultiLogWriter(ITracerFactory setupTracerFactory, params ILogWriter[] logWriters)
			: this(setupTracerFactory)
		{
			Contract.Requires<ArgumentNullException>(setupTracerFactory != null);
			Contract.Requires<ArgumentNullException>(logWriters != null);

			foreach (ILogWriter logWriter in logWriters)
			{
				if (logWriter is IMultiLogWriter)
				{
					CreateProxyWriterFor((IMultiLogWriter) logWriter);
				}
				else
				{
					// Use reflection to handle generic ILogWriter<> forms
					Type[] typeArgs = logWriter.GetType().GetGenericTypeArgumentsFor(typeof(ILogWriter<>));
					if ((typeArgs == null) || (typeArgs.Length != 1))
					{
						throw new LogJamSetupException("LogWriter type not supported in constructor: " + logWriter.GetType().GetCSharpName(), this);
					}
					this.InvokeGenericMethod(typeArgs, "CreateProxyWriterFor", logWriter, DefaultMaxQueueLength);
				}
			}
		}

		/// <summary>
		/// Finalizer, used to ensure that queued logs get flushed during finalization.
		/// </summary>
		~BackgroundMultiLogWriter()
		{
			_tracer.Error("In finalizer (~BackgroundMultiLogWriter) - forgot to Dispose()?");
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

				var multiLogWriter = new MultiLogWriterProxy(innerMultiWriter, _backgroundActionQueue, _setupTracerFactory);
				foreach (ILogWriter logWriter in innerMultiWriter)
				{
					Type[] typeArgs = logWriter.GetType().GetGenericTypeArgumentsFor(typeof(ILogWriter<>));
					if ((typeArgs == null) || (typeArgs.Length != 1))
					{
						throw new LogJamSetupException("LogWriter type not supported in IMultiLogWriter: " + logWriter.GetType().GetCSharpName(), this);
					}
					object blockingQueueLogWriter = this.InvokeGenericMethod(typeArgs, "CreateBlockingQueueLogWriter", logWriter, maxQueueLength);
					multiLogWriter.InvokeGenericMethod(typeArgs, "AddLogWriter", blockingQueueLogWriter);
				}

				_proxyWriters.Add(multiLogWriter);
				return multiLogWriter;
			}
		}

		#region IMultiLogWriter

		public bool GetLogWriter<TEntry>(out ILogWriter<TEntry> logWriter) where TEntry : ILogEntry
		{
			lock (this)
			{
				var logWriters = new List<ILogWriter<TEntry>>();
				_proxyWriters.FindLogWritersByType(logWriters);
				if (logWriters.Count == 1)
				{
					logWriter = logWriters[0];
					return true;
				}
				else if (logWriters.Count == 0)
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

		/// <summary>
		/// Returns <c>true</c> if this object is ready to receive log writes.
		/// </summary>
		/// <remarks>Enabled is synonymous with <see cref="IsStarted"/> for this class.</remarks>
		public bool Enabled { get { return (_backgroundThread != null) && _backgroundThread.IsStarted; } }

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
				EnsureNotDisposed();
				if (_backgroundThread == null)
				{
					_backgroundThread = new BackgroundThread(_setupTracerFactory, _backgroundActionQueue);
				}
			}

			_proxyWriters.SafeStart(_setupTracerFactory);
			_backgroundThread.Start();
		}

		public void Stop()
		{
			lock (this)
			{
				var backgroundThread = _backgroundThread;
				if (backgroundThread != null)
				{
					_proxyWriters.SafeStop(_setupTracerFactory);
					backgroundThread.Stop();
				}
			}
		}

		public bool IsStarted
		{
			get { return Enabled; }
		}

		#endregion

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
				_backgroundActionQueue.Enqueue(() => _proxyWriters.Clear());

				_isDisposed = true;
				backgroundThread.Stop();
				_backgroundThread = null;
			}

			_isDisposed = true;
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
			if (IsStarted)
			{
				throw new LogWriterException(string.Format("{0} not supported when instance is started.", method), this);
			}
		}

		/// <summary>
		/// Used only for test verification.
		/// </summary>
		internal bool IsBackgroundThreadRunning
		{
			get { return _backgroundThread != null && _backgroundThread.IsThreadRunning; }
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
		/// A proxy <see cref="IMultiLogWriter"/> that can be accessed in the foreground thread, but that queues Start() and 
		/// Stop() operations to the background thread.
		/// </summary>
		private class MultiLogWriterProxy : MultiLogWriter
		{

			// The IMultiLogWriter that is accessed only on the background thread
			private readonly IMultiLogWriter _innerMultiLogWriter;
			// References parent._backgroundActionQueue
			private readonly ConcurrentQueue<Action> _backgroundActionQueue;

			internal MultiLogWriterProxy(IMultiLogWriter innerMultiLogWriter, ConcurrentQueue<Action> backgroundActionQueue, ITracerFactory setupTracerFactory)
				: base(false, setupTracerFactory)
			{
				Contract.Requires<ArgumentNullException>(innerMultiLogWriter != null);
				Contract.Requires<ArgumentNullException>(backgroundActionQueue != null);
				Contract.Requires<ArgumentNullException>(setupTracerFactory != null);

				_innerMultiLogWriter = innerMultiLogWriter;
				_backgroundActionQueue = backgroundActionQueue;
			}

			public override bool IsSynchronized { get { return true; } }

			private void QueueBackgroundAction(Action backgroundAction)
			{
				_backgroundActionQueue.Enqueue(backgroundAction);
			}

			protected override void InternalStart()
			{
				QueueBackgroundAction(() => _innerMultiLogWriter.SafeStart(SetupTracerFactory));
				
				base.InternalStart();
			}

			protected override void InternalStop()
			{
				base.InternalStop();

				QueueBackgroundAction(() => _innerMultiLogWriter.SafeStop(SetupTracerFactory));
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);

				QueueBackgroundAction(() => _innerMultiLogWriter.SafeDispose(SetupTracerFactory));
			}

		}

		/// <summary>
		/// A proxy <see cref="ILogWriter{TEntry}"/> implementation that queues elements so they can be written on a background thread.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		private class BlockingQueueLogWriter<TEntry> : IQueueLogWriter<TEntry>, IBackgroundThreadLogWriterActions, IDisposable
			where TEntry : ILogEntry
		{

			private readonly ILogWriter<TEntry> _innerLogWriter;
			private readonly ConcurrentQueue<TEntry> _queue;
			private readonly SemaphoreSlim _slotsLeftInQueue;
	
			// References parent._backgroundActionQueue
			private readonly ConcurrentQueue<Action> _backgroundActionQueue;
			// References parent._setupTracerFactory
			private readonly ITracerFactory _setupTracerFactory;

			private bool _isStarted;
			private bool _isDisposed;

			internal BlockingQueueLogWriter(ILogWriter<TEntry> innerLogWriter, BackgroundMultiLogWriter parent, int maxQueueLength)
			{
				Contract.Requires<ArgumentNullException>(innerLogWriter != null);
				Contract.Requires<ArgumentNullException>(parent != null);
				Contract.Requires<ArgumentException>(maxQueueLength > 0);

				_innerLogWriter = innerLogWriter;
				_queue = new ConcurrentQueue<TEntry>();
				_slotsLeftInQueue = new SemaphoreSlim(maxQueueLength);

				_backgroundActionQueue = parent._backgroundActionQueue;
				_setupTracerFactory = parent._setupTracerFactory;

				_isStarted = _innerLogWriter.Enabled;
				_isDisposed = false;
			}

			private void QueueBackgroundAction(Action backgroundAction)
			{
				_backgroundActionQueue.Enqueue(backgroundAction);
			}

			public void Write(ref TEntry entry)
			{
				if (! _isStarted)
				{
					return;
				}

				// Blocks if maxQueueLength is exceeded
				_slotsLeftInQueue.Wait();

				// TODO: Evaluate writing my own lockless ConcurrentQueue implementation, which could be slightly more performant for 
				// reads and writes by using ref parameters for value types, and a contiguous block of entries.
				_queue.Enqueue(entry);

				QueueBackgroundAction(DequeAndWriteEntry);
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
					_slotsLeftInQueue.Release();
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

				QueueBackgroundAction(StartInnerWriter);
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
				_slotsLeftInQueue.Wait();

				QueueBackgroundAction(StopInnerWriter);
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
				_slotsLeftInQueue.Wait();

				QueueBackgroundAction(DisposeInnerWriter);
			}

			#region IBackgroundThreadLogWriterActions

			public void StartInnerWriter()
			{
				var startableInnerLogWriter = _innerLogWriter as IStartable;
				if ((startableInnerLogWriter != null) && (!startableInnerLogWriter.IsStarted))
				{
					// Start is delegated on the foreground thread
					startableInnerLogWriter.SafeStart(_setupTracerFactory);
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
				(_innerLogWriter as IStartable).SafeStop(_setupTracerFactory);
				_slotsLeftInQueue.Release();
			}

			public void DisposeInnerWriter()
			{
				(_innerLogWriter as IDisposable).SafeDispose(_setupTracerFactory);
				_slotsLeftInQueue.Release();
			}

			#endregion

		}


		/// <summary>
		/// Encapsulates the background thread for a <see cref="BackgroundMultiLogWriter"/> instance.
		/// </summary>
		private class BackgroundThread : IStartable
		{

			private readonly Tracer _tracer;
			// Queued actions to invoke on the background thread.
			private readonly ConcurrentQueue<Action> _backgroundActionQueue;
			private volatile bool _isStarted;
			private Thread _thread;

			// REVIEW: Important that this object + ThreadProc has NO reference to the parent BackgroundMultiLogWriter.
			// If there were a reference from this, it would never finalize.

			public BackgroundThread(ITracerFactory setupTracerFactory, ConcurrentQueue<Action> backgroundActionQueue)
			{
				Contract.Requires<ArgumentNullException>(setupTracerFactory != null);
				Contract.Requires<ArgumentNullException>(backgroundActionQueue != null);

				_tracer = setupTracerFactory.TracerFor(this);
				_backgroundActionQueue = backgroundActionQueue;
				_isStarted = false;
			}

			#region IStartable

			public void Start()
			{
				lock (this)
				{
					if (_isStarted)
					{
						return;
					}

					Debug.Assert(!IsThreadRunning);

					_isStarted = true;
					_thread = new Thread(BackgroundThreadProc)
					          {
						          Name = "BackgroundMultiLogWriter.BackgroundThread"
					          };
					_thread.Start();
				}
			}

			public void Stop()
			{
				lock (this)
				{
					var thread = _thread;
					if (thread != null)
					{
						_isStarted = false;
						thread.Join();
					}

					_isStarted = false;
				}
			}

			public bool IsStarted
			{
				get { return _isStarted; }
			}

			#endregion

			internal bool IsThreadRunning
			{
				get
				{
					var thread = _thread;
					if (thread == null)
					{
						return false;
					}
					else if (! thread.IsAlive)
					{
						_thread = null;
						return false;
					}
					else
					{
						return true;
					}
				}
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
				_thread = null;
			}

		}

	}

}