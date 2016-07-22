// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizingProxyLogWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;

    using LogJam.Trace;
    using LogJam.Util;


    /// <summary>
    /// A <see cref="ProxyLogWriter" /> that synchronizes all writes to an inner <see cref="ILogWriter" />.
    /// </summary>
    public sealed class SynchronizingProxyLogWriter : ProxyLogWriter, IStartable
    {

        // Use SpinLock because it should be more efficient at times.
        private SpinLock _spinLock;
        private readonly Dictionary<Type, object> _entryWriters;

        public SynchronizingProxyLogWriter(ITracerFactory setupTracerFactory, ILogWriter innerLogWriter)
            : base(setupTracerFactory, innerLogWriter)
        {
            _spinLock = new SpinLock();
            _entryWriters = new Dictionary<Type, object>();
        }

        private SynchronizingProxyEntryWriter<TEntry> CreateSynchronizingProxyEntryWriter<TEntry>(IEntryWriter<TEntry> innerEntryWriter)
            where TEntry : ILogEntry
        {
            return new SynchronizingProxyEntryWriter<TEntry>(this, innerEntryWriter);
        }

        protected override void InternalStart()
        {
            bool lockTaken = false;
            _spinLock.Enter(ref lockTaken);
            try
            {
                base.InternalStart();

                // Fill _entryWriters with SychronizingProxyEntryWriters
                _entryWriters.Clear();
                foreach (KeyValuePair<Type, object> kvp in InnerLogWriter.EntryWriters)
                {
                    Type entryWriterEntryType = kvp.Key;
                    object innerEntryWriter = kvp.Value;
                    var entryTypeArgs = new Type[] { entryWriterEntryType };
                    object synchronizingEntryWriter = this.InvokeGenericMethod(entryTypeArgs, "CreateSynchronizingProxyEntryWriter", innerEntryWriter);
                    _entryWriters.Add(entryWriterEntryType, synchronizingEntryWriter);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _spinLock.Exit(false);
                }
            }
        }

        protected override void InternalStop()
        {
            bool lockTaken = false;
            _spinLock.Enter(ref lockTaken);
            try
            {
                base.InternalStop();

                _entryWriters.Clear();
            }
            finally
            {
                if (lockTaken)
                {
                    _spinLock.Exit(false);
                }
            }
        }

        public override void Dispose()
        {
            bool lockTaken = false;
            _spinLock.Enter(ref lockTaken);
            try
            {
                base.Dispose();

                _entryWriters.Clear();
            }
            finally
            {
                if (lockTaken)
                {
                    _spinLock.Exit(false);
                }
            }
        }

        #region ILogWriter

        public override bool IsSynchronized { get { return true; } }

        public override bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter)
        {
            object objEntryWriter;
            if (! _entryWriters.TryGetValue(typeof(TEntry), out objEntryWriter))
            {
                entryWriter = null;
                return false;
            }

            entryWriter = objEntryWriter as IEntryWriter<TEntry>;
            if (entryWriter == null)
            {
                return false;
            }
            return true;
        }

        public override IEnumerable<KeyValuePair<Type, object>> EntryWriters { get { return _entryWriters; } }

        #endregion

        internal class SynchronizingProxyEntryWriter<TEntry> : ProxyEntryWriter<TEntry>
            where TEntry : ILogEntry
        {

            private readonly SynchronizingProxyLogWriter _parent;

            internal SynchronizingProxyEntryWriter(SynchronizingProxyLogWriter parent, IEntryWriter<TEntry> innerEntryWriter)
                : base(innerEntryWriter)
            {
                Contract.Requires<ArgumentNullException>(parent != null);

                _parent = parent;
            }

            public override void Write(ref TEntry entry)
            {
                bool lockTaken = false;
                _parent._spinLock.Enter(ref lockTaken);
                try
                {
                    base.Write(ref entry);
                }
                finally
                {
                    if (lockTaken)
                    {
                        _parent._spinLock.Exit(false);
                    }
                }
            }

            public override void Dispose()
            {
                bool lockTaken = false;
                _parent._spinLock.Enter(ref lockTaken);
                try
                {
                    base.Dispose();
                }
                finally
                {
                    if (lockTaken)
                    {
                        _parent._spinLock.Exit(false);
                    }
                }
            }

        }

    }

}
