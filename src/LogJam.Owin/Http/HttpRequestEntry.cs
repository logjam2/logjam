// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestEntry.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.Http
{
	using System;
	using System.Collections.Generic;

	using Microsoft.Owin;


	/// <summary>
	/// A log entry that records an HTTP request.
	/// </summary>
	public struct HttpRequestEntry : ITimestampedLogEntry
	{
		/// <summary>
		/// Monotonically increasing request number - starts from 1 when the webapp is started.
		/// </summary>
		public long RequestNumber;

		/// <summary>
		/// When the HTTP request began processing.
		/// </summary>
		public DateTimeOffset RequestStarted;

		/// <summary>
		/// The HTTP request method, eg GET, POST, etc.
		/// </summary>
		public string Method;

		/// <summary>
		/// The HTTP request URI.
		/// </summary>
		public string Uri;

		/// <summary>
		/// The HTTP request headers.
		/// </summary>
		public KeyValuePair<string, string[]>[] RequestHeaders;

        DateTime ITimestampedLogEntry.TimestampUtc { get { return this.RequestStarted.UtcDateTime; } }

	}

}