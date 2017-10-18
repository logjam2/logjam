// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundMultiLogWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using LogJam.Internal;
    using LogJam.Shared.Internal;
    using LogJam.Trace;
    using LogJam.Util;


    /// <summary>
    /// Provides multiple synchronized <see cref="IEntryWriter{TEntry}" />s and <see cref="ILogWriter" />s that write to
    /// corresponding <see cref="IEntryWriter{TEntry}" />s on a single background thread.
    /// This implementation minimizes the performance impact of writing to logs by allowing clients to "send and forget",
    /// until the queue is full. In normal cases all logged entries are guaranteed to be written to the background log
    /// writers,
    /// however abnormal termination of an application can result in queued entries not being written.
    /// </summary>
    internal sealed class BackgroundMultiLogWriter : Startable, IDisposable, ILogJamComponent
    {

        /// <summary>
        /// The default max length for the blocking queue for log writers.
        /// </summary>
        public const int DefaultMaxQueueLength = 512;

        private readonly ITracerFactory _setupTracerFactory;
        private readonly Tracer _tracer;

        // The set of log writers that have been proxied
        private readonly List<LogWriterProxy> _proxyLogWriters;
        // The proxy entry writers wrapping the inner entry writers
        private readonly List<object> _proxyEntryWriters;
        // Queued actions to invoke on the background thread.
        private readonly ConcurrentQueue<Action> _backgroundActionQueue;

        private BackgroundTask _backgroundTask;

        internal BackgroundMultiLogWriter(ITracerFactory setupTracerFactory)
        {
            Arg.DebugNotNull(setupTracerFactory, nameof(setupTracerFactory));

            _setupTracerFactory = setupTracerFactory;
            _tracer = setupTracerFactory.TracerFor(this);

            _proxyLogWriters = new List<LogWriterProxy>();
            _proxyEntryWriters = new List<object>();
            _backgroundActionQueue = new ConcurrentQueue<Action>();

            _backgroundTask = null;
        }

        internal BackgroundMultiLogWriter(ITracerFactory setupTracerFactory, params ILogWriter[] logWriters)
            : this(setupTracerFactory)
        {
            Arg.DebugNotNull(setupTracerFactory, nameof(setupTracerFactory));
            Arg.DebugNotNull(logWriters, nameof(logWriters));

            foreach (ILogWriter logWriter in logWriters)
            {
                CreateProxyFor(logWriter);
            }
        }

        public ITracerFactory SetupTracerFactory { get { return _setupTracerFactory; } }

        public IEnumerable<ILogWriter> ProxyLogWriters { get { return _proxyLogWriters; } }

        /// <summary>
        /// Finalizer, used to ensure that queued logs get flushed during finalization.
        /// </summary>
        ~BackgroundMultiLogWriter()
        {
            _tracer.Error("In finalizer (~BackgroundMultiLogWriter) - forgot to Dispose()?");
            Dispose(false);
        }

        /// <summary>
        /// Creates and returns a proxy <see cref="IEntryWriter{TEntry}" /> that is synchronized,
        /// and that implements blocking queue functionality for background logging.
        /// </summary>
        /// <typeparam name="TEntry">The log entry type.</typeparam>
        /// <param name="innerEntryWriter">A <see cref="IEntryWriter{TEntry}" /> that is written to in a background thread.</param>
        /// <param name="maxQueueLength">
        /// The max length for the queue. If more than this number of log entries is queued, the
        /// writer will block.
        /// </param>
        /// <returns></returns>
        private IQueueEntryWriter<TEntry> CreateProxyFor<TEntry>(IEntryWriter<TEntry> innerEntryWriter, int maxQueueLength = DefaultMaxQueueLength)
            where TEntry : ILogEntry
        {
            Arg.DebugNotNull(innerEntryWriter, nameof(innerEntryWriter));
            Arg.InRange(maxQueueLength, 0, int.MaxValue, nameof(maxQueueLength));

            lock (this)
            {
                EnsureNotDisposed();
                OperationNotSupportedWhenStarted("CreateProxyFor(IEntryWriter<TEntry>)");

                var proxyLogWriter = CreateBlockingQueueLogWriter(innerEntryWriter, maxQueueLength);
                return proxyLogWriter;
            }
        }

        private BlockingQueueEntryWriter<TEntry> CreateBlockingQueueLogWriter<TEntry>(IEntryWriter<TEntry> innerEntryWriter, int maxQueueLength)
            where TEntry : ILogEntry
        {
            var proxyEntryWriter = new BlockingQueueEntryWriter<TEntry>(innerEntryWriter, this, maxQueueLength);
            _proxyEntryWriters.Add(proxyEntryWriter);
            return proxyEntryWriter;
        }

        public ILogWriter CreateProxyFor(ILogWriter innerLogWriter, int maxQueueLength = DefaultMaxQueueLength)
        {
            Arg.NotNull(innerLogWriter, nameof(innerLogWriter));
            Arg.InRange(maxQueueLength, 0, int.MaxValue, nameof(maxQueueLength));

            lock (this)
            {
                EnsureNotDisposed();
                OperationNotSupportedWhenStarted("CreateProxyFor(ILogWriter)");

                var logWriter = new LogWriterProxy(innerLogWriter, _backgroundActionQueue, _setupTracerFactory);
                foreach (var kvp in innerLogWriter.EntryWriters)
                {
                    Type entryWriterEntryType = kvp.Key;
                    object innerEntryWriter = kvp.Value;
                    var entryTypeArgs = new Type[] { entryWriterEntryType };
                    object blockingQueueEntryWriter = this.InvokeGenericMethod(entryTypeArgs, "CreateBlockingQueueLogWriter", innerEntryWriter, maxQueueLength);
                    logWriter.InvokeGenericMethod(entryTypeArgs, "AddEntryWriter", blockingQueueEntryWriter);
                }

                _proxyLogWriters.Add(logWriter);
                return logWriter;
            }
        }

        //#region ILogWriter

        //public bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter) where TEntry : ILogEntry
        //{
        //	lock (this)
        //	{
        //		var logWriters = new List<IEntryWriter<TEntry>>();
        //		_proxyEntryWriters.GetEntryWriters(logWriters);
        //		if (logWriters.Count == 1)
        //		{
        //			entryWriter = logWriters[0];
        //			return true;
        //		}
        //		else if (logWriters.Count == 0)
        //		{
        //			entryWriter = null;
        //			return false;
        //		}
        //		else
        //		{
        //			entryWriter = new FanOutEntryWriter<TEntry>(logWriters);
        //			return true;
        //		}
        //	}
        //}

        ///// <summary>
        ///// Returns <c>true</c> if this object is ready to receive log writes.
        ///// </summary>
        ///// <remarks>IsEnabled is synonymous with <see cref="IsStarted"/> for this class.</remarks>
        //public bool IsEnabled { get { return (_backgroundTask != null) && _backgroundTask.IsStarted; } }

        //public bool IsSynchronized { get { return true; } }

        //public IEnumerator<ILogWriter> GetEnumerator()
        //{
        //	return _proxyEntryWriters.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //	return GetEnumerator();
        //}

        //#endregion

        #region IStartable

        protected override void InternalStart()
        {
            lock (this)
            {
                if (_backgroundTask == null)
                {
                    _backgroundTask = new BackgroundTask(_setupTracerFactory, _backgroundActionQueue);
                }
            }

            _proxyLogWriters.SafeStart(_setupTracerFactory);
            _proxyEntryWriters.SafeStart(_setupTracerFactory);
            _backgroundTask.Start();
        }

        protected override void InternalStop()
        {
            lock (this)
            {
                var backgroundThread = _backgroundTask;
                if (backgroundThread != null)
                {
                    _proxyLogWriters.SafeStop(_setupTracerFactory);
                    _proxyEntryWriters.SafeStop(_setupTracerFactory);
                    backgroundThread.Stop();
                }
            }
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
            if (IsDisposed)
            {
                return;
            }

            State = StartableState.Disposing;
            var backgroundThread = _backgroundTask;
            if (backgroundThread != null)
            {
                _proxyEntryWriters.SafeStop(_setupTracerFactory);
                _proxyEntryWriters.SafeDispose(_setupTracerFactory);
                _backgroundActionQueue.Enqueue(() => _proxyEntryWriters.Clear());

                backgroundThread.Stop();
                _backgroundTask = null;
            }

            State = StartableState.Disposed;
        }

        private void EnsureNotDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
        }

        private void OperationNotSupportedWhenStarted(string method)
        {
            if (IsStarted)
            {
                throw new LogJamException(string.Format("{0} not supported when instance is started.", method), this);
            }
        }

        /// <summary>
        /// Used only for test verification.
        /// </summary>
        internal bool IsBackgroundThreadRunning { get { return _backgroundTask != null && _backgroundTask.IsTaskRunning; } }


        /// <summary>
        /// The set of operations that are executed on the background thread. All these methods must be valid
        /// <see cref="Action" />s.
        /// </summary>
        private interface IBackgroundThreadLogWriterActions
        {

            void StartInnerWriter();

            void DequeAndWriteEntry();

            void StopInnerWriter();

            void DisposeInnerWriter();

        }


        /// <summary>
        /// A proxy <see cref="ILogWriter" /> that can be accessed in the foreground thread, but that queues Start() and
        /// Stop() operations to the background thread.
        /// </summary>
        private class LogWriterProxy : BaseLogWriter
        {

            // The ILogWriter that is accessed only on the background thread
            private readonly ILogWriter _innerLogWriter;
            // References parent._backgroundActionQueue
            private readonly ConcurrentQueue<Action> _backgroundActionQueue;

            internal LogWriterProxy(ILogWriter innerLogWriter, ConcurrentQueue<Action> backgroundActionQueue, ITracerFactory setupTracerFactory)
                : base(setupTracerFactory)
            {
                Arg.DebugNotNull(innerLogWriter, nameof(innerLogWriter));
                Arg.DebugNotNull(backgroundActionQueue, nameof(backgroundActionQueue));
                Arg.DebugNotNull(setupTracerFactory, nameof(setupTracerFactory));

                _innerLogWriter = innerLogWriter;
                _backgroundActionQueue = backgroundActionQueue;
            }

            internal ILogWriter InnerLogWriter { get { return _innerLogWriter; } }

            public override bool IsSynchronized { get { return true; } }

            private void QueueBackgroundAction(Action backgroundAction)
            {
                _backgroundActionQueue.Enqueue(backgroundAction);
            }

            protected override void InternalStart()
            {
                if (_innerLogWriter is IStartable startableLogWriter)
                {
                    QueueBackgroundAction(() => startableLogWriter.SafeStart(SetupTracerFactory));
                }

                base.InternalStart();
            }

            protected override void InternalStop()
            {
                base.InternalStop();

                if (_innerLogWriter is IStartable startableLogWriter)
                {
                    QueueBackgroundAction(() => startableLogWriter.SafeStop(SetupTracerFactory));
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (_innerLogWriter is IDisposable disposableLogWriter)
                {
                    QueueBackgroundAction(() => disposableLogWriter.SafeDispose(SetupTracerFactory));
                }
            }

        }


        /// <summary>
        /// A proxy <see cref="IEntryWriter{TEntry}" /> implementation that queues elements so they can be written on a background
        /// thread.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        private class BlockingQueueEntryWriter<TEntry> : Startable, IQueueEntryWriter<TEntry>, IBackgroundThreadLogWriterActions, IDisposable
            where TEntry : ILogEntry
        {

            private readonly IEntryWriter<TEntry> _innerEntryWriter;
            private readonly ConcurrentQueue<TEntry> _queue;
            private readonly SemaphoreSlim _slotsLeftInQueue;

            // References parent._backgroundActionQueue
            private readonly ConcurrentQueue<Action> _backgroundActionQueue;
            // References parent._setupTracerFactory
            private readonly ITracerFactory _setupTracerFactory;

            internal BlockingQueueEntryWriter(IEntryWriter<TEntry> innerEntryWriter, BackgroundMultiLogWriter parent, int maxQueueLength)
            {
                Arg.DebugNotNull(innerEntryWriter, nameof(innerEntryWriter));
                Arg.DebugNotNull(parent, nameof(parent));
                Arg.InRange(maxQueueLength, 0, Int32.MaxValue, nameof(maxQueueLength));

                _innerEntryWriter = innerEntryWriter;
                _queue = new ConcurrentQueue<TEntry>();
                _slotsLeftInQueue = new SemaphoreSlim(maxQueueLength);

                _backgroundActionQueue = parent._backgroundActionQueue;
                _setupTracerFactory = parent._setupTracerFactory;

                State = _innerEntryWriter.IsEnabled ? StartableState.Started : StartableState.Unstarted;
            }

            private void QueueBackgroundAction(Action backgroundAction)
            {
                _backgroundActionQueue.Enqueue(backgroundAction);
            }

            public void Write(ref TEntry entry)
            {
                if (! IsEnabled)
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

            public bool IsEnabled => IsStarted;

            public bool IsSynchronized { get { return true; } }

            public bool IsEmpty { get { return _queue.IsEmpty; } }

            public bool TryDequeue(out TEntry logEntry)
            {
                bool success = _queue.TryDequeue(out logEntry);
                if (success)
                {
                    _slotsLeftInQueue.Release();
                }
                return success;
            }

            protected override void InternalStart()
            {
                if (_innerEntryWriter is IStartable)
                {
                    QueueBackgroundAction(StartInnerWriter);
                }

                // The QueueEntryWriter is considered started as soon as the start signal is sent;
                // state should be "started" when Start() returns.
                // In the case of the QueueEntryWriter, "Started" means "ready to accept entries".
                // We could wait for a queued action to execute before changing the state to started,
                // but that seems like an unnecessary wait.
            }

            protected override void InternalStop()
            {
                if (_innerEntryWriter is IStartable)
                {
                    // Blocks if maxQueueLength is exceeded
                    _slotsLeftInQueue.Wait();

                    QueueBackgroundAction(StopInnerWriter);
                }

                // Wait for stop to finish on the background thread
                var waitEvent = new ManualResetEventSlim();
                QueueBackgroundAction(() =>
                                      {
                                          waitEvent.Set();
                                      });
                waitEvent.Wait(1000); // REVIEW: Add support for cancellation token to the BackgroundMultiLogWriter?
            }

            public void Dispose()
            {
                lock (this)
                {
                    if (IsDisposed)
                    {
                        return;
                    }
                    State = StartableState.Disposing;
                }

                if (_innerEntryWriter is IStartable)
                {
                    // Blocks if maxQueueLength is exceeded
                    _slotsLeftInQueue.Wait();

                    QueueBackgroundAction(StopInnerWriter);
                }

                if (_innerEntryWriter is IDisposable)
                {
                    // Blocks if maxQueueLength is exceeded
                    _slotsLeftInQueue.Wait();

                    QueueBackgroundAction(DisposeInnerWriter);
                }

                // No need to wait for dispose to finish on the background thread:
                // This class is private and is controlled by BackgroundMultiLogWriter.Dispose(), which doesn't return
                // until the thread exits.
                // Plus, if this class is called in the finalizer, there's no guarantee the thread hasn't already exited.
                State = StartableState.Disposed;
            }

            #region IBackgroundThreadLogWriterActions

            public void StartInnerWriter()
            {
                if ((_innerEntryWriter is IStartable startableInnerLogWriter)
                    && (! startableInnerLogWriter.IsStarted))
                {
                    // Start is delegated on the foreground thread
                    startableInnerLogWriter.SafeStart(_setupTracerFactory);
                }
            }

            public void DequeAndWriteEntry()
            {
                bool success = TryDequeue(out var logEntry);
                if (success)
                {
                    _innerEntryWriter.Write(ref logEntry);
                }
            }

            public void StopInnerWriter()
            {
                (_innerEntryWriter as IStartable).SafeStop(_setupTracerFactory);
                _slotsLeftInQueue.Release();
            }

            public void DisposeInnerWriter()
            {
                (_innerEntryWriter as IDisposable).SafeDispose(_setupTracerFactory);
                _slotsLeftInQueue.Release();
            }

            #endregion
        }


        /// <summary>
        /// Encapsulates the logic that runs on a background thread, receiving write actions from a queue that is filled by
        /// the containing <see cref="BackgroundMultiLogWriter" /> instance.
        /// </summary>
        private class BackgroundTask : IStartable
        {

            private readonly Tracer _tracer;
            // Queued actions to invoke on the background thread.
            private readonly ConcurrentQueue<Action> _backgroundActionQueue;
            private volatile StartableState _startableState;
            private Task _task;

            // REVIEW: It's important that this object has NO reference to the parent BackgroundMultiLogWriter.
            // If there were a reference from this, it would never finalize.

            public BackgroundTask(ITracerFactory setupTracerFactory, ConcurrentQueue<Action> backgroundActionQueue)
            {
                Arg.DebugNotNull(setupTracerFactory, nameof(setupTracerFactory));
                Arg.DebugNotNull(backgroundActionQueue, nameof(backgroundActionQueue));

                _tracer = setupTracerFactory.TracerFor(this);
                _backgroundActionQueue = backgroundActionQueue;
            }

            #region IStartable

            public StartableState State { get { return _startableState; } }

            /// @inheritdoc
            public bool IsStarted { get { return _startableState == StartableState.Started; } }

            /// @inheritdoc
            public bool ReadyToStart
            {
                get
                {
                    var state = _startableState;
                    return ((state == StartableState.Unstarted) || (state == StartableState.Stopped));
                }
            }

            public void Start()
            {
                lock (this)
                {
                    if (IsStarted)
                    {
                        return;
                    }

                    System.Diagnostics.Debug.Assert(! IsTaskRunning);

                    _startableState = StartableState.Starting;
                    _task = new Task(BackgroundThreadProc, TaskCreationOptions.LongRunning);
                    _task.Start();
                }
            }

            public void Stop()
            {
                lock (this)
                {
                    var task = _task;
                    if (task != null)
                    {
                        _startableState = StartableState.Stopping;
                        _task.Wait();
                    }

                    _startableState = StartableState.Stopped;
                }
            }

            #endregion

            internal bool IsTaskRunning
            {
                get
                {
                    var task = _task;
                    if (task == null)
                    {
                        return false;
                    }
                    else if ( task.Status == TaskStatus.Running)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            /// <summary>
            /// ThreadProc for the background thread. At any one time, there should be 0 or 1 background threads for each
            /// <see cref="BackgroundMultiLogWriter" />
            /// instance.
            /// </summary>
            private void BackgroundThreadProc()
            {
                _startableState = StartableState.Started;
                _tracer.Info("Started background thread.");

                SpinWait spinWait = new SpinWait();
                while (true)
                {
                    if (_backgroundActionQueue.TryDequeue(out var action))
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
                    else if (spinWait.NextSpinWillYield && _startableState == StartableState.Stopping)
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
                _task = null;
            }

        }

    }

}
