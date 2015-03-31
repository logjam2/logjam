// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamWebApiTraceWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.WebApi
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Net.Http;
	using System.Text;

	using LogJam.Trace;


	/// <summary>
	/// A Web API <see cref="ITraceWriter"/> that writes all Web API trace output to LogJam <see cref="Tracer"/>s.
	/// </summary>
	public sealed class LogJamWebApiTraceWriter : System.Web.Http.Tracing.ITraceWriter
	{

		private readonly ITracerFactory _tracerFactory;

		/// <summary>
		/// Creates a new <see cref="LogJamWebApiTraceWriter"/>, which writes all Web API trace output
		/// to <see cref="Tracer"/>s obtained from <paramref name="tracerFactory"/>.
		/// </summary>
		/// <param name="tracerFactory">A <see cref="ITracerFactory"/>.</param>
		public LogJamWebApiTraceWriter(ITracerFactory tracerFactory)
		{
			Contract.Requires<ArgumentNullException>(tracerFactory != null);

			_tracerFactory = tracerFactory;
		}

		public void Trace(HttpRequestMessage request, string category, System.Web.Http.Tracing.TraceLevel level, Action<System.Web.Http.Tracing.TraceRecord> traceAction)
		{
			Tracer tracer = _tracerFactory.GetTracer(category);
			TraceLevel traceLevel = ConvertToLogJamTraceLevel(level);
			if (tracer.IsTraceEnabled(traceLevel))
			{
				var sb = new StringBuilder();
				if (request != null)
				{
					long requestNum = request.GetRequestNumber();
					if (requestNum > 0)
					{
						sb.Append(requestNum);
					}
					else
					{
						sb.Append(request.RequestUri.OriginalString);
					}
					sb.Append(": ");
				}

				var traceRecord = new System.Web.Http.Tracing.TraceRecord(request, category, level);
				traceAction(traceRecord);
				sb.AppendFormat("{0} {1} {2} {3}", traceRecord.Kind, traceRecord.Operation, traceRecord.Operator, traceRecord.Message);

				// Avoid logging the same exception over and over
				Exception traceException = traceRecord.Exception;
				if (traceException != null)
				{
					if (request != null)
					{
						if (request.HasRequestExceptionBeenLogged(traceException))
						{
							sb.AppendLine();
							sb.Append("    (exception already logged)");
							tracer.Trace(traceLevel, sb.ToString());
						}
						else
						{
							request.LoggedRequestException(traceException);
							tracer.Trace(traceLevel, traceException, sb.ToString());
						}
					}
				}
				else
				{
					tracer.Trace(traceLevel, sb.ToString());
				}

			}
		}

		private TraceLevel ConvertToLogJamTraceLevel(System.Web.Http.Tracing.TraceLevel level)
		{
			switch (level)
			{
				case System.Web.Http.Tracing.TraceLevel.Off:
					return TraceLevel.Off;
				case System.Web.Http.Tracing.TraceLevel.Debug:
					return TraceLevel.Debug;
				case System.Web.Http.Tracing.TraceLevel.Info:
					// Info is used promiscuously in Web API - thus we switch it to Verbose.
					return TraceLevel.Verbose;
				case System.Web.Http.Tracing.TraceLevel.Warn:
					return TraceLevel.Warn;
				case System.Web.Http.Tracing.TraceLevel.Error:
					return TraceLevel.Error;
				case System.Web.Http.Tracing.TraceLevel.Fatal:
					return TraceLevel.Severe;
				default:
					throw new ArgumentException("Value " + level + " is invalid.", "level");
			}
		}

	}

}
