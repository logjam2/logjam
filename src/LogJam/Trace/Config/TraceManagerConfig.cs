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
    using System.Linq;

    using LogJam.Config;
    using LogJam.Shared.Internal;
    using LogJam.Trace.Format;
    using LogJam.Trace.Switches;
    using LogJam.Util;
    using LogJam.Writer.Text;


    /// <summary>
    /// Holds the configuration settings for a <see cref="TraceManager" /> instance.
    /// </summary>
    public sealed class TraceManagerConfig : IEquatable<TraceManagerConfig>
    {
        /// <summary>
        /// The default <see cref="TypeNameFunc"/>, which uses CSharp names including generic type parameters.
        /// </summary>
        public static readonly Func<Type, string> DefaultTypeNameFunc = t => TypeExtensions.GetCSharpName(t, false);

        #region Fields

        /// <summary>
        /// Holds the configuration for <see cref="TraceWriter" />s.
        /// </summary>
        private readonly ObservableSet<TraceWriterConfig> _traceWriterConfigs;

        /// <summary>
        /// The Type -> <see cref="Tracer.Name"/> function.
        /// </summary>
        private Func<Type, string> _typeNameFunc = DefaultTypeNameFunc;

        #endregion

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
            Arg.NotNull(logManagerConfig, nameof(logManagerConfig));

            LogManagerConfig = logManagerConfig;

            _traceWriterConfigs = new ObservableSet<TraceWriterConfig>();
            _traceWriterConfigs.AddingItem += OnAddingTraceWriterConfig;
            _traceWriterConfigs.RemovingItem += OnRemovingTraceWriterConfig;

            var observableLogWriterConfigSet = (ObservableSet<ILogWriterConfig>) logManagerConfig.Writers;
            observableLogWriterConfigSet.RemovingItem += OnRemovingLogWriterConfig;
        }

        public TraceManagerConfig(TraceWriterConfig traceWriterConfig)
            : this(new LogManagerConfig())
        {
            Arg.NotNull(traceWriterConfig, nameof(traceWriterConfig));

            _traceWriterConfigs.Add(traceWriterConfig);
        }

        public TraceManagerConfig(params TraceWriterConfig[] traceWriterConfigs)
            : this(new LogManagerConfig())
        {
            Arg.NotNullOrEmpty(traceWriterConfigs, nameof(traceWriterConfigs));
            Arg.NoneNull(traceWriterConfigs, nameof(traceWriterConfigs));

            _traceWriterConfigs.UnionWith(traceWriterConfigs);
        }

        /// <summary>
        /// Default configuration for tracing writes to the debugger when the debugger is attached.
        /// </summary>
        internal void SetToDefaultConfiguration()
        {
            _traceWriterConfigs.Clear();
            if (DebuggerFormatWriter.IsDebuggerActive())
            {
                _traceWriterConfigs.Add(CreateDebugTraceWriterConfig());
            }
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
        public ISet<TraceWriterConfig> Writers => _traceWriterConfigs;

        /// <summary>
        /// The Type -> <see cref="Tracer.Name"/> function used by <see cref="ITracerFactory.GetTracer(Type)"/>
        /// </summary>
        /// <remarks>
        /// To reset to the default value, set to <see cref="TraceManagerConfig.DefaultTypeNameFunc"/>.
        /// </remarks>
        public Func<Type, string> TypeNameFunc
        {
            get { return _typeNameFunc; }
            set
            {
                Arg.NotNull(value, nameof(value));

                _typeNameFunc = value;
            }
        }

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
