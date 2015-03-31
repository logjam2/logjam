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
	/// Supports writing log entries to a <see cref="TextWriter"/>.
	/// </summary>
	public class TextWriterLogWriter : BaseLogWriter, IBufferingLogWriter
	{

		private readonly TextWriter _textWriter;
		private readonly bool _isSynchronized;
		private readonly bool _disposeWriter;
		private readonly string _newLine;

		/// <summary>
		/// Holds the function that is used to determine whether to flush the buffers or not.
		/// </summary>
		private Func<bool> _flushPredicate;

		/// <summary>
		/// Creates a new <see cref="TextWriterLogWriter"/>.
		/// </summary>
		/// <param name="textWriter">The <see cref="TextWriter"/> to write formatted log output to.</param>
		/// <param name="setupTracerFactory">The <see cref="ITracerFactory"/> to use for logging setup operations.</param>
		/// <param name="synchronize">Set to <c>true</c> if all logging operations should be synchronized.</param>
		/// <param name="disposeWriter">Whether to dispose <paramref name="textWriter"/> when the <c>TextWriterLogWriter</c> is disposed.</param>
		/// <param name="flushPredicate">A function that is used to determine whether to flush the buffers or not.  If <c>null</c>,
		/// a predicate is used to cause buffers to be flushed after every write.</param>
		public TextWriterLogWriter(TextWriter textWriter, ITracerFactory setupTracerFactory, bool synchronize = false, bool disposeWriter = true, Func<bool> flushPredicate = null)
			: base(setupTracerFactory)
		{
			Contract.Requires<ArgumentNullException>(textWriter != null);

			_textWriter = textWriter;
			_isSynchronized = synchronize;
			_disposeWriter = disposeWriter;
			_newLine = textWriter.NewLine;

			// Default to "always flush" if not specified.
			_flushPredicate = flushPredicate ?? BufferingLogWriter.AlwaysFlush;
		}

		public override bool IsSynchronized { get { return _isSynchronized; } }

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
		/// Adds the specified <typeparamref name="TEntry"/> to this <see cref="TextWriterLogWriter"/>, using <paramref name="entryFormatter"/> to
		/// format entries of type <c>TEntry</c>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="entryFormatter"></param>
		/// <returns>this, for chaining calls in fluent style.</returns>
		public TextWriterLogWriter AddFormat<TEntry>(LogFormatter<TEntry> entryFormatter)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(entryFormatter != null);

			AddEntryWriter(new InnerEntryWriter<TEntry>(this, entryFormatter));
			return this;
		}

		/// <summary>
		/// Adds the specified <typeparamref name="TEntry"/> to this <see cref="TextWriterLogWriter"/>, using <paramref name="formatAction"/> to
		/// format entries of type <c>TEntry</c>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="formatAction"></param>
		/// <returns>this, for chaining calls in fluent style.</returns>
		public TextWriterLogWriter AddFormat<TEntry>(FormatAction<TEntry> formatAction)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(formatAction != null);

			return AddFormat((LogFormatter<TEntry>) formatAction);
		}

		internal TextWriter InternalTextWriter { get { return _textWriter; } }

		private void WriteFormattedEntry(string formattedEntry)
		{
			if (! IsStarted)
			{
				return;
			}

			bool lockTaken = false;
			bool includeNewLine = ! formattedEntry.EndsWith(_newLine);
			if (_isSynchronized)
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
		/// Provides log writing to the <see cref="TextWriterLogWriter"/> for entry type <typeparamref name="TEntry"/>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		internal class InnerEntryWriter<TEntry> : IEntryWriter<TEntry>
			where TEntry : ILogEntry
		{

			private readonly TextWriterLogWriter _parent;
			private readonly LogFormatter<TEntry> _formatter;

			public InnerEntryWriter(TextWriterLogWriter parent, LogFormatter<TEntry> entryFormatter)
			{
				_parent = parent;
				_formatter = entryFormatter;
			}

			public void Write(ref TEntry entry)
			{
				if (_parent.IsStarted)
				{
					_parent.WriteFormattedEntry(_formatter.Format(ref entry));
				}
			}

			public bool Enabled { get { return _parent.IsStarted; } }

			internal LogFormatter<TEntry> Formatter { get { return _formatter; } }
			internal TextWriterLogWriter Parent { get { return _parent; } } 

		}

	}

}