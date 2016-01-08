// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputFormatWriterTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2.UnitTests
{
	using System.IO;

	using LogJam.Test.Shared;
	using LogJam.Trace;
	using LogJam.Trace.Config;
	using LogJam.Trace.Format;
	using LogJam.Trace.Switches;

	using Xunit;


	/// <summary>
	/// Validates <see cref="TestOutputFormatWriter"/>.
	/// </summary>
	public sealed class TestOutputFormatWriterTests
	{

		[Fact]
		public void TestLogMatchesTextWriterLogs()
		{
			// Trace output is sent here
			var textWriter = new StringWriter();
			// And also here
			var testOutput = new FakeTestOutputHelper();
			var switches = new SwitchSet()
							   {
								   { Tracer.All, new OnOffTraceSwitch(true) }
							   };
			var traceFormatter = new DefaultTraceFormatter()
			                     {
				                     IncludeTimestamp = true
			                     };

			using (var traceManager = new TraceManager())
			{
				var testOutputLogWriterConfig = traceManager.LogManager.Config.UseTestOutput(testOutput);
				testOutputLogWriterConfig.IncludeTimeOffset = false; // Turn this off so test log formatting matches the text logging default
				testOutputLogWriterConfig.IncludeTime = true; // Turn this on so test log formatting matches the text logging default
				traceManager.Config.TraceTo(testOutputLogWriterConfig, switches, traceFormatter);

				traceManager.Config.TraceTo(textWriter, switches, traceFormatter);

				// Some test traces
				var tracer = traceManager.TracerFor(this);
				tracer.Severe("Severe message");
				tracer.Error("Error message");
				tracer.Warn("Warning message");

				TestHelper.WarnException(tracer, 5);

				tracer.Info("Info message");
				tracer.Verbose("Verbose message");
				tracer.Debug("Debug message");

				Assert.True(traceManager.IsHealthy);
			}

			Assert.Equal(textWriter.ToString(), testOutput.GetTestOutput());
		}

	}

}