// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizingProxyLogWriter.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using LogJam.Config;
using LogJam.Config.Initializer;
using LogJam.Shared.Internal;
using LogJam.Trace;
using LogJam.Util;

namespace LogJam.Writer
{

    /// <summary>
    /// A <see cref="ProxyLogWriter" /> that synchronizes all writes to an inner <see cref="ILogWriter" />.
    /// </summary>
    public sealed class SynchronizingProxyLogWriter : ProxyLogWriter, ISynchronizingLogWriter
    {

        // Internal lock object
        private readonly object _lock;

        // Queue of actions to be run synchonously after the next entry is logged
        private readonly ConcurrentQueue<Action> _syncActionQueue;

        public SynchronizingProxyLogWriter(ITracerFactory setupTracerFactory, ILogWriter innerLogWriter)
            : base(setupTracerFactory, innerLogWriter)
        {
            _lock = new object();
            _syncActionQueue = new ConcurrentQueue<Action>();
        }

        public void QueueSynchronized(Action action, LogWriterActionPriority priority)
        {
            switch (priority)
            {
                case LogWriterActionPriority.Delay:
                    // Queueing the action on the ThreadPool works, but provides no assurance that action will be run before other synchronized log writes.
                    Task.Run(() =>
                             {
                                 lock (_lock)
                                 {
                                     action();
                                 }
                             });
                    break;

                case LogWriterActionPriority.Normal:
                    // Run action at the beginning of the next EntryWriter<TEntry>.Write()
                    _syncActionQueue.Enqueue(action);
                    break;

                case LogWriterActionPriority.High:
                    // Run action at the beginning of the next EntryWriter<TEntry>.Write(), or on a ThreadPool task, whichever comes first
                    _syncActionQueue.Enqueue(action);
                    Task.Run(() =>
                             {
                                 lock (_lock)
                                 {
                                     RunQueuedActions();
                                 }
                             });
                    break;

                default:
                    throw new ArgumentException("Priority " + priority + " is not an acceptable value.");
            }
        }

        protected override void InternalStart()
        {
            lock (_lock)
            {
                (InnerLogWriter as IStartable).SafeStart(SetupTracerFactory);

                // Fill EntryWriters with SychronizingProxyEntryWriters
                ClearEntryWriters();
                foreach (var kvp in InnerLogWriter.EntryWriters)
                {
                    Type entryWriterEntryType = kvp.Key;
                    object innerEntryWriter = kvp.Value;
                    var entryTypeArgs = new Type[] { entryWriterEntryType };
                    object synchronizingEntryWriter = this.InvokeGenericMethod(entryTypeArgs, "CreateSynchronizingProxyEntryWriter", innerEntryWriter);
                    this.InvokeGenericMethod(entryTypeArgs, "AddEntryWriter", synchronizingEntryWriter);
                }

                EntryWriters.Select(kvp => kvp.Value).SafeStart(SetupTracerFactory);
            }
        }

        protected override void InternalStop()
        {
            lock (_lock)
            {
                // Run queued actions before stopping
                RunQueuedActions();

                base.InternalStop();
                ClearEntryWriters();
            }
        }

        protected override void Dispose(bool disposing)
        {
            lock (_lock)
            {
                EntryWriters.SafeDispose(SetupTracerFactory);
                ClearEntryWriters();

                (InnerLogWriter as IDisposable).SafeDispose(SetupTracerFactory);
            }
        }

        private void RunQueuedActions()
        {
            // Run all queued actions
            while (_syncActionQueue.TryDequeue(out var syncAction))
            {
                syncAction();
            }
        }

        // ReSharper disable once UnusedMember.Local
        /// <summary>
        /// Called via reflection to create a <see cref="SynchronizingProxyEntryWriter{TEntry}" />
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="innerEntryWriter"></param>
        /// <returns></returns>
        private SynchronizingProxyEntryWriter<TEntry> CreateSynchronizingProxyEntryWriter<TEntry>(IEntryWriter<TEntry> innerEntryWriter)
            where TEntry : ILogEntry
        {
            return new SynchronizingProxyEntryWriter<TEntry>(this, innerEntryWriter);
        }


        /// <summary>
        /// Standard initializer to create a <see cref="SynchronizingProxyLogWriter" /> if <see cref="ILogWriterConfig.Synchronize" /> is <c>true</c> and <see cref="ILogWriterConfig.BackgroundLogging" /> is
        /// <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This initializer is included in <see cref="LogManagerConfig.Initializers" /> by default.
        /// </remarks>
        public sealed class Initializer : IExtendLogWriterPipeline
        {

            public ILogWriter InitializeLogWriter(ITracerFactory setupTracerFactory, ILogWriter logWriter, DependencyDictionary dependencyDictionary)
            {
                var logWriterConfig = dependencyDictionary.Get<ILogWriterConfig>();
                if (! logWriter.IsSynchronized && logWriterConfig.Synchronize && ! logWriterConfig.BackgroundLogging)
                {
                    // Wrap non-synchronized LogWriters to make them threadsafe
                    var synchronizingLogWriter = new SynchronizingProxyLogWriter(setupTracerFactory, logWriter);
                    setupTracerFactory.TracerFor(this).Verbose("Adding synchronizing proxy in front of {0}", logWriter);
                    dependencyDictionary.AddIfNotDefined(typeof(ISynchronizingLogWriter), synchronizingLogWriter);
                    return synchronizingLogWriter;
                }
                else
                {
                    return logWriter;
                }
            }

        }


        /// <summary>
        /// A proxy EntryWriter that ensures that all operations are synchronized (ensured sequential/non-overlapping) via the
        /// parent <see cref="SynchronizingProxyLogWriter" />'s lock.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        internal sealed class SynchronizingProxyEntryWriter<TEntry> : ProxyEntryWriter<TEntry>
            where TEntry : ILogEntry
        {

            private readonly SynchronizingProxyLogWriter _parent;

            internal SynchronizingProxyEntryWriter(SynchronizingProxyLogWriter parent, IEntryWriter<TEntry> innerEntryWriter)
                : base(innerEntryWriter)
            {
                Arg.NotNull(parent, nameof(parent));

                _parent = parent;
            }

            public override void Write(ref TEntry entry)
            {
                lock (_parent._lock)
                {
                    // Run actions queued in parent logwriter
                    _parent.RunQueuedActions();

                    // Delegate to the inner EntryWriter
                    base.Write(ref entry);
                }
            }

            public override void Dispose()
            {
                lock (_parent._lock)
                {
                    base.Dispose();
                }
            }

        }


        #region ILogWriter

        public override bool IsSynchronized => true;

        #endregion

    }

}
