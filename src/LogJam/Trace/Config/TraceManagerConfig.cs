// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfig.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using LogJam.Config;
    using LogJam.Trace.Format;
    using LogJam.Trace.Switches;
    using LogJam.Util;


    /// <summary>
    /// Holds the configuration settings for a <see cref="TraceManager" /> instance.
    /// </summary>
    public sealed class TraceManagerConfig : IEquatable<TraceManagerConfig>
    {
        #region Fields

        /// <summary>
        /// Holds the configuration for <see cref="TraceWriter" />s.
        /// </summary>
        private readonly ISet<TraceWriterConfig> _traceWriterConfigs;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Creates and returns a new <see cref="TraceManagerConfig" /> with default trace configuration,
        /// which means trace output is logged to the debugger.
        /// </summary>
        /// <returns></returns>
        public static TraceManagerConfig Default()
        {
            return new TraceManagerConfig(CreateDefaultTraceWriterConfig());
        }

        /// <summary>
        /// Empty configuration, no traces written.
        /// </summary>
        public TraceManagerConfig()
        {
            _traceWriterConfigs = new HashSet<TraceWriterConfig>();
        }

        public TraceManagerConfig(TraceWriterConfig traceWriterConfig)
        {
            Contract.Requires<ArgumentNullException>(traceWriterConfig != null);

            _traceWriterConfigs = new HashSet<TraceWriterConfig>();
            _traceWriterConfigs.Add(traceWriterConfig);
        }

        public TraceManagerConfig(params TraceWriterConfig[] traceWriterConfigs)
        {
            Contract.Requires<ArgumentNullException>(traceWriterConfigs != null);
            Contract.Requires<ArgumentException>(traceWriterConfigs.Length > 0);
            Contract.Requires<ArgumentNullException>(traceWriterConfigs.All(config => config != null));

            _traceWriterConfigs = new HashSet<TraceWriterConfig>(traceWriterConfigs);
        }

        /// <summary>
        /// Returns a <see cref="TraceWriterConfig" /> containing default values - trace output is logged to the debugger.
        /// </summary>
        /// <returns></returns>
        public static TraceWriterConfig CreateDefaultTraceWriterConfig()
        {
            return new TraceWriterConfig(new DebuggerLogWriterConfig().Format(new DefaultTraceFormatter()), CreateDefaultSwitchSet());
        }

        public static SwitchSet CreateDefaultSwitchSet()
        {
            return new SwitchSet()
                   {
                       { Tracer.All, CreateDefaultTraceSwitch() }
                   };
        }

        internal static ITraceSwitch CreateDefaultTraceSwitch()
        {
            return new ThresholdTraceSwitch(TraceLevel.Info);
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The global config changed.
        /// </summary>
        /// <summary>
        /// An event that is raised when a <see cref="TracerConfig" /> instance is added.
        /// <summary>
        /// Gets the set of <see cref="TraceWriterConfig" /> objects that the <see cref="TraceManager" /> is configured from.
        /// </summary>
        public ISet<TraceWriterConfig> Writers { get { return _traceWriterConfigs; } }

        #endregion

        #region Public Methods and Operators

        #endregion

        public bool Equals(TraceManagerConfig other)
        {
            if (other == null)
            {
                return false;
            }

            return _traceWriterConfigs.SetEquals(other._traceWriterConfigs);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TraceManagerConfig);
        }

        public override int GetHashCode()
        {
            return (_traceWriterConfigs != null ? _traceWriterConfigs.GetUnorderedCollectionHashCode() : 0);
        }

        public void Clear()
        {
            _traceWriterConfigs.Clear();
        }

    }
}
