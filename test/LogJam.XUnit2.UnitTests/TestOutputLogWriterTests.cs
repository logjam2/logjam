// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOuputLogWriterTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2.UnitTests
{
	using System.IO;
	using System.Linq;
	using System.Text;

	using LogJam.Internal.UnitTests.Examples;
	using LogJam.Test.Shared;
	using LogJam.Trace;
	using LogJam.Trace.Config;
	using LogJam.Trace.Format;
	using LogJam.Trace.Switches;
	using LogJam.Writer;
	using LogJam.Writer.Text;

	using Xunit;


	/// <summary>
	/// Validates <see cref="TestOuputLogWriter" />.
	/// </summary>
	public sealed class TestOuputLogWriterTests
	{

		private readonly TraceManager _traceManager;

		public TestOuputLogWriterTests()
		{
			_traceManager = new TraceManager();
		}

		[Fact]
		public void LogWriterWritesToExpectedTestOutputAfterSetOutput()
		{
			var testOutput1 = new FakeTestOutputHelper();
			var testOutputConfig = _traceManager.Config.TraceToTestOutput(testOutput1);
			var tracer = _traceManager.TracerFor(this);

			tracer.Trace(TraceLevel.Info, "Test Output 1");
			Assert.True(testOutput1.GetTestOutput().Contains("Test Output 1"));

			var testOutput2 = new FakeTestOutputHelper();
			_traceManager.LogManager.SetTestOutputForWriterConfig(testOutputConfig.LogWriterConfig, testOutput2);
			tracer.Trace(TraceLevel.Info, "Test Output 2");
			Assert.False(testOutput1.GetTestOutput().Contains("Test Output 2"));
			Assert.True(testOutput2.GetTestOutput().Contains("Test Output 2"));
		}

		[Fact]
		public void LogWriterWritesWithExpectedFormatAfterSetOutput()
		{
			var testOutput1 = new FakeTestOutputHelper();
			var logManager = new LogManager();
			var testOutputConfig = logManager.Config.UseTestOutput(testOutput1);
			testOutputConfig.Format(new NumberEntryFormatter("X"));

			var writer = logManager.GetLogWriter(testOutputConfig);
			Assert.NotNull(writer);
			Assert.True(writer.EntryWriters.Count() == 1);

			IEntryWriter<NumberEntry> entryWriter;
			writer.TryGetEntryWriter<NumberEntry>(out entryWriter);
			Assert.NotNull(entryWriter);

			var entry = new NumberEntry() { Value = 8 };
			entryWriter.Write(ref entry);
			Assert.True(testOutput1.GetTestOutput().Contains("8"));
			
			var testOutput2 = new FakeTestOutputHelper();
			logManager.SetTestOutputForWriterConfig(testOutputConfig, testOutput2);
			Assert.True(writer.EntryWriters.Count() == 1);
			entry = new NumberEntry() { Value = 15 };
			entryWriter.Write(ref entry);
			Assert.False(testOutput1.GetTestOutput().Contains("F"));
			Assert.True(testOutput2.GetTestOutput().Contains("F"));
		}

		[Fact]
		public void LogWriterCanAddFormatterAfterSetOuput()
		{
			var testOutput1 = new FakeTestOutputHelper();
			var logManager = new LogManager();
			var testOutputConfig = logManager.Config.UseTestOutput(testOutput1);
			testOutputConfig.Format(new NumberEntryFormatter("X"));

			var writer = logManager.GetLogWriter(testOutputConfig);
			Assert.NotNull(writer);
			Assert.True(writer.EntryWriters.Count() == 1);
			
			var testOutput2 = new FakeTestOutputHelper();
			logManager.SetTestOutputForWriterConfig(testOutputConfig, testOutput2);
			testOutputConfig.Format(new MessageEntry.MessageEntryFormatter());

			logManager.Start();

			writer = logManager.GetLogWriter(testOutputConfig);
			Assert.True(writer.EntryWriters.Count() == 2);
		}

		private class NumberEntry : ILogEntry
		{

			public int Value { get; set; }

		}
		private class NumberEntryFormatter : EntryFormatter<NumberEntry>
		{

			private readonly string _format;
			public NumberEntryFormatter(string format)
			{
				_format = format;
			}

			public override void Format(ref NumberEntry entry, FormatWriter formatWriter)
			{
				StringBuilder buf = formatWriter.FieldBuffer;

				formatWriter.BeginEntry(0);
				buf.Clear();
				buf.Append(entry.Value.ToString(_format));
				formatWriter.WriteLine(buf);
				formatWriter.EndEntry();
			}

		}
	}

}
