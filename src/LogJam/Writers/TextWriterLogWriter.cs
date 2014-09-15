// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Writers
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;

	using LogJam.Trace;
	using LogJam.Trace.Formatters;


	/// <summary>
	/// Formats and writes log entries to a <see cref="TextWriter"/>.
	/// </summary>
	public sealed class TextWriterLogWriter<TEntry> : ILogWriter<TEntry> where TEntry : ILogEntry
	{

		private bool _disposed;
		private readonly LogFormatter<TEntry> _formatter;
		private readonly TextWriter _writer;

		/// <summary>
		/// Creates a new <see cref="TextWriterLogWriter{TEntry}"/>.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/> to write formatted log output to.</param>
		/// <param name="formatter">The <see cref="LogFormatter{TEntry}"/> used to format log entries.</param>
		public TextWriterLogWriter(TextWriter writer, LogFormatter<TEntry> formatter)
		{
			Contract.Requires<ArgumentNullException>(writer != null);
			Contract.Requires<ArgumentNullException>(formatter != null);

			_disposed = false;
			_formatter = formatter;
			_writer = writer;
		}

		/// <summary>
		/// Gets the formatter.
		/// </summary>
		/// <value>
		/// The formatter.
		/// </value>
		public LogFormatter<TEntry> Formatter { get { return _formatter; } }

		/// <summary>
		/// Gets the inner <see cref="TextWriter"/>.
		/// </summary>
		/// <value>
		/// The <c>TextWriter</c> that this class writes to.
		/// </value>
		internal TextWriter TextWriter { get { return _writer; } }

		public bool Enabled { get { return !_disposed; } }

		public bool IsSynchronized { get { return false; } }

		public void Write(ref TEntry entry)
		{
			if (! _disposed)
			{
				try
				{
					_formatter.Format(ref entry, _writer);
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
