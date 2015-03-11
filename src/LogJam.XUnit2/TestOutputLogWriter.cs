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
	public sealed class TestOutputLogWriter<TEntry> : ILogWriter<TEntry>
		where TEntry : ILogEntry
	{

		private readonly LogFormatter<TEntry> _formatter;
		private readonly ITestOutputHelper _testOutput;

		/// <summary>
		/// Creates a new <see cref="TestOutputLogWriter{TEntry}"/>.
		/// </summary>
		/// <param name="testOutput">The xunit <see cref="ITestOutputHelper"/> to write formatted log output to.</param>
		/// <param name="formatter">The <see cref="LogFormatter{TEntry}"/> used to format log entries.</param>
		public TestOutputLogWriter(ITestOutputHelper testOutput, LogFormatter<TEntry> formatter = null)
		{
			Contract.Requires<ArgumentNullException>(testOutput != null);
			Contract.Requires<ArgumentNullException>((formatter != null) || (typeof(TEntry) == typeof(TraceEntry)));

			if ((formatter == null) && (typeof(TEntry) == typeof(TraceEntry)))
			{
				formatter = new TestOutputTraceFormatter() as LogFormatter<TEntry>;
			}
			_formatter = formatter;
			_testOutput = testOutput;
		}

		public LogFormatter<TEntry> Formatter { get { return _formatter; } }

		public bool Enabled { get { return true; } }

		/// <summary>
		/// Log writing is synchronized based on the assumption that the <see cref="ITestOutputHelper"/>
		/// implementation is synchronized - it must be, since xunit2 supports async tests.
		/// </summary>
		public bool IsSynchronized { get { return true; } }

		public void Write(ref TEntry entry)
		{
			_testOutput.WriteLine(_formatter.Format(ref entry));
		}

	}

}
