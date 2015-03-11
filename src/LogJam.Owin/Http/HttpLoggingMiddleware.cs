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
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

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

		private readonly ILogWriter<HttpRequestEntry> _requestEntryWriter;
		private readonly ILogWriter<HttpResponseEntry> _responseEntryWriter;

		public HttpLoggingMiddleware(OwinMiddleware next, ILogWriter<HttpRequestEntry> requestEntryWriter, ILogWriter<HttpResponseEntry> responseEntryWriter)
			: base(next)
		{
			Contract.Requires<ArgumentNullException>(next != null);
			Contract.Requires<ArgumentNullException>(requestEntryWriter != null);
			Contract.Requires<ArgumentNullException>(responseEntryWriter != null);

			_requestCounter = 0L;

			_requestEntryWriter = requestEntryWriter;
			_responseEntryWriter = responseEntryWriter;
		}

		public void Dispose()
		{
			_disposed = true;
		}

		public override async Task Invoke(IOwinContext owinContext)
		{
			DateTimeOffset requestStarted = DateTimeOffset.Now;

			// Create RequestNumber
			long requestNum = Interlocked.Increment(ref _requestCounter);
			owinContext.Set(RequestNumberKey, requestNum);

			// Log request entry
			IOwinRequest request = owinContext.Request;
			if (!_disposed && _requestEntryWriter.Enabled)
			{
				HttpRequestEntry requestEntry;
				requestEntry.RequestNumber = requestNum;
				requestEntry.RequestStarted = requestStarted;
				requestEntry.Method = request.Method;
				requestEntry.Uri = request.Uri.OriginalString;
				requestEntry.RequestHeaders = request.Headers;
				_requestEntryWriter.Write(ref requestEntry);
			}

			// Run inner handlers
			await Next.Invoke(owinContext);

			// Log response entry
			if (!_disposed && _responseEntryWriter.Enabled)
			{
				IOwinResponse response = owinContext.Response;
				HttpResponseEntry responseEntry;
				responseEntry.RequestNumber = requestNum;
				responseEntry.Ttfb = DateTimeOffset.Now - requestStarted;
				responseEntry.Method = request.Method;
				responseEntry.Uri = request.Uri.OriginalString;
				responseEntry.HttpStatusCode = (short) response.StatusCode;
				responseEntry.HttpReasonPhrase = response.ReasonPhrase;
				responseEntry.ResponseHeaders = response.Headers;
				_responseEntryWriter.Write(ref responseEntry);
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
