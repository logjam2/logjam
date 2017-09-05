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
    using System.Runtime.Serialization;

    using LogJam.Config;
    using LogJam.Shared.Internal;
    using LogJam.Writer;


    /// <summary>
    /// Holds configuration for a single trace writer.
    /// </summary>
    /// <remarks>
    /// <c>TraceWriterConfig</c> subclasses should generally not override <see cref="object.GetHashCode" /> or
    /// <see cref="object.Equals(object)" />,
    /// because they are identified by reference. It should be valid to have two <c>TraceWriterConfig</c> objects with the
    /// same values stored
    /// in a set or dictionary.
    /// </remarks>
    public sealed class TraceWriterConfig
    {

        private ILogWriterConfig _tracelogWriterConfig;
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
            Arg.NotNull(logWriterConfig, nameof(logWriterConfig));

            _tracelogWriterConfig = logWriterConfig;
            _switches = switches ?? new SwitchSet();
        }

        public TraceWriterConfig(ILogWriter logWriter, SwitchSet switches = null)
            : this(new UseExistingLogWriterConfig(logWriter), switches)
        {
            Arg.NotNull(logWriter, nameof(logWriter));
        }

        [DataMember(Name = "LogWriter")]
        public ILogWriterConfig LogWriterConfig
        {
            get
            {
#if CODECONTRACTS
                System.Diagnostics.Contracts.Contract.Ensures(System.Diagnostics.Contracts.Contract.Result<ILogWriterConfig>() != null);
#endif
                if (_tracelogWriterConfig == null)
                {
                    _tracelogWriterConfig = new NoOpLogWriterConfig();
                }

                return _tracelogWriterConfig;
            }
            set
            {
                Arg.NotNull(value, nameof(value));

                _tracelogWriterConfig = value;
            }
        }

        public SwitchSet Switches => _switches;

    }

}
