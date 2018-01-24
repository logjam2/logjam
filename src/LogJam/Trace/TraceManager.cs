// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManager.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using LogJam.Config;
    using LogJam.Internal;
    using LogJam.Trace.Config;
    using LogJam.Trace.Switches;
    using LogJam.Writer;


    /// <summary>
    /// Entry point for everything related to tracing.
    /// </summary>
    public sealed class TraceManager : BaseLogJamManager, ITracerFactory
    {
        #region Static fields

        private static readonly Lazy<TraceManager> s_instance;

        #endregion

        #region Instance fields

        private readonly TraceManagerConfig _traceConfig;

        private readonly Dictionary<string, WeakReference> _tracers = new Dictionary<string, WeakReference>(100);

        private readonly List<Tuple<TraceWriterConfig, IEntryWriter<TraceEntry>>> _activeTraceEntryWriters = new List<Tuple<TraceWriterConfig, IEntryWriter<TraceEntry>>>();

        /// <summary>
        /// TraceManager uses a <see cref="LogManager" /> to manage the <see cref="ILogWriter" />s that <see cref="TraceEntry" />s
        /// are written to.
        /// </summary>
        private readonly LogManager _logManager;

        #endregion

        static TraceManager()
        {
            s_instance = new Lazy<TraceManager>(() => new TraceManager(LogManager.Instance, TraceManagerConfig.Default(LogManager.Instance.Config)));
        }

        /// <summary>
        /// Returns an AppDomain-global <see cref="TraceManager" />.
        /// </summary>
        public static TraceManager Instance { get { return s_instance.Value; } }

        #region Constructors and Destructors

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> instance using default configuration.
        /// </summary>
        public TraceManager()
            : this(TraceManagerConfig.Default(new LogManagerConfig()))
        {
            // TODO: Check for local or remote config?
        }

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> instance using default configuration and the specified
        /// <paramref name="setupLog" />.
        /// </summary>
        /// <param name="setupLog"></param>
        public TraceManager(SetupLog setupLog)
            : this(TraceManagerConfig.Default(new LogManagerConfig()), setupLog)
        {
            // TODO: Check for local or remote config?
        }

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> configured to use <paramref name="logWriterConfig" /> and
        /// <paramref name="traceSwitch" /> for all <see cref="Tracer" />s.
        /// </summary>
        /// <param name="logWriterConfig">The <see cref="ILogWriterConfig" /> to use to configure tracing.</param>
        /// <param name="traceSwitch">
        /// A <see cref="ITraceSwitch" /> to use for all <see cref="Tracer" />s. If
        /// <c>null</c>, all <see cref="Tracer" /> calls of severity <see cref="TraceLevel.Info" /> or higher are written.
        /// </param>
        /// <param name="tracerNamePrefix">
        /// The <see cref="Tracer.Name" /> prefix to use. Tracing will not occur if the
        /// <c>Tracer.Name</c> doesn't match this prefix. By default, <see cref="Tracer.All" /> is used.
        /// </param>
        public TraceManager(ILogWriterConfig logWriterConfig, ITraceSwitch traceSwitch = null, string tracerNamePrefix = Tracer.All)
        {
            Contract.Requires<ArgumentNullException>(logWriterConfig != null);

            if (traceSwitch == null)
            {
                traceSwitch = TraceManagerConfig.CreateDefaultTraceSwitch();
            }

            // REVIEW: The need for this is messy, and we miss cases (eg multiple existing logwriters used) - perhaps each component manages its own messages? 
            // If there's an existing LogWriter in the logwriter config, use its SetupLog, if any.
            ITracerFactory setupTracerFactory = null;
            var useExistingLogWriterConfig = logWriterConfig as UseExistingLogWriterConfig;
            if (useExistingLogWriterConfig != null)
            {
                var component = useExistingLogWriterConfig.LogWriter as ILogJamComponent;
                if (component != null)
                {
                    setupTracerFactory = component.SetupTracerFactory;
                }
            }

            _logManager = new LogManager(new LogManagerConfig(), setupTracerFactory);
            LinkDispose(_logManager); // b/c the LogManager is owned by this
            _traceConfig = new TraceManagerConfig(_logManager.Config)
                           {
                               Writers =
                               {
                                   new TraceWriterConfig(logWriterConfig)
                                   {
                                       Switches =
                                       {
                                           { tracerNamePrefix, traceSwitch }
                                       }
                                   }
                               }
                           };

            // Ensure trace formatting is enabled for logWriterConfig, if it's a text writer.
            var textLogWriterConfig = logWriterConfig as TextLogWriterConfig;
            if ((textLogWriterConfig != null)
                && ! textLogWriterConfig.HasFormatterFor<TraceEntry>())
            {
                textLogWriterConfig.Format<TraceEntry>();
            }
        }

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> configured to use <paramref name="logWriter" /> and
        /// <paramref name="traceSwitch" /> for all <see cref="Tracer" />s.
        /// </summary>
        /// <param name="logWriter">The <see cref="IEntryWriter{TEntry}" /> to use.</param>
        /// <param name="traceSwitch">
        /// A <see cref="ITraceSwitch" /> to use for all <see cref="Tracer" />s. If
        /// <c>null</c>, all <see cref="Tracer" /> calls of severity <see cref="TraceLevel.Info" /> or higher are written.
        /// </param>
        /// <param name="tracerNamePrefix">
        /// The <see cref="Tracer.Name" /> prefix to use. Tracing will not occur if the
        /// <c>Tracer.Name</c> doesn't match this prefix. By default, <see cref="Tracer.All" /> is used.
        /// </param>
        public TraceManager(ILogWriter logWriter, ITraceSwitch traceSwitch = null, string tracerNamePrefix = Tracer.All)
            : this(new UseExistingLogWriterConfig(logWriter), traceSwitch, tracerNamePrefix)
        {
            Contract.Requires<ArgumentNullException>(logWriter != null);
        }

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> configured to use <paramref name="logWriter" /> and
        /// <paramref name="switches" /> for all <see cref="Tracer" />s.
        /// </summary>
        /// <param name="logWriter">The <see cref="IEntryWriter{TEntry}" /> to use.</param>
        /// <param name="switches">Defines the trace switches to use with <paramref name="logWriter" />.</param>
        public TraceManager(ILogWriter logWriter, SwitchSet switches)
            : this(new TraceWriterConfig(logWriter, switches))
        {
            Contract.Requires<ArgumentNullException>(logWriter != null);
            Contract.Requires<ArgumentNullException>(switches != null);
        }

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> configured to use <paramref name="logWriter" /> and trace
        /// everything that meets or exceeds <paramref name="traceThreshold" />.
        /// </summary>
        /// <param name="logWriter">The <see cref="IEntryWriter{TEntry}" /> to use.</param>
        /// <param name="traceThreshold">The minimum <see cref="TraceLevel" /> that will be logged.</param>
        /// <param name="tracerNamePrefix">
        /// The <see cref="Tracer.Name" /> prefix to use. Tracing will not occur if the
        /// <c>Tracer.Name</c> doesn't match this prefix. By default, <see cref="Tracer.All" /> is used.
        /// </param>
        /// <param name="setupTracerFactory">
        /// The <see cref="SetupTracerFactory" /> to use for tracing setup operations. This should be the
        /// same <see cref="SetupTracerFactory" /> used to initialize <paramref name="logWriter" />.
        /// </param>
        public TraceManager(ILogWriter logWriter, TraceLevel traceThreshold, string tracerNamePrefix = Tracer.All)
            : this(new UseExistingLogWriterConfig(logWriter), new ThresholdTraceSwitch(traceThreshold))
        {
            Contract.Requires<ArgumentNullException>(logWriter != null);
        }

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> instance using the specified <paramref name="traceWriterConfig" />.
        /// </summary>
        /// <param name="traceWriterConfig">The <see cref="TraceWriterConfig" /> to use for this <c>TraceManager</c>.</param>
        /// <param name="setupLog">The <see cref="SetupTracerFactory" /> to use for tracing setup operations.</param>
        public TraceManager(TraceWriterConfig traceWriterConfig, SetupLog setupLog = null)
            : this(new TraceManagerConfig(traceWriterConfig), setupLog)
        {
            Contract.Requires<ArgumentNullException>(traceWriterConfig != null);
        }

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> instance using the specified <paramref name="logManager"/> and <paramref name="traceWriterConfig" />.
        /// </summary>
        /// <param name="logManager"></param>
        /// <param name="traceWriterConfigs">A set of 1 or more <see cref="TraceWriterConfig" />s to use for this <c>TraceManager</c>.</param>
        public TraceManager(LogManager logManager, params TraceWriterConfig[] traceWriterConfigs)
            : this(logManager)
        {
            Contract.Requires<ArgumentNullException>(logManager != null);
            Contract.Requires<ArgumentNullException>(traceWriterConfigs != null);

            Config.Writers.UnionWith(traceWriterConfigs);
        }

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> instance using the specified <paramref name="configuration" />.
        /// </summary>
        /// <param name="configuration">The <see cref="TraceManagerConfig" /> to use to configure this <c>TraceManager</c>.</param>
        /// <param name="setupLog">The <see cref="SetupTracerFactory" /> to use for tracing setup operations.</param>
        public TraceManager(TraceManagerConfig configuration, SetupLog setupLog = null)
            : this(new LogManager(configuration.LogManagerConfig, setupLog), configuration)
        {
            Contract.Requires<ArgumentNullException>(configuration != null);

            // This TraceManager owns the LogManager, so dispose it on this.Dispose()
            LinkDispose(_logManager);
        }

        /// <summary>
        /// Creates a new <see cref="TraceManager" /> instance using the specified <paramref name="logManager" /> and
        /// <paramref name="configuration" />.
        /// </summary>
        /// <param name="logManager">The <see cref="LogManager" /> associated with this <see cref="TraceManager" />.</param>
        /// <param name="configuration">The <see cref="TraceManagerConfig" /> to use to configure this <c>TraceManager</c>.</param>
        private TraceManager(LogManager logManager, TraceManagerConfig configuration = null)
        {
            Contract.Requires<ArgumentNullException>(logManager != null);
            Contract.Requires<ArgumentException>(configuration == null || ReferenceEquals(logManager.Config, configuration.LogManagerConfig));

            if (configuration == null)
            {
                configuration = new TraceManagerConfig(logManager.Config);
            }

            _logManager = logManager;
            _traceConfig = configuration;
        }

        protected override void InternalStart()
        {
            lock (this)
            {
                if (_logManager.IsRestartNeeded())
                {
                    // Restart the LogManager to pick up any config changes
                    _logManager.Start();
                }
                else
                {
                    // No config changes needed, just start it if not already started
                    _logManager.EnsureAutoStarted();
                }

                // Create all the TraceWriters associated with each config entry
                _activeTraceEntryWriters.Clear();
                foreach (TraceWriterConfig traceWriterConfig in Config.Writers)
                {
                    try
                    {
                        IEntryWriter<TraceEntry> traceEntryWriter = LogManager.GetEntryWriter<TraceEntry>(traceWriterConfig.LogWriterConfig);
                        _activeTraceEntryWriters.Add(new Tuple<TraceWriterConfig, IEntryWriter<TraceEntry>>(traceWriterConfig, traceEntryWriter));
                    }
                    catch (Exception excp)
                    {
                        SetupTracerFactory.TracerFor(this)
                                          .Error(excp, "Couldn't setup Tracing to target {0} due to exception getting IEntryWriter<TraceEntry>.", traceWriterConfig.LogWriterConfig);
                    }
                }

                // Reset TraceWriter for each Tracer
                ForEachTracer(tracer => tracer.Configure(GetTraceWritersFor(tracer.Name)));
            }

        }

        protected override void InternalStop()
        {
            lock (this)
            {
                _activeTraceEntryWriters.Clear();

                // Set all Tracers to write to a NoOpTraceWriter
                var noopTraceWriter = new NoOpTraceWriter();
                ForEachTracer(tracer => tracer.Writer = noopTraceWriter);
            }
        }

        protected override void InternalReset()
        {
            // Stop has already been called
            _traceConfig.SetToDefaultConfiguration();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the <see cref="TraceManagerConfig" /> used to configure this <see cref="TraceManager" />.
        /// </summary>
        public TraceManagerConfig Config { get { return _traceConfig; } }

        /// <summary>
        /// Returns the <see cref="LogManager" /> associated with this <see cref="TraceManager" />.
        /// </summary>
        public LogManager LogManager { get { return _logManager; } }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets or creates a <see cref="Tracer" /> using the specified <paramref name="name" />.
        /// </summary>
        /// <param name="name">The <see cref="Tracer.Name" /> to match or create.</param>
        /// <returns>
        /// The <see cref="Tracer" />.
        /// </returns>
        public Tracer GetTracer(string name)
        {
            EnsureAutoStarted();

            if (name == null)
            {
                name = string.Empty;
            }

            name = name.Trim();

            // Lookup the Tracer, or add a new one
            WeakReference weakRefTracer;
            lock (this)
            {
                if (_tracers.TryGetValue(name, out weakRefTracer))
                {
                    object objTracer = weakRefTracer.Target;
                    if (objTracer == null)
                    {
                        _tracers.Remove(name);
                    }
                    else
                    {
                        // Return the existing Tracer
                        return (Tracer) objTracer;
                    }
                }

                // Create a new Tracer and register it
                Tracer tracer = new Tracer(name, GetTraceWritersFor(name));
                _tracers[name] = new WeakReference(tracer);
                return tracer;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles performing an action on each registered <see cref="Tracer" />, and cleaning up any that have
        /// been GCed.
        /// </summary>
        /// <param name="action"></param>
        private void ForEachTracer(Action<Tracer> action)
        {
            lock (this)
            {
                List<string> keysToRemove = new List<string>();
                foreach (var kvp in _tracers)
                {
                    WeakReference weakRefTracer = kvp.Value;
                    object objTracer = weakRefTracer.Target;
                    Tracer tracer = objTracer as Tracer;
                    if (tracer == null)
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                    else
                    {
                        action(tracer);
                    }
                }

                foreach (var keyToRemove in keysToRemove)
                {
                    _tracers.Remove(keyToRemove);
                }
            }
        }

        private TraceWriter[] GetTraceWritersFor(string tracerName)
        {
            // REVIEW: We could cache the TraceWriters so that the same instances are returned when the switch + logwriter instances are the same
            var traceWriters = new List<TraceWriter>();
            foreach (var traceWriterTuple in _activeTraceEntryWriters)
            {
                ITraceSwitch traceSwitch;
                if (traceWriterTuple.Item1.Switches.FindBestMatchingSwitch(tracerName, out traceSwitch))
                {
                    traceWriters.Add(new TraceWriter(traceSwitch, traceWriterTuple.Item2, SetupTracerFactory));
                }
            }
            return traceWriters.ToArray();
        }

        //private void OnTracerConfigAddedOrRemoved(object sender, ConfigChangedEventArgs<TracerConfig> e)
        //{
        //	// Re-configure each existing Tracer that prefix matches the TracerConfig being added or removed
        //	foreach (Tracer tracer in FindMatchingTracersFor(e.ConfigChanged))
        //	{
        //		// REVIEW: It's probably best if TracerConfig TraceSwitch and traceLogWriter are always populated
        //		tracer.Configure(e.ConfigChanged.TraceWriters);
        //	}
        //}

        #endregion

        #region BaseLogJamManager overrides

        public override ITracerFactory SetupTracerFactory { get { return LogManager.SetupTracerFactory; } }

        public override IEnumerable<TraceEntry> SetupLog { get { return LogManager.SetupLog; } }

        #endregion
    }
}
