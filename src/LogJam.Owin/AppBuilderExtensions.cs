// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBuilderExtensions.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace Owin
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;

	using LogJam.Owin;
	using LogJam.Trace;
	using LogJam.Trace.Formatters;
	using LogJam.Writers;

	using Microsoft.Owin.Logging;


	/// <summary>
	/// AppBuilder extension methods for diagnostics.
	/// </summary>
	public static class AppBuilderExtensions
	{

		private const string c_tracerFactoryKey = "LogJam.TracerFactory";

		public static void LogHttpRequests(this IAppBuilder appBuilder, bool logRequestBodies, bool logResponseBodies)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			appBuilder.Use<HttpLoggingMiddleware>(logRequestBodies, logResponseBodies);
		}

		/// <summary>
		/// Stores an application <see cref="ITracerFactory"/> in the app properties.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <param name="tracerFactory"></param>
		public static void SetTracerFactory(this IAppBuilder appBuilder, ITracerFactory tracerFactory)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			appBuilder.Properties.Add(c_tracerFactoryKey, tracerFactory);
		}

		public static ITracerFactory CreateDefaultTracerFactory(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);
			TraceManager traceManager;

			// REVIEW: This may not be a good idea, since writing to host.TraceOutput seems to be slow...
			var traceOutput = appBuilder.Properties.Get<TextWriter>("host.TraceOutput");
			if (traceOutput != null)
			{ // Use the host.TraceOutput instead of the regular debug window

				// Workaround for threading bugs:
				traceOutput = TextWriter.Synchronized(traceOutput);
				appBuilder.Properties["host.TraceOutput"] = traceOutput;


				var traceWriter = new TextWriterLogWriter<TraceEntry>(traceOutput, new DebuggerTraceFormatter());
				traceManager = new TraceManager(traceWriter);
			}
			else
			{
				traceManager = new TraceManager();
			}

			traceManager.Start();
			return traceManager;
		}

		/// <summary>
		/// Retrieves the <see cref="ITracerFactory"/> from the Properties collection.
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <returns></returns>
		public static ITracerFactory GetTracerFactory(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			object objTracerFactory;
			ITracerFactory tracerFactory;
			if (appBuilder.Properties.TryGetValue(c_tracerFactoryKey, out objTracerFactory))
			{
				tracerFactory = objTracerFactory as ITracerFactory;
				if (tracerFactory != null)
				{
					return tracerFactory;
				}
			}

			// JIT create
			tracerFactory = appBuilder.CreateDefaultTracerFactory();
			appBuilder.SetTracerFactory(tracerFactory);
			return tracerFactory;
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

		public static IAppBuilder UseTracerLogging(this IAppBuilder appBuilder)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);

			appBuilder.SetLoggerFactory(new OwinLoggerFactory(appBuilder.GetTracerFactory()));

			return appBuilder;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appBuilder"></param>
		/// <param name="logFirstChance"><c>true</c> to log first-chance exceptions - logs every exception that is thrown.</param>
		/// <param name="logUnhandled"><c>true</c> to log unhandled exceptions in the Owin pipeline.</param>
		/// <returns></returns>
		public static IAppBuilder TraceExceptions(this IAppBuilder appBuilder, bool logFirstChance = false, bool logUnhandled = true)
		{
			Contract.Requires<ArgumentNullException>(appBuilder != null);
			//if (logFirstChance || logUnhandled)
			{
				var tracer = appBuilder.TracerFor<ExceptionLoggingMiddleware>();

				appBuilder.Use<ExceptionLoggingMiddleware>(tracer, null, logFirstChance, logUnhandled);
			}

			return appBuilder;
		}

	}

}
