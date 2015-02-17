// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Writer
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;

	using LogJam.Format;


	/// <summary>
	/// Formats and writes log entries to a <see cref="TextWriter"/>.
	/// </summary>
	/// <seealso cref="TextWriterMultiLogWriter"/>
	public sealed class TextWriterLogWriter<TEntry> : ILogWriter<TEntry>, IDisposable
		where TEntry : ILogEntry
	{

		private readonly TextWriter _writer;
		private readonly LogFormatter<TEntry> _formatter;
		private readonly bool _disposeWriter;
		private bool _disposed;

		/// <summary>
		/// Creates a new <see cref="TextWriterLogWriter{TEntry}"/>.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/> to write formatted log output to.</param>
		/// <param name="formatter">The <see cref="LogFormatter{TEntry}"/> used to format log entries.</param>
		/// <param name="autoFlush">Whether to call <see cref="System.IO.TextWriter.Flush"/> after every entry is written.</param>
		/// <param name="disposeWriter">Whether to dispose <paramref name="writer"/> when the <c>TextWriterLogWriter</c> is disposed.</param>
		public TextWriterLogWriter(TextWriter writer, LogFormatter<TEntry> formatter, bool autoFlush = true, bool disposeWriter = true)
		{
			Contract.Requires<ArgumentNullException>(writer != null);
			Contract.Requires<ArgumentNullException>(formatter != null);

			_writer = writer;
			_formatter = formatter;
			_disposeWriter = disposeWriter;
			_disposed = false;
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

		/// <summary>
		/// Whether to call <see cref="System.IO.TextWriter.Flush"/> after every entry is written.
		/// </summary>
		public bool AutoFlush { get; set; }

		public bool Enabled { get { return !_disposed; } }

		public bool IsSynchronized { get { return false; } }

		public void Write(ref TEntry entry)
		{
			if (! _disposed)
			{
				try
				{
					_formatter.Format(ref entry, _writer);
					if (AutoFlush)
					{
						_writer.Flush();
					}
				}
				catch (ObjectDisposedException)
				{
					_disposed = true;
				}
			}
		}

		public void Dispose()
		{
			lock (this)
			{
				if (! _disposed)
				{
					_writer.Flush();

					if (_disposeWriter)
					{
						try
						{
							_writer.Dispose();
						}
						catch (ObjectDisposedException)
						{}
					}

					_disposed = true;
				}
			}
		}

	}

}
