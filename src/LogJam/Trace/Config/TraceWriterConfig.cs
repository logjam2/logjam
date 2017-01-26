// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    using LogJam.Config;
    using LogJam.Writer;


    /// <summary>
    /// Holds configuration for a single trace writer.
    /// </summary>
    public sealed class TraceWriterConfig
    {

        private ILogWriterConfig _logWriterConfig;
        private readonly SwitchSet _switches;

        /// <summary>
        /// Creates a new <see cref="TraceWriterConfig" /> using all default values.
        /// If <see cref="LogWriterConfig" /> is not subsequently set, a <see cref="NoOpEntryWriter{TEntry}" /> will be
        /// used.
        /// </summary>
        public TraceWriterConfig()
        {
            _switches = new SwitchSet();
        }

        public TraceWriterConfig(ILogWriterConfig logWriterConfig, SwitchSet switches = null)
        {
            Contract.Requires<ArgumentNullException>(logWriterConfig != null);

            _logWriterConfig = logWriterConfig;
            _switches = switches ?? new SwitchSet();
        }

        public TraceWriterConfig(ILogWriter logWriter, SwitchSet switches = null)
            : this(new UseExistingLogWriterConfig(logWriter), switches)
        {
            Contract.Requires<ArgumentNullException>(logWriter != null);
        }

        [DataMember(Name = "LogWriter")]
        public ILogWriterConfig LogWriterConfig
        {
            get
            {
                Contract.Ensures(Contract.Result<ILogWriterConfig>() != null);
                if (_logWriterConfig == null)
                {
                    _logWriterConfig = new NoOpLogWriterConfig();
                }

                return _logWriterConfig;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                if (_logWriterConfig != null)
                {
                    throw new LogJamSetupException("LogWriterConfig property cannot be set more than once", this);
                }

                _logWriterConfig = value;
            }
        }

        public SwitchSet Switches { get { return _switches; } }

    }

}
