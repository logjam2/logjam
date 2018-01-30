// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManager.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using LogJam.Config;
using LogJam.Config.Initializer;
using LogJam.Internal;
using LogJam.Shared.Internal;
using LogJam.Trace;
using LogJam.Util;
using LogJam.Writer;
using LogJam.Writer.Rotator;

namespace LogJam
{

    /// <summary>
    /// Manager object that manages all <see cref="ILogWriter" />s and their configuration.
    /// </summary>
    public sealed class LogManager : BaseLogJamManager
    {

        #region Static fields

        private static readonly Lazy<LogManager> s_instance = new Lazy<LogManager>(() => new LogManager());

        #endregion

        #region Static properties

        /// <summary>
        /// Returns an AppDomain-global <see cref="LogManager" />.
        /// </summary>
        public static LogManager Instance => s_instance.Value;

        #endregion

        /// <summary>
        /// The <see cref="LogManagerConfig" /> used to configure this <c>LogManager</c>.
        /// </summary>
        public LogManagerConfig Config { get { return _config; } }

        public override ITracerFactory SetupTracerFactory { get { return _setupTracerFactory; } }

        /// <summary>
        /// Returns the collection of <see cref="TraceEntry" />s logged through <see cref="SetupTracerFactory" />.
        /// </summary>
        public override IEnumerable<TraceEntry> SetupLog { get { return _setupTracerFactory as IEnumerable<TraceEntry> ?? Enumerable.Empty<TraceEntry>(); } }

        /// <summary>
        /// Checks whether this <see cref="LogManager" /> should be restarted to pick up any config changes.
        /// </summary>
        /// <returns></returns>
        internal bool IsRestartNeeded()
        {
            // REVIEW: This currently only picks up new log writers. If there are changes to logwriter configuration, the changes are not detected.
            return State == StartableState.Started && ! ((_logWriters.Count == Config.Writers.Count) && Config.Writers.SetEquals(_logWriters.Keys));
        }

        /// <summary>
        /// Adds a <see cref="BackgroundMultiLogWriter" /> so a reference is maintained (to keep it alive), and so
        /// it can be stopped when the <c>LogManager</c> is stopped.
        /// </summary>
        /// <param name="backgroundMultiLogWriter"></param>
        internal void AddBackgroundMultiLogWriter(BackgroundMultiLogWriter backgroundMultiLogWriter)
        {
            Arg.NotNull(backgroundMultiLogWriter, nameof(backgroundMultiLogWriter));
            _backgroundMultiLogWriters.Add(backgroundMultiLogWriter);
        }

        protected override void InternalStart()
        {
            // NOTE: LogManager is locked when InternalStart() is called.

            var logManagerTracer = SetupTracerFactory.TracerFor(this);

            if (_logWriters.Count > 0)
            {
                logManagerTracer.Debug("Stopping LogManager before re-starting it...");
                Stop();
            }

            // Apply all ILogManagerConfigInitializers
            foreach (var logManagerConfigInitializer in Config.Initializers.OfType<ILogManagerConfigInitializer>())
            {
                logManagerConfigInitializer.UpdateLogManagerConfig(Config);
            }

            // Create LogWriters from config
            foreach (ILogWriterConfig logWriterConfig in Config.Writers)
            {
                if (_logWriters.ContainsKey(logWriterConfig))
                {
                    // Occurs when the logWriterConfig already exists - this shouldn't happen
                    var tracer = SetupTracerFactory.TracerFor(logWriterConfig);
                    tracer.Severe("LogWriterConfig {0} is already active - this shouldn't happen. Skipping it...", logWriterConfig);
                    continue;
                }

                ILogWriter logWriter = CreateLogWriter(logWriterConfig, SetupTracerFactory, this);

                if (logWriter != null)
                {
                    (logWriter as IStartable).SafeStart(SetupTracerFactory);

                    _logWriters.Add(logWriterConfig, logWriter);

                    if (logWriterConfig.DisposeOnStop)
                    {
                        DisposeOnStop(logWriter);
                    }
                }

            }

            _cachedEntryWriters.Clear();

            // Start any BackgroundMultiLogWriters, if created during initialization
            _backgroundMultiLogWriters.SafeStart(SetupTracerFactory);
        }

        /// <summary>
        /// Creates a log writer and builds a pipeline to it based on the specified configuration.
        /// </summary>
        /// <param name="logWriterConfig"></param>
        /// <param name="setupLog"></param>
        /// <param name="logManager"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method is static so it may be used without a <paramref name="logManager" />, eg within <see cref="RotatingLogFileWriter" />
        /// to create inner log writers.
        /// </remarks>
        internal static ILogWriter CreateLogWriter(ILogWriterConfig logWriterConfig, ITracerFactory setupLog, LogManager logManager = null)
        {
            Arg.NotNull(logWriterConfig, nameof(logWriterConfig));

            ILogWriter logWriter = null;

            // A lightweight service locator scoped to a single LogWriter pipeline
            DependencyDictionary logWriterDependencyDictionary = new DependencyDictionary();
            if (logManager != null)
            {
                logWriterDependencyDictionary.Add<LogManager>(logManager);
            }

            if (setupLog != null)
            {
                logWriterDependencyDictionary.Add<ITracerFactory>(setupLog);
            }

            logWriterDependencyDictionary.Add<ILogWriterConfig>(logWriterConfig);
            logWriterDependencyDictionary.Add(logWriterConfig.GetType(), logWriterConfig);

            try
            {
                // Combine the initializers - LogWriterConfig.Initializers comes first
                var initializers = new List<ILogWriterInitializer>(logWriterConfig.Initializers);
                if (logManager != null)
                {
                    initializers.AddRange(logManager.Config.Initializers.OfType<ILogWriterInitializer>());
                }

                ILogWriterPipelineBuilder pipelineBuilder = initializers.OfType<ILogWriterPipelineBuilder>().FirstOrDefault() ?? new DefaultLogWriterPipelineBuilder();

                logWriter = pipelineBuilder.CreateLogWriterPipeline(logWriterConfig, initializers, logWriterDependencyDictionary, setupLog);
            }
            catch (Exception excp)
            {
                // TODO: Store initialization failure status
                var tracer = setupLog.TracerFor(logWriterConfig);
                tracer.Severe(excp, "Exception creating logwriter from config: {0}", logWriterConfig);
                logWriter = null;
            }

            return logWriter;
        }

        /// <summary>
        /// Stops all log writers managed by this <see cref="LogManager" />.
        /// </summary>
        protected override void InternalStop()
        {
            lock (this)
            {
                _logWriters.Values.SafeStop(SetupTracerFactory);
                _logWriters.Clear();
                _cachedEntryWriters.Clear();
                _backgroundMultiLogWriters.SafeStop(SetupTracerFactory);
                _backgroundMultiLogWriters.SafeDispose(SetupTracerFactory);
                _backgroundMultiLogWriters.Clear();
            }
        }

        protected override void InternalReset()
        {
            // Stop has already been called
            _config.Reset();
        }

        /// <summary>
        /// Returns all successfully started <see cref="IEntryWriter{TEntry}" />s associated with this <c>LogManager</c>.
        /// </summary>
        /// <typeparam name="TEntry">The logentry type written by the returned <see cref="IEntryWriter{TEntry}" />s.</typeparam>
        /// <returns>
        /// All successfully started <see cref="IEntryWriter{TEntry}" />s that are type-compatible with
        /// <typeparamref name="TEntry" />. May return an empty enumerable.
        /// </returns>
        /// <remarks>Even if Start() wasn't 100% successful, we still return any logwriters that were successfully started.</remarks>
        public IEnumerable<IEntryWriter<TEntry>> GetEntryWriters<TEntry>()
            where TEntry : ILogEntry
        {
            // Even if Start() wasn't 100% successful, we still return any logwriters that are available.
            EnsureAutoStarted();

            // If the config has changed since starting, restart
            if (IsRestartNeeded())
            {
                Start();
            }

            lock (this)
            {
                var listLogWriters = new List<IEntryWriter<TEntry>>();
                _logWriters.Values.GetEntryWriters(listLogWriters);
                return listLogWriters;
            }
        }

        /// <summary>
        /// Returns a single <see cref="IEntryWriter{TEntry}" /> that writes to all successfully started
        /// <see cref="IEntryWriter{TEntry}" />s associated with this <c>LogManager</c>.
        /// </summary>
        /// <typeparam name="TEntry">The logentry type written by the returned <see cref="IEntryWriter{TEntry}" />s.</typeparam>
        /// <returns>
        /// A single <see cref="IEntryWriter{TEntry}" /> that writes to all successfully started
        /// <see cref="IEntryWriter{TEntry}" />s that are type-compatible with <typeparamref name="TEntry" />.
        /// </returns>
        public bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter)
            where TEntry : ILogEntry
        {
            if (_cachedEntryWriters.TryGetValue(typeof(TEntry), out object entryWriterObject))
            {
                // Cache hit
                entryWriter = (IEntryWriter<TEntry>) entryWriterObject;
            }
            else
            {
                // Cache miss
                IEntryWriter<TEntry>[] entryWriters = GetEntryWriters<TEntry>().ToArray();
                if (entryWriters.Length == 1)
                {
                    entryWriter = entryWriters[0];
                }
                else if (entryWriters.Length == 0)
                {
                    entryWriter = null;
                }
                else
                {
                    entryWriter = new FanOutEntryWriter<TEntry>(entryWriters);
                }

                _cachedEntryWriters.TryAdd(typeof(TEntry), entryWriter);
            }

            return entryWriter != null;
        }

        /// <summary>
        /// Returns a single <see cref="IEntryWriter{TEntry}" /> that writes to all successfully started
        /// <see cref="IEntryWriter{TEntry}" />s associated with this <c>LogManager</c>.
        /// </summary>
        /// <typeparam name="TEntry">The logentry type written by the returned <see cref="IEntryWriter{TEntry}" />s.</typeparam>
        /// <returns>
        /// A single <see cref="IEntryWriter{TEntry}" /> that writes to all successfully started
        /// <see cref="IEntryWriter{TEntry}" />s that are type-compatible with <typeparamref name="TEntry" />.
        /// </returns>
        public IEntryWriter<TEntry> GetEntryWriter<TEntry>()
            where TEntry : ILogEntry
        {
            TryGetEntryWriter(out IEntryWriter<TEntry> entryWriter);
            return entryWriter ?? new NoOpEntryWriter<TEntry>();
        }

        /// <summary>
        /// Returns the <see cref="ILogWriter" /> created from the specified <paramref name="logWriterConfig" />. If this
        /// <c>LogManager</c> has not yet been started, it is started first.
        /// </summary>
        /// <param name="logWriterConfig"></param>
        /// <param name="logWriter">
        /// Returns the <see cref="ILogWriter" /> created from <paramref name="logWriterConfig" /> if one exists; otherwise
        /// <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if a <paramref name="logWriter" /> was found; <c>false</c> if no match was found.</returns>
        public bool TryGetLogWriter(ILogWriterConfig logWriterConfig, out ILogWriter logWriter)
        {
            Arg.NotNull(logWriterConfig, nameof(logWriterConfig));

            // Even if Start() wasn't 100% successful, we still return any logwriters that were successfully started.
            EnsureAutoStarted();

            lock (this)
            {
                return _logWriters.TryGetValue(logWriterConfig, out logWriter);
            }
        }

        /// <summary>
        /// Returns the <see cref="ILogWriter" /> created from the specified <paramref name="logWriterConfig" />.
        /// </summary>
        /// <param name="logWriterConfig"></param>
        /// <returns>The <see cref="ILogWriter" /> created from <paramref name="logWriterConfig" /> if one exists; otherwise null.</returns>
        /// <exception cref="KeyNotFoundException">
        /// If no value in <c>Config.Writers</c> is equal to
        /// <paramref name="logWriterConfig" />
        /// </exception>
        public ILogWriter GetLogWriter(ILogWriterConfig logWriterConfig)
        {
            Arg.NotNull(logWriterConfig, nameof(logWriterConfig));

            if (! TryGetLogWriter(logWriterConfig, out var logWriter))
            {
                throw new KeyNotFoundException("LogManager does not contain logWriterConfig: " + logWriterConfig);
            }

            return logWriter;
        }

        /// <summary>
        /// Returns the <see cref="IEntryWriter{TEntry}" /> matching with the specified <see cref="ILogWriterConfig" />.
        /// </summary>
        /// <typeparam name="TEntry">The logentry type written by the returned <see cref="IEntryWriter{TEntry}" />.</typeparam>
        /// <param name="logWriterConfig">An <see cref="ILogWriterConfig" /> instance.</param>
        /// <returns>
        /// An <see cref="IEntryWriter{TEntry}" /> for <paramref name="logWriterConfig" /> and of entry type
        /// <typeparamref name="TEntry" />.
        /// If the <c>logWriterConfig</c> is valid, but the log writer failed to start or doesn't contain an entry writer of the
        /// specified type,
        /// a <see cref="NoOpEntryWriter{TEntry}" /> is returned.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// If no value in <c>Config.Writers</c> is equal to
        /// <paramref name="logWriterConfig" />
        /// </exception>
        /// <remarks>
        /// This method throws exceptions if the call is invalid, but
        /// does not throw an exception if the returned logwriter failed to start.
        /// </remarks>
        public IEntryWriter<TEntry> GetEntryWriter<TEntry>(ILogWriterConfig logWriterConfig)
            where TEntry : ILogEntry
        {
            Arg.NotNull(logWriterConfig, nameof(logWriterConfig));

            ILogWriter logWriter = null;
            logWriter = GetLogWriter(logWriterConfig);
            if (logWriter == null)
            { // This occurs when entryWriter.Start() fails. In this case, the desired behavior is to return a functioning logwriter.
                var tracer = SetupTracerFactory.TracerFor(this);
                tracer.Warn("Returning a NoOpEntryWriter<{0}> for log writer config: {1} - check start errors.", typeof(TEntry).Name, logWriterConfig);
                return new NoOpEntryWriter<TEntry>();
            }

            if (! logWriter.TryGetEntryWriter(out IEntryWriter<TEntry> entryWriter))
            {
                var tracer = SetupTracerFactory.TracerFor(this);
                tracer.Warn("Returning a NoOpEntryWriter<{0}> for log writer {1} - log writer did not contain an entry writer for log entry type {0}.",
                            typeof(TEntry).Name,
                            logWriterConfig);
                return new NoOpEntryWriter<TEntry>();
            }

            return entryWriter;
        }

        #region Instance fields

        /// <summary>
        /// An internal <see cref="ITracerFactory" />, which is used only for tracing LogJam operations.
        /// </summary>
        private readonly ITracerFactory _setupTracerFactory;

        private readonly LogManagerConfig _config;

        private readonly List<BackgroundMultiLogWriter> _backgroundMultiLogWriters;

        private readonly Dictionary<ILogWriterConfig, ILogWriter> _logWriters;

        // Cached set of TEntry => IEntryWriter<TEntry> | null ; Cleared when log manager is started or stopped.
        private readonly ConcurrentDictionary<Type, object> _cachedEntryWriters;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Creates a new <see cref="LogManager" /> instance using an empty initial configuration.
        /// </summary>
        public LogManager()
            : this(new LogManagerConfig())
        { }

        /// <summary>
        /// Creates a new <see cref="LogManager" /> instance using the specified <paramref name="logManagerConfig" /> for
        /// configuration, and an optional <paramref name="setupTracerFactory" /> for logging LogJam setup and internal operations.
        /// </summary>
        /// <param name="logManagerConfig">
        /// The <see cref="LogManagerConfig" /> that describes how this <see cref="LogManager" />
        /// should be setup.
        /// </param>
        /// <param name="setupTracerFactory">The <see cref="ITracerFactory" /> to use for tracking internal operations.</param>
        public LogManager(LogManagerConfig logManagerConfig, ITracerFactory setupTracerFactory = null)
        {
            Arg.NotNull(logManagerConfig, nameof(logManagerConfig));

            _setupTracerFactory = setupTracerFactory ?? new SetupLog();
            _config = logManagerConfig;
            _backgroundMultiLogWriters = new List<BackgroundMultiLogWriter>();
            _logWriters = new Dictionary<ILogWriterConfig, ILogWriter>();
            _cachedEntryWriters = new ConcurrentDictionary<Type, object>();
        }

        /// <summary>
        /// Creates a new <see cref="LogManager" /> instance using the specified <paramref name="logWriterConfigs" /> to configure
        /// logging.
        /// </summary>
        /// <param name="logWriterConfigs">
        /// A set of 1 or more <see cref="ILogWriterConfig" /> instances to use to configure log
        /// writers.
        /// </param>
        public LogManager(params ILogWriterConfig[] logWriterConfigs)
            : this(new LogManagerConfig(logWriterConfigs))
        { }

        /// <summary>
        /// Creates a new <see cref="LogManager" /> instance using the specified <paramref name="logWriters" />.
        /// </summary>
        /// <param name="logWriters">
        /// A set of 1 or more <see cref="ILogWriter" /> instances to include in the
        /// <see cref="LogManager" />.
        /// </param>
        public LogManager(params ILogWriter[] logWriters)
            : this(logWriters.Select(logWriter => (ILogWriterConfig) new UseExistingLogWriterConfig(logWriter)).ToArray())
        {
            Arg.NoneNull(logWriters, nameof(logWriters));

            // Get the first SetupLog from the passed in logwriters
            var setupTracerFactory = GetSetupTracerFactoryForComponents(logWriters.OfType<ILogJamComponent>());
            _setupTracerFactory = setupTracerFactory ?? _setupTracerFactory ?? new SetupLog();
        }

        ~LogManager()
        {
            if (! IsDisposed)
            {
                var tracer = SetupTracerFactory?.TracerFor(this);
                tracer?.Error("In finalizer (~LogManager) - forgot to Dispose()?");
                Dispose(false);
            }
        }

        #endregion

    }

}
