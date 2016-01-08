// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfigExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Config;
	using LogJam.Trace;
	using LogJam.Trace.Config;
	using LogJam.Writer.Text;

	using Xunit.Abstractions;


	/// <summary>
	/// Extension methods for configuring LogJam to log to XUnit2 output.
	/// </summary>
	public static class LogJamConfigExtensions
	{

		public static TestOutputLogWriterConfig UseTestOutput(this LogManagerConfig logManagerConfig,
																  ITestOutputHelper testOutput)
		{
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);
			Contract.Requires<ArgumentNullException>(testOutput != null);

			var testOutputLogWriterConfig = new TestOutputLogWriterConfig(testOutput);
			return testOutputLogWriterConfig;
		}

		public static TraceWriterConfig TraceToTestOutput(this TraceManagerConfig traceManagerConfig,
		                                                          ITestOutputHelper testOutput,
		                                                          SwitchSet switchSet,
		                                                          EntryFormatter<TraceEntry> traceFormatter = null)
		{
			Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
			Contract.Requires<ArgumentNullException>(testOutput != null);
			Contract.Requires<ArgumentNullException>(switchSet != null);

			return TraceManagerConfigFluentExtensions.TraceTo(traceManagerConfig, new TestOutputLogWriterConfig(testOutput), switchSet, traceFormatter);
		}

		public static TraceWriterConfig TraceToTestOutput(this TraceManagerConfig traceManagerConfig,
		                                                  ITestOutputHelper testOutput,
		                                                  string tracerName = Tracer.All,
		                                                  ITraceSwitch traceSwitch = null,
		                                                  EntryFormatter<TraceEntry> traceFormatter = null)
		{
			Contract.Requires<ArgumentNullException>(traceManagerConfig != null);
			Contract.Requires<ArgumentNullException>(testOutput != null);
			Contract.Requires<ArgumentNullException>(tracerName != null);

			return TraceManagerConfigFluentExtensions.TraceTo(traceManagerConfig, new TestOutputLogWriterConfig(testOutput), tracerName, traceSwitch, traceFormatter);
		}

	}

}