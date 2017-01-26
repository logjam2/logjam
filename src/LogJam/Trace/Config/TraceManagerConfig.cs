// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
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
    using LogJam.Trace.Config.Initializer;
    using LogJam.Trace.Format;
    using LogJam.Trace.Switches;
    using LogJam.Util;
    using LogJam.Writer.Text;


    /// <summary>
    /// Holds the configuration settings for a <see cref="TraceManager" /> instance.
    /// </summary>
    public sealed class TraceManagerConfig : IEquatable<TraceManagerConfig>
    {
        #region Fields

        /// <summary>
        /// Holds the configuration for <see cref="TraceWriter" />s.
        /// </summary>
        private readonly ObservableSet<TraceWriterConfig> _traceWriterConfigs;

        /// <summary>
        /// Holds initializers that are applied during startup.
        /// </summary>
        private readonly List<ITraceInitializer> _initializers;

        #endregion

        public static readonly IEnumerable<ITraceInitializer> DefaultInitializers = new ITraceInitializer[]
                                                                                    {
                                                                                        new FirstChanceExceptionTracerInitializer()
                                                                                    };

        /// <summary>
        /// Creates and returns a new <see cref="TraceManagerConfig" /> with default trace configuration,
        /// which means if a debugger is attached, trace output is logged to the debugger.
        /// </summary>
        /// <returns></returns>
        public static TraceManagerConfig Default(LogManagerConfig logManagerConfig)
        {
            var traceManagerConfig = new TraceManagerConfig(logManagerConfig);
            traceManagerConfig.SetToDefaultConfiguration();
            return traceManagerConfig;
        }

        /// <summary>
        /// Empty configuration, no traces written.
        /// </summary>
        public TraceManagerConfig()
            : this(new LogManagerConfig())
        {}

        /// <summary>
        /// Empty trace configuration, connected to <paramref name="logManagerConfig"/>.
        /// </summary>
        public TraceManagerConfig(LogManagerConfig logManagerConfig)
        {
            Contract.Requires<ArgumentNullException>(logManagerConfig != null);

            LogManagerConfig = logManagerConfig;

            _initializers = new List<ITraceInitializer>(DefaultInitializers);

            _traceWriterConfigs = new ObservableSet<TraceWriterConfig>();
            _traceWriterConfigs.AddingItem += OnAddingTraceWriterConfig;
            _traceWriterConfigs.RemovingItem += OnRemovingTraceWriterConfig;

            var observableLogWriterConfigSet = (ObservableSet<ILogWriterConfig>) logManagerConfig.Writers;
            observableLogWriterConfigSet.RemovingItem += OnRemovingLogWriterConfig;
        }

        public TraceManagerConfig(TraceWriterConfig traceWriterConfig)
            : this(new LogManagerConfig())
        {
            Contract.Requires<ArgumentNullException>(traceWriterConfig != null);

            _traceWriterConfigs.Add(traceWriterConfig);
        }

        public TraceManagerConfig(params TraceWriterConfig[] traceWriterConfigs)
            : this(new LogManagerConfig())
        {
            Contract.Requires<ArgumentNullException>(traceWriterConfigs != null);
            Contract.Requires<ArgumentException>(traceWriterConfigs.Length > 0);
            Contract.Requires<ArgumentNullException>(traceWriterConfigs.All(config => config != null));

            _traceWriterConfigs.UnionWith(traceWriterConfigs);
        }

        /// <summary>
        /// Default configuration for tracing writes to the debugger when the debugger is attached.
        /// </summary>
        public void SetToDefaultConfiguration()
        {
            _traceWriterConfigs.Clear();
            if (DebuggerFormatWriter.IsDebuggerActive())
            {
                _traceWriterConfigs.Add(CreateDebugTraceWriterConfig());
            }

            _initializers.Clear();
            _initializers.AddRange(DefaultInitializers);

            TraceFirstChanceExceptions = false;
        }

        /// <summary>
        /// Returns a <see cref="TraceWriterConfig" /> containing default values - trace output is logged to the debugger.
        /// </summary>
        /// <returns></returns>
        public static TraceWriterConfig CreateDebugTraceWriterConfig()
        {
            return new TraceWriterConfig(new DebuggerLogWriterConfig().Format(new DefaultTraceFormatter()), CreateDefaultSwitchSet());
        }

        /// <summary>
        /// Creates a <see cref="SwitchSet"/> containing default values - ie log the <see cref="TraceLevel.Info"/> level and above for all <see cref="Tracer"/> names.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the <see cref="LogManagerConfig"/> object associated with this <see cref="TraceManagerConfig"/> object.
        /// </summary>
        public LogManagerConfig LogManagerConfig { get; }

        /// <summary>
        /// Gets the set of <see cref="TraceWriterConfig" /> objects that the <see cref="TraceManager" /> is configured from.
        /// </summary>
        public ISet<TraceWriterConfig> Writers { get { return _traceWriterConfigs; } }


        /// <summary>
        /// Returns a collection of initializers that are called when the <see cref="TraceManager"/> is started and stopped.
        /// </summary>
        public ICollection<ITraceInitializer> Initializers { get { return _initializers; } }

        /// <summary>
        /// Set to <c>true</c> to Trace first-chance exceptions as warnings. This traces all exceptions as they're thrown
        /// so can be verbose.
        /// </summary>
        public bool TraceFirstChanceExceptions { get; set; }

        private void OnAddingTraceWriterConfig(TraceWriterConfig traceWriterConfig)
        {
            if (traceWriterConfig != null)
            {
                LogManagerConfig.Writers.UnionWith(new ILogWriterConfig[] { traceWriterConfig.LogWriterConfig });
            }
        }

        private void OnRemovingTraceWriterConfig(TraceWriterConfig traceWriterConfig)
        {
            // Don't remove the TraceWriterConfig, b/c we can't be sure that other formatters/encoders/etc don't support it.

            //if (traceWriterConfig != null)
            //{
            //    LogManagerConfig.Writers.Remove(traceWriterConfig.LogWriterConfig);
            //}
        }

        /// <summary>
        /// Event handler called when <paramref name="logWriterConfig"/> is removed from the <c>LogManager.Config.Writers</c> collection.
        /// </summary>
        /// <param name="logWriterConfig"></param>
        private void OnRemovingLogWriterConfig(ILogWriterConfig logWriterConfig)
        {
            if (logWriterConfig != null)
            {
                var removeSet = Writers.Where(twc => twc.LogWriterConfig == logWriterConfig);
                Writers.ExceptWith(removeSet);
            }
        }

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
