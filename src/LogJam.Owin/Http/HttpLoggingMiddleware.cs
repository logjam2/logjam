// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpLoggingMiddleware.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Owin.Http
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using LogJam.Config;
	using LogJam.Trace;
	using LogJam.Writer;

	using Microsoft.Owin;


	/// <summary>
	/// Middleware that logs HTTP requests and responses
	/// </summary>
	internal sealed class HttpLoggingMiddleware : OwinMiddleware, IDisposable
	{

		internal const string RequestNumberKey = "LogJam.RequestNum";

		private long _requestCounter;
		private bool _disposed;

		private readonly IEntryWriter<HttpRequestEntry> _requestEntryWriter;
		private readonly IEntryWriter<HttpResponseEntry> _responseEntryWriter;
		private readonly Tracer _tracer;

		public HttpLoggingMiddleware(OwinMiddleware next, LogManager logManager, ITracerFactory tracerFactory, ILogWriterConfig[] logWriterConfigs)
			: base(next)
		{
			Contract.Requires<ArgumentNullException>(next != null);
			Contract.Requires<ArgumentNullException>(logManager != null);
			Contract.Requires<ArgumentNullException>(tracerFactory != null);
			Contract.Requires<ArgumentNullException>(logWriterConfigs != null);

			_requestCounter = 0L;

			// Sort the configured LogWriters into request and response entry writers
			var requestWriters = new List<IEntryWriter<HttpRequestEntry>>();
			var responseWriters = new List<IEntryWriter<HttpResponseEntry>>();
			foreach (var logWriterConfig in logWriterConfigs)
			{
				var logWriter = logManager.GetLogWriter(logWriterConfig);
				IEntryWriter<HttpRequestEntry> requestWriter;
				if (logWriter.TryGetEntryWriter(out requestWriter))
				{
					requestWriters.Add(requestWriter);
				}
				IEntryWriter<HttpResponseEntry> responseWriter;
				if (logWriter.TryGetEntryWriter(out responseWriter))
				{
					responseWriters.Add(responseWriter);
				}
			}

			_requestEntryWriter = requestWriters.GetSingleEntryWriter();
			_responseEntryWriter = responseWriters.GetSingleEntryWriter();

			_tracer = tracerFactory.TracerFor(this);
		}

		public void Dispose()
		{
			_disposed = true;
		}

		public override Task Invoke(IOwinContext owinContext)
		{
			Contract.Assert(owinContext != null);

			DateTimeOffset requestStarted = DateTimeOffset.Now;

			// Create RequestNumber
			long requestNum = Interlocked.Increment(ref _requestCounter);
			owinContext.Set(RequestNumberKey, requestNum);

			// Log request entry
			IOwinRequest request = owinContext.Request;
			string requestUri = request.Uri.OriginalString;
			string requestMethod = request.Method;

			if (!_disposed && _requestEntryWriter.IsEnabled)
			{
				HttpRequestEntry requestEntry;
				requestEntry.RequestNumber = requestNum;
				requestEntry.RequestStarted = requestStarted;
				requestEntry.Method = requestMethod;
				requestEntry.Uri = requestUri;
				requestEntry.RequestHeaders = request.Headers.ToArray();
				_requestEntryWriter.Write(ref requestEntry);
			}

			// Run inner handlers
			Task taskNext = Next.Invoke(owinContext);

			return taskNext.ContinueWith((innerTask) =>
			                             {
											 // Try logging the HTTP response whether or not it faulted
					                         LogHttpResponse(owinContext, requestStarted, requestNum, requestMethod, requestUri);

											 // Propagate the exception or cancellation
				                             innerTask.Wait();
			                             });
		}

		private void LogHttpResponse(IOwinContext owinContext, DateTimeOffset requestStarted, long requestNum, string requestMethod, string requestUri)
		{
			// Log response entry
			if (!_disposed && _responseEntryWriter.IsEnabled)
			{
				IOwinResponse response = owinContext.Response;
				if (response == null)
				{
					_tracer.Error("Cannot log HTTP response - no owinContext.Response.  Request #{0} {1} {2}", requestNum, requestMethod, requestUri);
				}
				else
				{
					HttpResponseEntry responseEntry;
					responseEntry.RequestNumber = requestNum;
					responseEntry.Ttfb = DateTimeOffset.Now - requestStarted;
					responseEntry.Method = requestMethod;
					responseEntry.Uri = requestUri;
					responseEntry.HttpStatusCode = (short) response.StatusCode;
					responseEntry.HttpReasonPhrase = response.ReasonPhrase;
					responseEntry.ResponseHeaders = response.Headers.ToArray();
					_responseEntryWriter.Write(ref responseEntry);
				}
			}
		}

		/// <summary>
		/// Formats the contents of an HTTP request or response body into <paramref name="sb"/>.
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="bodyStream"></param>
		/// <returns>The request or response body stream to use (must replace the current stream).</returns>
		private async Task<Stream> FormatBodyStreamAsync(StringBuilder sb, Stream bodyStream)
		{
			if ((bodyStream == null) || ! bodyStream.CanRead)
			{
				return null;
			}

			Stream streamToRead;
			if (! bodyStream.CanSeek)
			{ // Need to copy the stream into a buffer, it will replace the previous stream.
				streamToRead = new MemoryStream(1024);
				bodyStream.CopyTo(streamToRead);
			}
			else
			{
				streamToRead = bodyStream;
			}

			streamToRead.Seek(0, SeekOrigin.Begin);
			var bodyReader = new StreamReader(streamToRead);
			if (bodyReader.Peek() != -1)
			{
				// Append the body contents to the StringBuilder
				sb.AppendLine();
				sb.AppendLine(await bodyReader.ReadToEndAsync());
				streamToRead.Seek(0, SeekOrigin.Begin);
			}

			return streamToRead;
		}

	}

}
