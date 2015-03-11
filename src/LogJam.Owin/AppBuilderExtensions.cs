// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBuilderExtensions.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace Owin
{
	using LogJam;
	using LogJam.Config;
	using LogJam.Owin;
	using LogJam.Owin.Http;
	using LogJam.Trace;
	using LogJam.Trace.Config;
	using LogJam.Trace.Switches;
	using Microsoft.Owin;
	using Microsoft.Owin.Logging;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;


	/// <summary>
	/// AppBuilder extension methods for diagnostics.
	/// </summary>
	public static class AppBuilderExtensions
	{

		private const string c_logManagerKey = OwinContextExtensions.LogManagerKey;
		private const string c_tracerFactoryKey = OwinContextExtensions.TracerFactoryKey;

		private const string c_logManagerConfigKey = "LogJam.Config.LogManagerConfig";
		private const string c_traceManagerConfigKey = "LogJam.Trace.Config.TraceManagerConfig";

		/// <summary>
		/// Returns the <see cref="GetLogManagerConfig"/> used to configure LogJam logging.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <returns></returns>
		public static LogManagerConfig GetLogManagerConfig(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			lock (appBuilder.Properties)
			{
				var config = appBuilder.Properties.Get<LogManagerConfig>(c_logManagerConfigKey);
				if (config == null)
				{
					config = new LogManagerConfig();
					appBuilder.Properties.Add(c_logManagerConfigKey, config);
				}
				return config;
			}
		}

		/// <summary>
		/// Returns the <see cref="GetTraceManagerConfig"/> used to configure LogJam tracing.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <returns></returns>
		public static TraceManagerConfig GetTraceManagerConfig(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			lock (appBuilder.Properties)
			{
				var config = appBuilder.Properties.Get<TraceManagerConfig>(c_logManagerConfigKey);
				if (config == null)
				{
					config = new TraceManagerConfig();
					appBuilder.Properties.Add(c_logManagerConfigKey, config);
				}
				return config;
			}
		}

		/// <summary>
		/// Enables sending trace messages to the debugger - this is off by default within OWIN.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <returns></returns>
		public static ILogWriterConfig LogToDebugger(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			return appBuilder.GetLogManagerConfig().UseMultiDebugger();
		}

		/// <summary>
		/// Logs to the OWIN <c>host.TraceOutput</c> stream.  It seems slow, so developer beware...
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <returns></returns>
		public static TextWriterMultiLogWriterConfig LogToOwinTraceOutput(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			var owintraceOutputTextWriter = appBuilder.Properties.Get<TextWriter>("host.TraceOutput");
			if (owintraceOutputTextWriter == null)
			{
				return null;
			}

			var logWriterConfig = appBuilder.LogToText(() => owintraceOutputTextWriter);
			logWriterConfig.DisposeTextWriter = false;
			return logWriterConfig;
		}

		/// <summary>
		/// Logs to the <see cref="TextWriter"/> returned from <see cref="textWriterCreateFunc"/>.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <param name="textWriterCreateFunc">A function that is called once each time logging starts, to obtain a <see cref="TextWriter"/>.</param>
		/// <returns></returns>
		public static TextWriterMultiLogWriterConfig LogToText(this IAppBuilder appBuilder, Func<TextWriter> textWriterCreateFunc)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);
			Contract.Requires<ArgumentNullException>(textWriterCreateFunc != null);

			return appBuilder.GetLogManagerConfig().UseMultiTextWriter(textWriterCreateFunc);
		}

		/// <summary>
		/// Enables a piece of OWIN middleware that associates the application <see cref="LogManager"/> with each
		/// request.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <remarks>This registration is run automatically by several other configuration methods, so may not need to
		/// be explicitly called.  If it IS called, it should be enabled at or near the beginning of the OWIN pipeline.</remarks>
		public static void RegisterLogManagerPerRequest(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			var logManager = appBuilder.Properties.Get<LogManager>(c_logManagerKey);
			if (logManager == null)
			{
				var tracerFactory = appBuilder.Properties.Get<ITracerFactory>(c_tracerFactoryKey);
				if (tracerFactory != null)
				{
					throw new LogJamOwinSetupException("Not supported to register the ITracerFactory before the LogManager.", appBuilder);
				}
				logManager = new LogManager(appBuilder.GetLogManagerConfig());
				var traceManager = new TraceManager(logManager, appBuilder.GetTraceManagerConfig());
				appBuilder.Properties.Add(c_logManagerKey, logManager);
				appBuilder.Properties.Add(c_tracerFactoryKey, traceManager);

				appBuilder.Use<LogJamManagerMiddleware>(logManager, traceManager);
			}
		}

		/// <summary>
		/// Enables logging of all HTTP requests.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <param name="configuredLogWriters">Specifies the log writers to use for HTTP logging.</param>
		/// <remarks>This middleware should be enabled at or near the beginning of the OWIN pipeline.</remarks>
		public static void LogHttpRequests(this IAppBuilder appBuilder, params ILogWriterConfig[] configuredLogWriters)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			appBuilder.RegisterLogManagerPerRequest();

			if (configuredLogWriters.Length >= 1)
			{
				HttpLoggingMiddleware.ValidateConfiguredLogWriters(configuredLogWriters);
				appBuilder.Use<HttpLoggingMiddleware>(configuredLogWriters);
			}
		}

		/// <summary>
		/// Enables sending trace messages to <paramref name="logWriters"/>.  This method can be called multiple times to
		/// specify different switch settings for different logWriters; or <see cref="GetTraceManagerConfig"/> can
		/// be used for finer-grained control of configuration.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <param name="switches"></param>
		/// <param name="configuredLogWriters"></param>
		/// <returns></returns>
		public static void TraceTo(this IAppBuilder appBuilder, SwitchSet switches, params ILogWriterConfig[] configuredLogWriters)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);
			Contract.Requires<ArgumentNullException>(switches != null);

			foreach (var logWriterConfig in configuredLogWriters)
			{
				appBuilder.GetTraceManagerConfig().Writers.Add(new TraceWriterConfig(logWriterConfig, switches));
			}
		}

		/// <summary>
		/// Enables sending trace messages to <paramref name="logWriters"/>.  This overload uses default trace threshold
		/// (<see cref="TraceLevel.Info"/>) for all tracing.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <param name="switches"></param>
		/// <returns></returns>
		public static void TraceTo(this IAppBuilder appBuilder, params ILogWriterConfig[] configuredLogWriters)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			var traceSwitch = new ThresholdTraceSwitch(TraceLevel.Info);
			var switches = new SwitchSet()
			               {
				               { Tracer.All, traceSwitch }
			               };
			appBuilder.TraceTo(switches, configuredLogWriters);
		}

		/// <summary>
		/// Retrieves the <see cref="ITracerFactory"/> from the Properties collection.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <returns></returns>
		public static ITracerFactory GetTracerFactory(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			ITracerFactory tracerFactory = appBuilder.Properties.Get<ITracerFactory>(c_tracerFactoryKey);
			if (tracerFactory != null)
			{
				return tracerFactory;
			}

			// JIT create will happen here
			appBuilder.RegisterLogManagerPerRequest();
			return appBuilder.Properties.Get<ITracerFactory>(c_tracerFactoryKey);
		}

		/// <summary>
		/// Returns a <see cref="Tracer"/> for type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <returns></returns>
		public static Tracer TracerFor<T>(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			return appBuilder.GetTracerFactory().TracerFor<T>();
		}

		/// <summary>
		/// Uses LogJam <see cref="Tracer"/> logging for all OWIN logging.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <returns></returns>
		public static IAppBuilder UseOwinTracerLogging(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			appBuilder.SetLoggerFactory(new OwinLoggerFactory(appBuilder.GetTracerFactory()));

			return appBuilder;
		}

		/// <summary>
		/// Turns on tracing of first-chance and/or unhandled OWIN exceptions.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <param name="logFirstChance"><c>true</c> to log first-chance exceptions - logs every exception that is thrown.</param>
		/// <param name="logUnhandled"><c>true</c> to log unhandled exceptions in the Owin pipeline.</param>
		/// <returns></returns>
		public static IAppBuilder TraceExceptions(this IAppBuilder appBuilder, bool logFirstChance = false, bool logUnhandled = true)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);
			if (logFirstChance || logUnhandled)
			{
				var tracer = appBuilder.TracerFor<ExceptionLoggingMiddleware>();

				appBuilder.Use<ExceptionLoggingMiddleware>(tracer, null, logFirstChance, logUnhandled);
			}

			return appBuilder;
		}

	}

}
