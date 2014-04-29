// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterTraceWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace.Writers
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;

	using LogJam.Trace.Formatters;


	/// <summary>
	/// Formats and writes trace messages to a <see cref="TextWriter"/>.
	/// </summary>
	public sealed class TextWriterTraceWriter : ITraceWriter, IDisposable
	{

		private bool _disposed;
		private readonly ITraceFormatter _formatter;
		private readonly TextWriter _writer;

		public TextWriterTraceWriter(TextWriter writer, ITraceFormatter traceFormatter = null)
		{
			Contract.Requires<ArgumentNullException>(writer != null);

			if (traceFormatter == null)
			{
				traceFormatter = new DebuggerTraceFormatter();
			}

			_disposed = false;
			_formatter = traceFormatter;
			_writer = writer;
		}

		/// <summary>
		/// Gets the formatter.
		/// </summary>
		/// <value>
		/// The formatter.
		/// </value>
		public ITraceFormatter Formatter { get { return _formatter; } }

		public bool IsActive { get { return !_disposed; } }

		public void Write(Tracer tracer, TraceLevel traceLevel, string message, object details)
		{
			if (! _disposed)
			{
				string formatted = _formatter.FormatTrace(DateTime.UtcNow, tracer.Name, traceLevel, message, details);
				try
				{
					_writer.WriteLine(formatted);
				}
				catch (ObjectDisposedException)
				{
					_disposed = true;
				}
			}
		}

		public void Dispose()
		{
			if (! _disposed)
			{
				try
				{
					_writer.Dispose();
				}
				catch (ObjectDisposedException)
				{}

				_disposed = true;
			}
		}

	}

}
