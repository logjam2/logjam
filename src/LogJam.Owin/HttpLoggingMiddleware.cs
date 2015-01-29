// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpLoggingMiddleware.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Owin
{
	using System;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.Owin;


	/// <summary>
	/// Middleware that logs HTTP requests and responses
	/// </summary>
	public sealed class HttpLoggingMiddleware : OwinMiddleware, IDisposable
	{

		internal const string RequestNumberKey = "LogJam.RequestNum";

		private readonly bool _logRequestBodies;
		private readonly bool _logResponseBodies;
		private long _requestCounter;
		private bool _disposed;

		public HttpLoggingMiddleware(OwinMiddleware next, bool logRequestBodies, bool logResponseBodies)
			: base(next)
		{
			_logRequestBodies = logRequestBodies;
			_logResponseBodies = logResponseBodies;
			_requestCounter = 0L;
		}

		public void Dispose()
		{
			_disposed = true;
		}

		public override async Task Invoke(IOwinContext owinContext)
		{
			DateTime requestStarted = DateTime.Now;

			TextWriter traceOutput = owinContext.TraceOutput;

			// Create RequestNumber
			long requestNum = Interlocked.Increment(ref _requestCounter);
			owinContext.Set(RequestNumberKey, requestNum);

			// Log request
			Task writeRequestTask = null;
			StringBuilder sb = new StringBuilder(1024);
			IOwinRequest request = owinContext.Request;
			if (!_disposed && (traceOutput != null))
			{
				sb.AppendFormat("{0}>\t{1:HH:mm:ss.fff}\t{2}\t{3}", requestNum, requestStarted, request.Method, request.Uri);
				sb.AppendLine();
				FormatHeaders(sb, request.Headers);

				if (_logRequestBodies)
				{
					request.Body = await FormatBodyStreamAsync(sb, request.Body);
				}

				writeRequestTask = WriteAsync(traceOutput, sb);
			}

			// Run inner handlers
			await Next.Invoke(owinContext);

			// Log response
			if (!_disposed && (traceOutput != null))
			{
				IOwinResponse response = owinContext.Response;
				sb.Clear();
				TimeSpan ttfb = DateTime.Now - requestStarted;
				sb.AppendFormat("{0}<\tResponse:   \t{1}\t{2}\t   {3:ss\\.fff}s", requestNum, request.Method, request.Uri, ttfb);
				sb.AppendLine();
				sb.AppendFormat("HTTP {0}\t{1}", response.StatusCode, response.ReasonPhrase);

				sb.AppendLine();
				FormatHeaders(sb, response.Headers);

				if (_logResponseBodies)
				{
					response.Body = await FormatBodyStreamAsync(sb, response.Body);
				}

#pragma warning disable 4014
			    if (writeRequestTask != null)
			    {
			        await writeRequestTask;
			    }

				await WriteAsync(traceOutput, sb);
			}
#pragma warning restore 4014
		}

		private void FormatHeaders(StringBuilder sb, IHeaderDictionary headerDictionary)
		{
			foreach (var header in headerDictionary)
			{
				foreach (string value in header.Value)
				{
					sb.Append(header.Key);
					sb.Append(": ");
					sb.AppendLine(value);
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

		private Task WriteAsync(TextWriter traceOutput, StringBuilder sb)
		{
			return traceOutput.WriteLineAsync(sb.ToString()).ContinueWith(
			                                                              task =>
			                                                              {
				                                                              var aggException = task.Exception;
				                                                              if ((aggException != null) && aggException.InnerException is ObjectDisposedException)
				                                                              {
					                                                              // Stop logging if the TraceOutput stream is disposed
					                                                              Dispose();
				                                                              }
				                                                              return task;
			                                                              },
			                                                              TaskContinuationOptions.NotOnRanToCompletion);
		}

	}

}
