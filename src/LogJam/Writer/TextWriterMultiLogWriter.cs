// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterMultiLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using LogJam.Format;
	using LogJam.Trace;
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Threading;


	/// <summary>
	/// Supports writing multiple log entry types to a <see cref="TextWriter"/>.
	/// </summary>
	/// <seealso cref="TextWriterLogWriter{TEntry}"/>
	public class TextWriterMultiLogWriter : MultiLogWriter, IBufferingLogWriter
	{

		private readonly TextWriter _textWriter;
		private readonly bool _disposeWriter;
		private readonly string _newLine;

		/// <summary>
		/// Holds the function that is used to determine whether to flush the buffers or not.
		/// </summary>
		private Func<bool> _flushPredicate;

		/// <summary>
		/// Creates a new <see cref="TextWriterMultiLogWriter"/>.
		/// </summary>
		/// <param name="textWriter">The <see cref="TextWriter"/> to write formatted log output to.</param>
		/// <param name="setupTracerFactory">The <see cref="ITracerFactory"/> to use for logging setup operations.</param>
		/// <param name="synchronize">Set to <c>true</c> if all logging operations should be synchronized.</param>
		/// <param name="disposeWriter">Whether to dispose <paramref name="writer"/> when the <c>TextWriterLogWriter</c> is disposed.</param>
		/// <param name="flushPredicate">A function that is used to determine whether to flush the buffers or not.  If <c>null</c>,
		/// a predicate is used to cause buffers to be flushed after every write.</param>
		public TextWriterMultiLogWriter(TextWriter textWriter, ITracerFactory setupTracerFactory, bool synchronize = false, bool disposeWriter = true, Func<bool> flushPredicate = null)
			: base(synchronize, setupTracerFactory)
		{
			Contract.Requires<ArgumentNullException>(textWriter != null);

			_textWriter = textWriter;
			_disposeWriter = disposeWriter;
			_newLine = textWriter.NewLine;

			// Default to "always flush" if not specified.
			_flushPredicate = flushPredicate ?? BufferingLogWriter.AlwaysFlush;
		}

		/// @inheritdoc
		public Func<bool> FlushPredicate
		{
			get { return _flushPredicate; }
			set
			{
				if (IsStarted)
				{
					throw new LogJamSetupException("FlushPredicate cannot be set after logwriter is started.", this);
				}
				_flushPredicate = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(true);
			_textWriter.Flush();
			if (_disposeWriter)
			{
				_textWriter.Dispose();
			}
		}

		/// <summary>
		/// Adds the specified <typeparamref name="TEntry"/> to this <see cref="TextWriterMultiLogWriter"/>, using <paramref name="entryFormatter"/> to
		/// format entries of type <c>TEntry</c>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="entryFormatter"></param>
		/// <returns>this, for chaining calls in fluent style.</returns>
		public TextWriterMultiLogWriter AddFormat<TEntry>(LogFormatter<TEntry> entryFormatter)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(entryFormatter != null);

			AddLogWriter(new InnerLogWriter<TEntry>(this, entryFormatter));
			return this;
		}

		/// <summary>
		/// Adds the specified <typeparamref name="TEntry"/> to this <see cref="TextWriterMultiLogWriter"/>, using <paramref name="formatAction"/> to
		/// format entries of type <c>TEntry</c>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="formatAction"></param>
		/// <returns>this, for chaining calls in fluent style.</returns>
		public TextWriterMultiLogWriter AddFormat<TEntry>(FormatAction<TEntry> formatAction)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(formatAction != null);

			return AddFormat((LogFormatter<TEntry>) formatAction);
		}

		private void WriteFormattedEntry(string formattedEntry)
		{
			if (! Enabled)
			{
				return;
			}

			bool lockTaken = false;
			bool includeNewLine = ! formattedEntry.EndsWith(_newLine);
			if (IsSynchronized)
			{
				Monitor.Enter(this, ref lockTaken);
			}
			try
			{
				if (includeNewLine)
				{
					_textWriter.WriteLine(formattedEntry);
				}
				else
				{
					_textWriter.Write(formattedEntry);
				}
				if (_flushPredicate())
				{
					_textWriter.Flush();
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(this);
				}
			}
		}

		/// <summary>
		/// Provides log writing to the <see cref="TextWriterMultiLogWriter"/> for entry type <typeparamref name="TEntry"/>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		private class InnerLogWriter<TEntry> : ILogWriter<TEntry>
			where TEntry : ILogEntry
		{

			private readonly TextWriterMultiLogWriter _parent;
			private readonly LogFormatter<TEntry> _formatter;

			public InnerLogWriter(TextWriterMultiLogWriter parent, LogFormatter<TEntry> entryFormatter)
			{
				_parent = parent;
				_formatter = entryFormatter;
			}

			public void Write(ref TEntry entry)
			{
				if (_parent.Enabled)
				{
					_parent.WriteFormattedEntry(_formatter.Format(ref entry));
				}
			}

			public bool Enabled { get { return _parent.Enabled; } }

			public bool IsSynchronized { get { return _parent.IsSynchronized; } }

		}

	}

}