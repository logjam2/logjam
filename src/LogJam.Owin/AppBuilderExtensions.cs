// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBuilderExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace Owin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;

    using LogJam;
    using LogJam.Config;
    using LogJam.Owin;
    using LogJam.Owin.Http;
    using LogJam.Trace;
    using LogJam.Trace.Config;

    using Microsoft.Owin;
    using Microsoft.Owin.BuilderProperties;
    using Microsoft.Owin.Logging;


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
        /// Returns the <see cref="LogManagerConfig" /> used to configure LogJam logging.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static LogManagerConfig GetLogManagerConfig(this IAppBuilder appBuilder)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);

            var config = appBuilder.Properties.Get<LogManagerConfig>(c_logManagerConfigKey);
            if (config == null)
            {
                // Includes creating the LogManagerConfig
                appBuilder.RegisterLogManager();
                config = appBuilder.Properties.Get<LogManagerConfig>(c_logManagerConfigKey);
            }

            return config;
        }

        /// <summary>
        /// Returns the <see cref="TraceManagerConfig" /> used to configure LogJam tracing.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static TraceManagerConfig GetTraceManagerConfig(this IAppBuilder appBuilder)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);

            var config = appBuilder.Properties.Get<TraceManagerConfig>(c_traceManagerConfigKey);
            if (config == null)
            {
                // Includes creating the TraceManagerConfig
                appBuilder.RegisterLogManager();
                config = appBuilder.Properties.Get<TraceManagerConfig>(c_traceManagerConfigKey);
            }

            return config;
        }

        /// <summary>
        /// Retrieves the <see cref="ITracerFactory" /> from the Properties collection.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static ITracerFactory GetTracerFactory(this IAppBuilder appBuilder)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);

            ITracerFactory tracerFactory = appBuilder.Properties.Get<ITracerFactory>(c_tracerFactoryKey);
            if (tracerFactory == null)
            {
                // Includes creating the TraceManager
                appBuilder.RegisterLogManager();
                tracerFactory = appBuilder.Properties.Get<ITracerFactory>(c_tracerFactoryKey);
            }

            return tracerFactory;
        }

        /// <summary>
        /// Retrieves the <see cref="LogManager" /> from the Properties collection.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static LogManager GetLogManager(this IAppBuilder appBuilder)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);

            LogManager logManager = appBuilder.Properties.Get<LogManager>(c_logManagerKey);
            if (logManager == null)
            {
                logManager = appBuilder.RegisterLogManager();
            }

            return logManager;
        }

        /// <summary>
        /// Logs to the OWIN <c>host.TraceOutput</c> stream.  It seems slow, so developer beware...
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static TextWriterLogWriterConfig LogToOwinTraceOutput(this IAppBuilder appBuilder)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);

            var owintraceOutputTextWriter = appBuilder.Properties.Get<TextWriter>("host.TraceOutput");
            if (owintraceOutputTextWriter == null)
            {
                return null;
            }

            var logWriterConfig = appBuilder.GetLogManagerConfig().UseTextWriter(() => owintraceOutputTextWriter);
            logWriterConfig.DisposeTextWriter = false;
            return logWriterConfig;
        }

        /// <summary>
        /// Registers <see cref="LogManager" /> and <see cref="TraceManager" /> instances, along with
        /// <see cref="LogManagerConfig" /> and <see cref="TraceManagerConfig" />, with the <paramref name="appBuilder" />.  In
        /// addition,
        /// this method registers OWIN middleware that associates the application <see cref="LogManager" /> and
        /// <see cref="TraceManager" /> with each request.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="logManager"></param>
        /// <param name="traceManager"></param>
        /// <remarks>
        /// LogManager and TraceManager registration is run automatically by several other configuration methods, so usually does
        /// not need to
        /// be explicitly called.  If it IS explicitly called (for example to use a custom <see cref="LogManager" /> for testing),
        /// it should be
        /// called before any other <c>LogJam.Owin</c> configuration methods.
        /// </remarks>
        public static LogManager RegisterLogManager(this IAppBuilder appBuilder, TraceManager traceManager = null, LogManager logManager = null)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);

            const string keyUsingLogJamManagerMiddleware = "Using:LogJamManagerMiddleware";

            var logManagerConfig = logManager?.Config ?? new LogManagerConfig();
            if (traceManager == null)
            {
                // Create new instance - using a global instance by default is a bad idea (eg webapp tests running concurrently or without adequate cleanup)
                traceManager = new TraceManager(logManager ?? new LogManager(logManagerConfig));
            }
            if (logManager == null)
            {
                logManager = traceManager.LogManager;
            }

            if (logManager != traceManager.LogManager)
            {
                throw new LogJamOwinSetupException("Not supported to register disassociated TraceManager and LogManager instances.", appBuilder);
            }

            appBuilder.Properties[c_logManagerConfigKey] = logManager.Config;
            appBuilder.Properties[c_traceManagerConfigKey] = traceManager.Config;

            appBuilder.Properties[c_logManagerKey] = logManager;
            appBuilder.Properties[c_tracerFactoryKey] = traceManager;

            // LogJamManagerMiddleware manages lifetime + registering the LogManager and TraceManager per request
            // Use the key to ensure that the middleware is only added once
            if (! appBuilder.Properties.ContainsKey(keyUsingLogJamManagerMiddleware))
            {
                appBuilder.Use<LogJamManagerMiddleware>(logManager, traceManager);
                appBuilder.Properties.Add(keyUsingLogJamManagerMiddleware, logManager);
            }

            // Ensure LogManager.Dispose - LogJamManagerMiddleware.Dispose() is not reliably called.
            var properties = new AppProperties(appBuilder.Properties);
            properties.OnAppDisposing.Register(() =>
                                               {
                                                   traceManager.Dispose();
                                                   logManager.Dispose();
                                               });

            return logManager;
        }

        /// <summary>
        /// Enables logging of all HTTP requests to the specified <paramref name="configuredLogWriters" />.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="configuredLogWriters">Specifies the log writers to use for HTTP logging.</param>
        /// <remarks>This middleware should be enabled at or near the beginning of the OWIN pipeline.</remarks>
        public static void LogHttpRequests(this IAppBuilder appBuilder, IEnumerable<ILogWriterConfig> configuredLogWriters)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);
            Contract.Requires<ArgumentNullException>(configuredLogWriters != null);

            const string keyUsingHttpLoggingMiddleware = "Using:HttpLoggingMiddleware";

            var logManager = appBuilder.GetLogManager();
            var tracerFactory = appBuilder.GetTracerFactory();

            ILogWriterConfig[] logWriterConfigs = configuredLogWriters.ToArray();
            if (logWriterConfigs.Any())
            {
                logWriterConfigs.FormatAll<HttpRequestEntry>()
                                .FormatAll<HttpResponseEntry>();

                // Use the key to ensure that the middleware is only added once
                if (! appBuilder.Properties.ContainsKey(keyUsingHttpLoggingMiddleware))
                {
                    appBuilder.Use<HttpLoggingMiddleware>(logManager, tracerFactory);
                    appBuilder.Properties.Add(keyUsingHttpLoggingMiddleware, logManager);
                }
            }
        }

        /// <summary>
        /// Enables logging of all HTTP requests to the specified <paramref name="logWriter" />.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <param name="logWriter">Specifies the log writer to use for HTTP logging.</param>
        /// <remarks>This middleware should be enabled at or near the beginning of the OWIN pipeline.</remarks>
        public static void LogHttpRequests(this IAppBuilder appBuilder, ILogWriterConfig logWriter)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);
            Contract.Requires<ArgumentNullException>(logWriter != null);

            LogHttpRequests(appBuilder, new[] { logWriter });
        }

        /// <summary>
        /// Enables logging of HTTP requests to all currently configured log writers.
        /// </summary>
        /// <param name="appBuilder"></param>
        public static void LogHttpRequestsToAll(this IAppBuilder appBuilder)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);

            LogHttpRequests(appBuilder, appBuilder.GetLogManagerConfig().Writers);
        }

        /// <summary>
        /// Returns a <see cref="Tracer" /> for type <typeparamref name="T" />.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static Tracer TracerFor<T>(this IAppBuilder appBuilder)
        {
            Contract.Requires<ArgumentNullException>(appBuilder != null);

            return appBuilder.GetTracerFactory().TracerFor<T>();
        }

        /// <summary>
        /// Uses LogJam <see cref="Tracer" /> logging for all OWIN logging.
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
