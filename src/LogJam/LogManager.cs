// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManager.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using System.Collections.Generic;
	using System.Linq;

	using LogJam.Config;
	using LogJam.Trace;
	using LogJam.Trace.Config;
	using LogJam.Trace.Switches;
	using LogJam.Writers;
	using System;
	using System.Diagnostics.Contracts;
	using System.Threading;


	/// <summary>
	/// Manages all logs in LogJam.
	/// </summary>
	public sealed class LogManager : BaseLogJamManager
	{
		#region Static fields

		private static LogManager s_instance;

		#endregion

		#region Instance fields

		/// <summary>
		/// An internal <see cref="ITracerFactory"/>, which is used only for tracing LogJam operations.
		/// </summary>
		private readonly SetupTracerFactory _setupTracerFactory;

		private readonly LogManagerConfig _config;

		private readonly Dictionary<ILogWriterConfig, ILogWriter> _logWriters; 

		#endregion

		/// <summary>
		/// Sets or gets an AppDomain-global <see cref="LogManager"/>.
		/// </summary>
		public static LogManager Instance
		{
			get
			{
				if (s_instance != null)
				{
					return s_instance;
				}

				Interlocked.CompareExchange(ref s_instance, new LogManager(), null);
				return s_instance;
			}
			set
			{
				var previousInstance = Interlocked.Exchange(ref s_instance, value);
				if (previousInstance != null)
				{
					previousInstance.Stop();
				}
			}
		}

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new <see cref="LogManager"/> instance using default configuration.
		/// </summary>
		public LogManager()
			: this(new SetupTracerFactory(), new LogManagerConfig())
		{}

		/// <summary>
		/// Creates a new <see cref="LogManager"/> instance using the specified <paramref name="setupTracerFactory"/> for logging LogJam operations.
		/// </summary>
		/// <param name="setupTracerFactory">The <see cref="LogJam.SetupTracerFactory"/> to use for tracking internal operations.</param>
		public LogManager(SetupTracerFactory setupTracerFactory, LogManagerConfig logManagerConfig)
		{
			Contract.Requires<ArgumentNullException>(setupTracerFactory != null);
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);

			_setupTracerFactory = setupTracerFactory;
			_config = logManagerConfig;
			_logWriters = new Dictionary<ILogWriterConfig, ILogWriter>();
		}

		#endregion

		/// <summary>
		/// The <see cref="LogManagerConfig"/> used to configure this <c>LogManager</c>.
		/// </summary>
		public LogManagerConfig Config { get { return _config; } }

		internal override ITracerFactory SetupTracerFactory { get { return _setupTracerFactory; } }

		/// <summary>
		/// Returns the collection of <see cref="TraceEntry"/>s logged through <see cref="SetupTracerFactory"/>.
		/// </summary>
		public override IEnumerable<TraceEntry> SetupTraces { get { return _setupTracerFactory; } }

		protected override void InternalStart()
		{
			lock (this)
			{
				var logManagerTracer = SetupTracerFactory.TracerFor(this);

				if (_logWriters.Count > 0)
				{
					logManagerTracer.Debug("Stopping LogManager before re-starting it...");
					Stop();
				}

				foreach (ILogWriterConfig logWriterConfig in Config.Writers)
				{
					ILogWriter logWriter = null;
					try
					{
						logWriter = logWriterConfig.CreateILogWriter();
						IStartable startableLogWriter = logWriter as IStartable;
						if (startableLogWriter != null)
						{
							var tracer = SetupTracerFactory.TracerFor(startableLogWriter);
							tracer.Info("Starting {0} ...", logWriter);
							startableLogWriter.Start();
							tracer.Info("Successfully started {0}.", logWriter);
						}
					}
					catch (Exception excp)
					{
						// TODO: Store initialization failure status
						var tracer = SetupTracerFactory.TracerFor((object) logWriter ?? logWriterConfig);
						tracer.Severe(excp, "Exception creating or Start()ing logwriter from config: {0}", logWriterConfig);
					}

					_logWriters.Add(logWriterConfig, logWriter);
					DisposeOnStop(logWriter);
				}
			}
		}

		/// <summary>
		/// Stops all 
		/// </summary>
		protected override void InternalStop()
		{
			lock (this)
			{
				foreach (var logWriter in _logWriters.Values)
				{
					var startableLogWriter = logWriter as IStartable;
					if (startableLogWriter != null)
					{
						startableLogWriter.Stop();
					}
				}
				_logWriters.Clear();
			}
		}

		/// <summary>
		/// Returns all successfully started <see cref="ILogWriter{TEntry}"/>s associated with this <c>LogManager</c>.
		/// </summary>
		/// <typeparam name="TEntry">The logentry type written by the returned <see cref="ILogWriter{TEntry}"/>s.</typeparam>
		/// <returns>All successfully started <see cref="ILogWriter{TEntry}"/>s that are type-compatible with <typeparamref name="TEntry"/>.  May return an empty enumerable.</returns>
		/// <remarks>Even if Start() wasn't 100% successful, we still return any logwriters that were successfully started.</remarks>
		public IEnumerable<ILogWriter<TEntry>> GetLogWriters<TEntry>() where TEntry : ILogEntry
		{
			// Even if Start() wasn't 100% successful, we still return any logwriters that were successfully started.
			EnsureStarted();

			lock (this)
			{
				return _logWriters.Values.OfType<ILogWriter<TEntry>>();
			}
		}

		/// <summary>
		/// Returns a single <see cref="ILogWriter{TEntry}"/> that writes to all successfully started <see cref="ILogWriter{TEntry}"/>s associated with this <c>LogManager</c>.
		/// </summary>
		/// <typeparam name="TEntry">The logentry type written by the returned <see cref="ILogWriter{TEntry}"/>s.</typeparam>
		/// <returns>A single <see cref="ILogWriter{TEntry}"/> that writes to all successfully started <see cref="ILogWriter{TEntry}"/>s that are type-compatible with <typeparamref name="TEntry"/>.</returns>
		public ILogWriter<TEntry> GetLogWriter<TEntry>() where TEntry : ILogEntry
		{
			ILogWriter<TEntry>[] logWriters = GetLogWriters<TEntry>().ToArray();
			if (logWriters.Length == 1)
			{
				return logWriters[0];
			}
			else if (logWriters.Length == 0)
			{
				return new NoOpLogWriter<TEntry>();
			}
			else
			{
				return new FanOutLogWriter<TEntry>(logWriters);
			}
		}

		/// <summary>
		/// Returns the <see cref="ILogWriter{TEntry}"/> matching with the specified <see cref="ILogWriterConfig"/>.  This method throws exceptions if the call is invalid, but
		/// does not throw an exception if the returned logwriter failed to start.
		/// </summary>
		/// <typeparam name="TEntry">The logentry type written by the returned <see cref="ILogWriter{TEntry}"/>.</typeparam>
		/// <param name="logWriterConfig">An <see cref="ILogWriterConfig"/> instance.</param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">If no value in <c>Config.Writers</c> is equal to <paramref name="logWriterConfig"/></exception>
		public ILogWriter<TEntry> GetLogWriter<TEntry>(ILogWriterConfig logWriterConfig) where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(logWriterConfig != null);

			// Even if Start() wasn't 100% successful, we still return any logwriters that were successfully started.
			EnsureStarted();

			ILogWriter logWriter;
			if (! _logWriters.TryGetValue(logWriterConfig, out logWriter))
			{
				throw new KeyNotFoundException("logWriterConfig not found.");
			}

			if (logWriter == null)
			{	// This occurs when logWriter.Start() fails.  In this case, the desired behavior is to return a functioning logwriter.
				return new NoOpLogWriter<TEntry>();
			}
			ILogWriter<TEntry> typedLogWriter = logWriter as ILogWriter<TEntry>;
			if (typedLogWriter == null)
			{
				throw new InvalidCastException("LogWriter type " + logWriter.GetType().Name + " could not be cast to " + typeof(ILogWriter<TEntry>).Name);
			}
			return typedLogWriter;
		}

	}

}