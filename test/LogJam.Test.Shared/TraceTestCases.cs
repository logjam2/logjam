// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceTestCases.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Test.Shared
{
    using LogJam.Trace;

    using Xunit;


    /// <summary>
    /// Some tracing test cases that can be used in multiple projects.
    /// </summary>
    public abstract class TraceTestCases
    {

        protected TraceTestCases(TraceManager traceManager)
        {
            TraceManager = traceManager;
        }

        public TraceManager TraceManager { get; set; }

        [Fact]
        public void SimpleTrace()
        {
            // Default threshold: Info
            var tracer = TraceManager.TracerFor(this);

            Assert.True(TraceManager.IsStarted); // Getting a tracer starts the TraceManager
            Assert.True(TraceManager.LogManager.IsStarted);

            Assert.True(tracer.IsInfoEnabled());
            Assert.False(tracer.IsVerboseEnabled());

            tracer.Info("By default info is enabled");
            tracer.Verbose("This message won't be logged");
        }

        public void TraceDebug(bool assertDebugEnabled)
        {
            var tracer = TraceManager.TracerFor(this);
            if (assertDebugEnabled)
            {
                Assert.True(tracer.IsVerboseEnabled());
                Assert.True(tracer.IsDebugEnabled());
            }

            tracer.Debug("Debug is enabled for this class");
        }

        [Fact]
        public void TraceAllLevels()
        {
            var tracer = TraceManager.TracerFor(this);
            tracer.Severe("Severe message");
            tracer.Error("Error message");
            tracer.Warn("Warning message");

            TestHelper.WarnException(tracer, 5);

            tracer.Info("Info message");
            tracer.Verbose("Verbose message");
            tracer.Debug("Debug message");
        }

        [Fact]
        public void WarnException()
        {
            var tracer = TraceManager.TracerFor(this);
            TestHelper.WarnException(tracer, 4);
        }

    }

}
