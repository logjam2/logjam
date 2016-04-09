// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseLogWriter.cs">
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
    using System.Linq;

    using LogJam.Internal;
    using LogJam.Trace;
    using LogJam.Util;


    /// <summary>
    /// Common implementation for <see cref="ILogWriter" />s.
    /// </summary>
    public abstract class BaseLogWriter : Startable, ILogWriter, IDisposable, ILogJamComponent
    {

        private readonly ITracerFactory _setupTracerFactory;

        private readonly Dictionary<Type, object> _entryWriters;

        /// <summary>
        /// Creates a new <see cref="BaseLogWriter" />.
        /// </summary>
        protected BaseLogWriter(ITracerFactory setupTracerFactory)
        {
            Contract.Requires<ArgumentNullException>(setupTracerFactory != null);

            _setupTracerFactory = setupTracerFactory;

            _entryWriters = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Returns the <see cref="ITracerFactory" /> to use when logging setup operations.
        /// </summary>
        protected ITracerFactory SetupTracerFactory { get { return _setupTracerFactory; } }

        ITracerFactory ILogJamComponent.SetupTracerFactory { get { return _setupTracerFactory; } }

        protected internal void AddEntryWriter<TEntry>(IEntryWriter<TEntry> entryWriter)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(entryWriter != null);

            if (IsStarted)
            {
                throw new LogJamSetupException("New entry writers cannot be added after starting.", this);
            }

            lock (this)
            {
                try
                {
                    _entryWriters.Add(typeof(TEntry), entryWriter);
                }
                catch (ArgumentException argExcp)
                {
                    throw new LogJamSetupException("Cannot add 2nd writer for Entry type " + typeof(TEntry), argExcp, this);
                }
            }
        }

        public void Dispose()
        {
            Stop();
            lock (this)
            {
                if (! IsDisposed)
                {
                    State = StartableState.Disposing;
                    Dispose(true);
                    GC.SuppressFinalize(this);
                    State = StartableState.Disposed;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {}

        protected void EnsureNotDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
        }

        #region ILogWriter

        public virtual bool IsSynchronized { get { return false; } }

        public virtual bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter) where TEntry : ILogEntry
        {
            lock (this)
            {
                object untypedEntryWriter;
                if (_entryWriters.TryGetValue(typeof(TEntry), out untypedEntryWriter))
                {
                    entryWriter = (IEntryWriter<TEntry>) untypedEntryWriter;
                    return true;
                }
                else
                {
                    entryWriter = null;
                    return false;
                }
            }
        }

        public virtual IEnumerable<KeyValuePair<Type, object>> EntryWriters { get { return _entryWriters.ToArray(); } }

        #endregion

        #region Startable overrides

        protected override void InternalStart()
        {
			EntryWriters.Select(kvp => kvp.Value).SafeStart(SetupTracerFactory);
        }

        protected override void InternalStop()
        {
			EntryWriters.Select(kvp => kvp.Value).SafeStop(SetupTracerFactory);
        }

        #endregion
    }

}
