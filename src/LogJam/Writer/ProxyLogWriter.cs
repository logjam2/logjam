// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyLogWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using LogJam.Internal;
    using LogJam.Trace;
    using LogJam.Util;


    /// <summary>
    /// An <see cref="ILogWriter" /> that delegates to an inner <see cref="ILogWriter" />.
    /// </summary>
    public abstract class ProxyLogWriter : Startable, ILogWriter, IDisposable, ILogJamComponent
    {

        private readonly ITracerFactory _setupTracerFactory;

        private readonly ILogWriter _innerLogWriter;
        private bool _disposed = false;

        /// <summary>
        /// Creates a new <see cref="ProxyLogWriter" />.
        /// </summary>
        /// <param name="setupTracerFactory">The <see cref="ITracerFactory" /> tracing setup operations.</param>
        /// <param name="innerLogWriter">The inner <see cref="ILogWriter" /> to delegate to.  Must not be <c>null</c>.</param>
        protected ProxyLogWriter(ITracerFactory setupTracerFactory, ILogWriter innerLogWriter)
        {
            Contract.Requires<ArgumentNullException>(setupTracerFactory != null);
            Contract.Requires<ArgumentNullException>(innerLogWriter != null);

            _setupTracerFactory = setupTracerFactory;
            _innerLogWriter = innerLogWriter;
        }

        /// <summary>
        /// Returns the inner <see cref="ILogWriter" /> that this <c>ProxyLogWriter</c>
        /// forwards to.
        /// </summary>
        public ILogWriter InnerLogWriter { get { return _innerLogWriter; } }

        #region ILogWriter

        public virtual bool IsSynchronized { get { return InnerLogWriter.IsSynchronized; } }

        public abstract bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter) where TEntry : ILogEntry;

        public abstract IEnumerable<KeyValuePair<Type, object>> EntryWriters { get; }

        #endregion

        #region Startable overrides

        protected override void InternalStart()
        {
            (InnerLogWriter as IStartable).SafeStart(SetupTracerFactory);
        }

        protected override void InternalStop()
        {
            (InnerLogWriter as IStartable).SafeStop(SetupTracerFactory);
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            if (! _disposed)
            {
                IDisposable innerDisposable = _innerLogWriter as IDisposable;
                if (innerDisposable != null)
                {
                    innerDisposable.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion

        #region ILogJamComponent

        public ITracerFactory SetupTracerFactory { get { return _setupTracerFactory; } }

        #endregion
    }

}
