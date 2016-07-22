// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleTestTracing.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2.UnitTests
{
    using System;

    using LogJam.Trace;
    using LogJam.Trace.Format;
    using LogJam.Trace.Switches;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Exercises use of TestOutputFormatWriter.
    /// </summary>
    public sealed class ExampleTestTracing
    {

        private readonly ThresholdTraceSwitch _traceSwitch;
        private readonly TraceManager _traceManager;

        public ExampleTestTracing(ITestOutputHelper testOutput)
        {
            _traceSwitch = new ThresholdTraceSwitch(TraceLevel.Debug);
            _traceManager = new TraceManager();
            _traceManager.Config.TraceToTestOutput(testOutput,
                                                   traceSwitch: _traceSwitch);
        }

        public TraceLevel TraceThreshold { get { return _traceSwitch.Threshold; } set { _traceSwitch.Threshold = value; } }

        public void Dispose()
        {
            _traceManager.Dispose();
        }

        /// <summary>
        /// Due to use of <see cref="TestOutputFormatWriter" /> and <see cref="ITestOutputHelper" /> by this test class, all traces
        /// that pass the configured threshold are written to the test output buffer.
        /// </summary>
        [Fact]
        public void TestWithTraces()
        {
            var tracer = _traceManager.TracerFor(this);
            Assert.True(tracer.IsDebugEnabled());
            tracer.Info("Info message");
            tracer.Debug("Formatted debug message at UTC: {0}", DateTime.UtcNow);
        }

    }

}
