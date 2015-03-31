// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.XUnit2
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Format;
	using LogJam.Trace;
	using LogJam.Writer;

	using Xunit.Abstractions;


	/// <summary>
	/// Formats and writes log entries to an <see cref="ITestOutputHelper"/>. This enables
	/// capturing trace and other log output and writing it to a test's xunit 2.0 test output.
	/// </summary>
	public sealed class TestOutputLogWriter : BaseLogWriter
	{

		private readonly ITestOutputHelper _testOutput;

		/// <summary>
		/// Creates a new <see cref="TestOutputLogWriter"/>.
		/// </summary>
		/// <param name="testOutput">The xunit <see cref="ITestOutputHelper"/> to write formatted log output to.</param>
		/// <param name="setupTracerFactory">The <see cref="ITracerFactory"/> used to trace setup operations.</param>
		public TestOutputLogWriter(ITestOutputHelper testOutput, ITracerFactory setupTracerFactory)
			: base(setupTracerFactory)
		{
			Contract.Requires<ArgumentNullException>(testOutput != null);

			_testOutput = testOutput;
		}

		/// <summary>
		/// Adds the specified <typeparamref name="TEntry"/> to this <see cref="TextWriterLogWriter"/>, using <paramref name="entryFormatter"/> to
		/// format entries of type <c>TEntry</c>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="entryFormatter"></param>
		/// <returns>this, for chaining calls in fluent style.</returns>
		public TestOutputLogWriter AddFormat<TEntry>(LogFormatter<TEntry> entryFormatter)
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
		public TestOutputLogWriter AddFormat<TEntry>(FormatAction<TEntry> formatAction)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(formatAction != null);

			return AddFormat((LogFormatter<TEntry>) formatAction);
		}


		/// <summary>
		/// Log writing is synchronized based on the assumption that the <see cref="ITestOutputHelper"/>
		/// implementation is synchronized - it must be, since xunit2 supports async tests.
		/// </summary>
		public override bool IsSynchronized { get { return true; } }

		private void WriteFormattedEntry(string formattedEntry)
		{
			_testOutput.WriteLine(formattedEntry);
		}

		/// <summary>
		/// Provides log writing to the <see cref="TestOutputLogWriter"/> for entry type <typeparamref name="TEntry"/>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		internal class InnerEntryWriter<TEntry> : IEntryWriter<TEntry>
			where TEntry : ILogEntry
		{

			private readonly TestOutputLogWriter _parent;
			private readonly LogFormatter<TEntry> _formatter;

			public InnerEntryWriter(TestOutputLogWriter parent, LogFormatter<TEntry> entryFormatter)
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

			internal TestOutputLogWriter Parent { get { return _parent; } }

		}

	}

}
