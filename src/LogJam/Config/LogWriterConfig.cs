// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System;
    using System.Collections.Generic;

    using LogJam.Config.Initializer;
    using LogJam.Trace;
    using LogJam.Trace.Config;
    using LogJam.Writer;


    /// <summary>
    /// Base class for logwriter configuration.
    /// </summary>
    /// <see cref="TraceWriterConfig.LogWriterConfig" />
    public abstract class LogWriterConfig : ILogWriterConfig
    {

        private bool _synchronized = true;

        /// <summary>
        /// Sets or gets whether the <see cref="IEntryWriter{TEntry}" /> returned from <see cref="CreateLogWriter" /> should have
        /// its writes synchronized or not. Default is <c>true</c>; however <c>false</c> is returned if
        /// <see cref="BackgroundLogging" />
        /// is <c>true</c>, to avoid synchronization overhead when logging on a single background thread.
        /// </summary>
        public virtual bool Synchronize
        {
            get { return _synchronized && ! BackgroundLogging; }
            set { _synchronized = value; }
        }

        /// <summary>
        /// Sets or gets whether log writes should be queued from the logging thread, and written on a single background thread.
        /// Default is <c>false</c>.
        /// </summary>
        public virtual bool BackgroundLogging { get; set; }

        /// <summary>
        /// Sets or gets whether the <see cref="ILogWriter" /> created by <see cref="CreateLogWriter" /> should be disposed
        /// when the <see cref="LogManager" /> is stopped. Default is <c>true</c>.
        /// </summary>
        public virtual bool DisposeOnStop { get; set; } = true;

        /// <inheritdoc />
        public abstract ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory);

        /// <inheritdoc />
        public virtual ICollection<ILogWriterInitializer> Initializers { get; } = new HashSet<ILogWriterInitializer>();

    }

}
