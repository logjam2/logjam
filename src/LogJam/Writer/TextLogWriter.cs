// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextLogWriter.cs">
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


	/// <summary>
	/// Supports writing log entries to a target that receives formatted text.
	/// </summary>
	public abstract class TextLogWriter : BaseLogWriter
	{

		//private readonly string _newLine;

		/// <summary>
		/// Creates a new <see cref="TextLogWriter"/>.
		/// </summary>
		/// <param name="setupTracerFactory">The <see cref="ITracerFactory"/> to use for logging setup operations.</param>
		protected TextLogWriter(ITracerFactory setupTracerFactory)
			: base(setupTracerFactory)
		{}

		/// <summary>
		/// Returns <c>true</c> when this logwriter and its entrywriters are ready to log.
		/// </summary>
		public virtual bool IsEnabled { get { return IsStarted; } }

		/// <summary>
		/// Adds the specified <typeparamref name="TEntry"/> to this <see cref="TextWriterLogWriter"/>, using <paramref name="entryFormatter"/> to
		/// format entries of type <c>TEntry</c>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="entryFormatter"></param>
		/// <returns>this, for chaining calls in fluent style.</returns>
		public TextLogWriter AddFormat<TEntry>(LogFormatter<TEntry> entryFormatter)
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
		public TextLogWriter AddFormat<TEntry>(FormatAction<TEntry> formatAction)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(formatAction != null);

			return AddFormat((LogFormatter<TEntry>) formatAction);
		}

		protected abstract void WriteFormattedEntry(string formattedEntry);

		/// <summary>
		/// Provides log writing to the <see cref="TextLogWriter"/> for entry type <typeparamref name="TEntry"/>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		internal class InnerEntryWriter<TEntry> : IEntryWriter<TEntry>
			where TEntry : ILogEntry
		{

			private readonly TextLogWriter _parent;
			private readonly LogFormatter<TEntry> _formatter;

			public InnerEntryWriter(TextLogWriter parent, LogFormatter<TEntry> entryFormatter)
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

			public bool IsEnabled { get { return _parent.IsEnabled; } }

			internal LogFormatter<TEntry> Formatter { get { return _formatter; } }
			internal TextLogWriter Parent { get { return _parent; } } 

		}

	}

}