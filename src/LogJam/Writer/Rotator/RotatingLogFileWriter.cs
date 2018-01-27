// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotatingLogFileWriter.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.IO;
using System.Threading.Tasks;

using LogJam.Config;
using LogJam.Shared.Internal;
using LogJam.Trace;
using LogJam.Util;

namespace LogJam.Writer.Rotator
{

    /// <summary>
    /// Implements writing to log files that rotate based on the provided <see cref="ILogFileRotator" />.
    /// </summary>
    /// <remarks>
    /// The reason that <c>RotatingLogFileWriter</c> doesn't subclass <see cref="ProxyLogWriter" /> is that the inner (proxied) log writer
    /// changes every time the log file is rotated. If <c>RotatingLogFileWriter</c> were a <see cref="ProxyLogWriter" /> it would suggest that
    /// the inner log writer should be stable/reliable.
    /// </remarks>
    public sealed class RotatingLogFileWriter : BaseLogWriter
    {

        /// <summary>
        /// Performs file rotation triggering and file rotation logic.
        /// </summary>
        private readonly ILogFileRotator _logFileRotator;

        /// <summary>
        /// Used to create the proxied <see cref="ILogWriter" /> for each file.
        /// </summary>
        private readonly ILogFileWriterConfig _logFileWriterConfig;

        private readonly Tracer _tracer;

        private ILogWriter _currentLogWriter;

        /// <summary>
        /// Supports synchronizing file rotation so that rotation occurs while file writes and flushes aren't executing.
        /// </summary>
        private ISynchronizingLogWriter _synchronizingLogWriter;

        public RotatingLogFileWriter(ITracerFactory setupTracerFactory,
                                     ILogFileWriterConfig logFileWriterConfig,
                                     ILogFileRotator logFileRotator)
            : base(setupTracerFactory)
        {
            Arg.NotNull(logFileWriterConfig, nameof(logFileWriterConfig));
            Arg.NotNull(logFileRotator, nameof(logFileRotator));

            _tracer = SetupTracerFactory.TracerFor(this);
            _logFileWriterConfig = logFileWriterConfig;
            _logFileRotator = logFileRotator;
        }

        /// <summary>
        /// The <see cref="ILogFileRotator"/> used to manage file rotation.
        /// </summary>
        public ILogFileRotator LogFileRotator { get { return _logFileRotator; } }

        #region ILogWriter

        public override bool IsSynchronized { get { return _currentLogWriter != null && _currentLogWriter.IsSynchronized; } }

        #endregion

        /// <summary>
        /// Initialized by <see cref="RotatingLogFileWriterConfig" /> as part of logwriter pipeline setup.
        /// </summary>
        /// <param name="synchronizingLogWriter"></param>
        internal void SetSynchronizingLogWriter(ISynchronizingLogWriter synchronizingLogWriter)
        {
            Arg.NotNull(synchronizingLogWriter, nameof(synchronizingLogWriter));

            _synchronizingLogWriter = synchronizingLogWriter;
        }

        /// <summary>
        /// Event that is fired after a log file is rotated.
        /// </summary>
        public event EventHandler<AfterRotateLogFileEventArgs> AfterRotate;

        /// <summary>
        /// Event handler for <see cref="ILogFileRotator.TriggerRotate" />.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rotateEventArgs"></param>
        private void HandleTriggerRotateEvent(object source, RotateLogFileEventArgs rotateEventArgs)
        {
            _tracer.Debug("Queueing log rotation to {0}", rotateEventArgs.NextLogFile);
            _synchronizingLogWriter.QueueSynchronized(() => SynchronizedRotate(rotateEventArgs), rotateEventArgs.Priority);
        }

        /// <summary>
        /// Perform the rotation while synchronized with writes, flushes, etc.
        /// </summary>
        /// <param name="rotateEventArgs"></param>
        private void SynchronizedRotate(RotateLogFileEventArgs rotateEventArgs)
        {
            _tracer.Debug("Starting log rotation to {0}", rotateEventArgs.NextLogFile);

            // Delegate to the LogFileRotator - this way rotation logic is completely pluggable.
            Action cleanupAction = _logFileRotator.Rotate(this, rotateEventArgs);

            // Queue creating the cleanup task so it doesn't start until currently queued operations complete;
            // and then run it in a Task (doesn't have to be synchronized with log writes).
            if (cleanupAction != null)
            {
                _synchronizingLogWriter.QueueSynchronized(() => new Task(cleanupAction).Start(), LogWriterActionPriority.Normal);
            }

            _tracer.Debug("Completed log rotation to {0}", rotateEventArgs.NextLogFile);
        }

        /// <summary>
        /// Switches to a new log file writer that writes to <paramref name="nextLogFile" />. Includes switching all
        /// entry writers to write to the next log file.
        /// </summary>
        /// <param name="nextLogFile"></param>
        /// <returns>
        /// A cleanup <see cref="Action" />, which flushes and closes the previous log file. This action should be
        /// queued for execution soon after rotation completes, but does not need to be run synchronously with rotation as
        /// it may be time consuming.
        /// </returns>
        public Action SwitchLogFileWriterTo(FileInfo nextLogFile)
        {
            lock (this)
            {
                var newLogWriter = CreateAndStartLogFileWriterForNewLogFile(nextLogFile);
                var previousLogWriter = _currentLogWriter;
                foreach (var kvp in EntryWriters)
                {
                    IRotatingEntryWriter rotatingEntryWriter = kvp.Value as IRotatingEntryWriter;
                    rotatingEntryWriter?.SwitchInnerWriter(newLogWriter, SetupTracerFactory);
                }

                _currentLogWriter = newLogWriter;

                // Return cleanup action
                return () =>
                       {
                           _tracer.Info("Closing {0}", previousLogWriter);
                           (previousLogWriter as IStartable).SafeStop(SetupTracerFactory);
                           (previousLogWriter as IDisposable).SafeDispose(SetupTracerFactory);
                       };
            }
        }

        private ILogWriter CreateAndStartLogFileWriterForNewLogFile(FileInfo logFile)
        {
            lock (this)
            {
                _logFileWriterConfig.LogFile.Directory = logFile.DirectoryName;
                _logFileWriterConfig.LogFile.Filename = logFile.Name;

                ILogWriter logWriter = LogManager.CreateLogWriter(_logFileWriterConfig, SetupTracerFactory);
                (logWriter as IStartable).SafeStart(SetupTracerFactory);
                return logWriter;
            }
        }

        // ReSharper disable once UnusedMember.Local
        // Called from <see cref="InternalStart"/> via reflection.
        private RotatingEntryWriter<TEntry> CreateRotatingEntryWriter<TEntry>(IEntryWriter<TEntry> innerEntryWriter)
            where TEntry : ILogEntry
        {
            return new RotatingEntryWriter<TEntry>(innerEntryWriter);
        }


        private interface IRotatingEntryWriter
        {

            void SwitchInnerWriter(ILogWriter newInnerLogWriter, ITracerFactory setupTracerFactory);

        }


        private class RotatingEntryWriter<TEntry> : ProxyEntryWriter<TEntry>, IRotatingEntryWriter
            where TEntry : ILogEntry
        {

            /// <summary>
            /// Creates a new <see cref="ProxyEntryWriter{TEntry}" />.
            /// </summary>
            /// <param name="innerEntryWriter">The inner <see cref="IEntryWriter{TEntry}" /> to delegate to. Must not be <c>null</c>.</param>
            public RotatingEntryWriter(IEntryWriter<TEntry> innerEntryWriter)
                : base(innerEntryWriter)
            { }

            public void SwitchInnerWriter(ILogWriter newInnerLogWriter, ITracerFactory setupTracerFactory)
            {
                if (! newInnerLogWriter.TryGetEntryWriter(out IEntryWriter<TEntry> newInnerEntryWriter))
                {
                    setupTracerFactory.TracerFor(this)
                                      .Severe("Unable to obtain new EntryWriter of type {0} from inner logwriter {1}; disabling logging of {2}",
                                              typeof(IEntryWriter<TEntry>).GetCSharpName(),
                                              newInnerLogWriter,
                                              typeof(TEntry).GetCSharpName());
                    newInnerEntryWriter = new NoOpEntryWriter<TEntry>();
                }

                IEntryWriter<TEntry> oldInnerEntryWriter = InnerEntryWriter;

                if (ReferenceEquals(newInnerEntryWriter, oldInnerEntryWriter))
                {
                    return;
                }

                // Start the EntryWriter, then switch
                if ((newInnerEntryWriter as IStartable).SafeStart(setupTracerFactory))
                {
                    InnerEntryWriter = newInnerEntryWriter;
                }
                else
                {
                    setupTracerFactory.TracerFor(this)
                                      .Severe("Unable to start {0}; disabling logging of {1}", newInnerEntryWriter, typeof(TEntry).GetCSharpName());
                    InnerEntryWriter = new NoOpEntryWriter<TEntry>();
                }

                // Shut down the old one
                if (oldInnerEntryWriter != null)
                {
                    (oldInnerEntryWriter as IStartable).SafeStop(setupTracerFactory);
                }
            }

        }


        #region IStartable and IDisposable

        protected override void InternalStart()
        {
            if (_synchronizingLogWriter == null)
            {
                throw new LogJamStartException("ISynchronizingLogWriter reference not set - synchronization is required for RotatingLogFileWriter to work.", this);
            }

            if (_logFileRotator is IStartable startableRotator)
            {
                startableRotator.SafeStart(SetupTracerFactory);
            }

            _logFileRotator.TriggerRotate += HandleTriggerRotateEvent;

            FileInfo logFile = _logFileRotator.CurrentLogFile;
            if (logFile == null)
            {
                throw new LogJamStartException("No CurrentLogFile returned from LogFileRotator {0}", _logFileRotator);
            }

            lock (this)
            {
                _currentLogWriter = CreateAndStartLogFileWriterForNewLogFile(logFile);

                // Fill EntryWriters with RotatingEntryWriters (proxying _currentLogWriter's entry writers)
                ClearEntryWriters();
                foreach (var kvp in _currentLogWriter.EntryWriters)
                {
                    Type entryWriterEntryType = kvp.Key;
                    object innerEntryWriter = kvp.Value;
                    var entryTypeArgs = new Type[] { entryWriterEntryType };
                    object synchronizingEntryWriter = this.InvokeGenericMethod(entryTypeArgs, "CreateRotatingEntryWriter", innerEntryWriter);
                    this.InvokeGenericMethod(entryTypeArgs, "AddEntryWriter", synchronizingEntryWriter);
                }

                base.InternalStart();
            }
        }

        protected override void InternalStop()
        {
            _logFileRotator.TriggerRotate -= HandleTriggerRotateEvent;

            if (_logFileRotator is IStartable startableRotator)
            {
                startableRotator.SafeStop(SetupTracerFactory);
            }

            lock (this)
            {
                (_currentLogWriter as IStartable).SafeStop(SetupTracerFactory);
                base.InternalStop();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();

                (_currentLogWriter as IDisposable).SafeDispose(SetupTracerFactory);
                (_logFileRotator as IDisposable).SafeDispose(SetupTracerFactory);
            }

            base.Dispose(disposing);
        }

        #endregion

    }

}
