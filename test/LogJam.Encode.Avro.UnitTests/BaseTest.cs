// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseTest.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode.Avro.UnitTests
{
    using System;

    using LogJam.Trace;
    using LogJam.Trace.Switches;
    using LogJam.XUnit2;

    using Xunit.Abstractions;


    /// <summary>
    /// Shared functionality for test classes.
    /// </summary>
    public abstract class BaseTest : IDisposable
    {

		private readonly ThresholdTraceSwitch _traceSwitch;
		private readonly TraceManager _traceManager;
        protected readonly ITestOutputHelper testOutput;
        protected readonly Tracer tracer;

        protected BaseTest(ITestOutputHelper testOutput)
		{
			_traceSwitch = new ThresholdTraceSwitch(TraceLevel.Debug);
			var logWriterConfig = new TestOutputLogWriterConfig(testOutput).UseTestTraceFormat();
			_traceManager = new TraceManager(logWriterConfig, _traceSwitch);
            tracer = _traceManager.TracerFor(this);
            this.testOutput = testOutput;
		}

		internal TraceLevel TraceThreshold
		{
			get { return _traceSwitch.Threshold; }
			set { _traceSwitch.Threshold = value; }
		}

		public void Dispose()
		{
			_traceManager.Dispose();
		}

    }

}