// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleTestTracing.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2.UnitTests
{
	using System;

	using LogJam.Trace;
	using LogJam.Trace.Switches;

	using Xunit;
	using Xunit.Abstractions;


	/// <summary>
	/// Exercises use of TestOutputLogWriter.
	/// </summary>
	public sealed class ExampleTestTracing
	{
		private readonly ThresholdTraceSwitch _traceSwitch;
		private readonly TraceManager _traceManager;

		public ExampleTestTracing(ITestOutputHelper testOutput)
		{
			_traceSwitch = new ThresholdTraceSwitch(TraceLevel.Debug);
			_traceManager = new TraceManager(new TestOutputLogWriter<TraceEntry>(testOutput), _traceSwitch);
		}

		public TraceLevel TraceThreshold
		{
			get { return _traceSwitch.Threshold; }
			set { _traceSwitch.Threshold = value; }
		}

		public void Dispose()
		{
			_traceManager.Dispose();
		}

		/// <summary>
		/// Due to use of <see cref="TestOutputLogWriter{TEntry}"/> and <see cref="ITestOutputHelper"/> by this test class, all traces
		/// that pass the configured threshold are written to the test output buffer.
		/// </summary>
		[Fact]
		public void TestWithTraces()
		{
			var tracer = _traceManager.TracerFor(this);
			tracer.Info("Info message");
			tracer.Debug("Formatted debug message at UTC: {0}", DateTime.UtcNow);
		}
	}

}