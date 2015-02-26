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
	using System.Threading;

	using LogJam.Format;
	using LogJam.Util;


	/// <summary>
	/// Formats and writes log entries to a <see cref="TextWriter"/>.
	/// </summary>
	/// <seealso cref="TextWriterMultiLogWriter"/>
	public sealed class TextWriterLogWriter<TEntry> : BufferingLogWriter, ILogWriter<TEntry>, IDisposable
		where TEntry : ILogEntry
	{

		private readonly TextWriter _writer;
		private readonly LogFormatter<TEntry> _formatter;
		private readonly bool _isSynchronized;
		private readonly bool _disposeWriter;
		private bool _disposed;

		/// <summary>
		/// Creates a new <see cref="TextWriterLogWriter{TEntry}"/>.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/> to write formatted log output to.</param>
		/// <param name="formatter">The <see cref="LogFormatter{TEntry}"/> used to format log entries.</param>
		/// <param name="synchronize">Set to <c>true</c> to synchronize writes to <paramref name="writer"/>.</param>
		/// <param name="disposeWriter">Whether to dispose <paramref name="writer"/> when the <c>TextWriterLogWriter</c> is disposed.</param>
		/// <param name="flushPredicate">A function that is used to determine whether to flush the buffers or not.  If <c>null</c>,
		/// a predicate is used to cause buffers to be flushed after every write.</param>
		public TextWriterLogWriter(TextWriter writer, LogFormatter<TEntry> formatter, bool synchronize = true, bool disposeWriter = true, Func<bool> flushPredicate = null)
			: base(flushPredicate)
		{
			Contract.Requires<ArgumentNullException>(writer != null);
			Contract.Requires<ArgumentNullException>(formatter != null);

			_writer = writer;
			_formatter = formatter;
			_isSynchronized = synchronize;
			_disposeWriter = disposeWriter;
			_disposed = false;
		}

		/// <summary>
		/// Returns the <see cref="LogFormatter{TEntry}"/> used to format the text written to <see cref="TextWriter"/>.
		/// </summary>
		public LogFormatter<TEntry> Formatter { get { return _formatter; } }

		/// <summary>
		/// Gets the inner <see cref="TextWriter"/>.
		/// </summary>
		/// <value>
		/// The <c>TextWriter</c> that this class writes to.
		/// </value>
		internal TextWriter TextWriter { get { return _writer; } }

		public override bool Enabled { get { return !_disposed && IsStarted; } }

		public override bool IsSynchronized { get { return _isSynchronized; } }

		/// @inheritdoc
		public void Write(ref TEntry entry)
		{
			if (Enabled)
			{
				bool lockTaken = false;
				if (_isSynchronized)
				{
					Monitor.Enter(this, ref lockTaken);
				}
				try
				{
					_formatter.Format(ref entry, _writer);
					if (FlushPredicate())
					{
						_writer.Flush();
					}
				}
				catch (ObjectDisposedException)
				{
					_disposed = true;
				}
				finally
				{
					if (lockTaken)
					{
						Monitor.Exit(this);
					}
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
