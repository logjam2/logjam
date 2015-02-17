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
	public class TextWriterMultiLogWriter : MultiLogWriter
	{

		private readonly TextWriter _textWriter;
		private readonly string _newLine;

		/// <summary>
		/// Creates a new <see cref="TextWriterMultiLogWriter"/>.
		/// </summary>
		/// <param name="textWriter">The <see cref="TextWriter"/> to write formatted log output to.</param>
		/// <param name="synchronize">Set to <c>true</c> if all logging operations should be synchronized.</param>
		/// <param name="setupTracerFactory">The <see cref="ITracerFactory"/> to use for logging setup operations.</param>
		public TextWriterMultiLogWriter(TextWriter textWriter, bool synchronize, ITracerFactory setupTracerFactory)
			: base(synchronize, setupTracerFactory)
		{
			Contract.Requires<ArgumentNullException>(textWriter != null);

			_textWriter = textWriter;
			_newLine = textWriter.NewLine;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(true);
			_textWriter.Dispose();
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
			bool lockTaken = false;
			bool includeNewLine = ! formattedEntry.EndsWith(_newLine);
			if (IsSynchronized)
			{
				Monitor.Enter(this, ref lockTaken);
			}

			try
			{
				if (Enabled)
				{
					if (includeNewLine)
					{
						_textWriter.WriteLine(formattedEntry);
					}
					else
					{
						_textWriter.Write(formattedEntry);
					}
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