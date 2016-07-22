// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionalTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Owin;

    using LogJam.Config;
    using LogJam.Test.Shared;
    using LogJam.Trace;
    using LogJam.Trace.Config;
    using LogJam.Trace.Format;
    using LogJam.Trace.Switches;
    using LogJam.XUnit2;

    using Microsoft.Owin.Testing;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Functional tests for <see cref="LogJam.Owin" />.
    /// </summary>
    public sealed class OwinFunctionalTests : BaseOwinTest
    {

        public OwinFunctionalTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {}

        [Fact]
        public void SingleRequestWithTracing()
        {
            var setupLog = new SetupLog();
            var stringWriter = new StringWriter();
            using (TestServer testServer = CreateTestServer(stringWriter, setupLog))
            {
                IssueTraceRequest(testServer, 2);
            }

            testOutputHelper.WriteLine(stringWriter.ToString());

            Assert.NotEmpty(setupLog);
            Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
        }

        [Fact]
        public void ParallelRequestsWithTracing()
        {
            var setupLog = new SetupLog();
            var stringWriter = new StringWriter();
            using (TestServer testServer = CreateTestServer(stringWriter, setupLog))
            {
                // 10 parallel threads, each thread issues 2 requests, each request traces 2x
                Action issueRequestAction = () =>
                                            {
                                                IssueTraceRequest(testServer, 2);
                                                IssueTraceRequest(testServer, 2);
                                            };
                Parallel.Invoke(Enumerable.Repeat(issueRequestAction, 10).ToArray());
            }

            string logOutput = stringWriter.ToString();
            testOutputHelper.WriteLine(logOutput);

            int countLineBreaks = TestHelper.CountLineBreaks(logOutput);
            Assert.True(countLineBreaks > 140 && countLineBreaks < 180);

            Assert.NotEmpty(setupLog);
            Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
        }

        [Fact]
        public void SingleRequestWithTracingWith3LogTargets()
        {
            var setupLog = new SetupLog();
            var stringWriter = new StringWriter();
            using (TestServer testServer = new OwinTestWith3LogTargets(testOutputHelper).CreateTestServer(stringWriter, setupLog, true))
            {
                IssueTraceRequest(testServer, 2);
            }
            testOutputHelper.WriteLine("Logging complete.");

            testOutputHelper.WriteLine("");
            testOutputHelper.WriteLine("StringWriter contents:");
            testOutputHelper.WriteLine(stringWriter.ToString());

            Assert.NotEmpty(setupLog);

            testOutputHelper.WriteLine("");
            testOutputHelper.WriteLine("Setup log:");
            testOutputHelper.WriteEntries(setupLog);
            Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
        }

        [Fact]
        public void ExceptionTracing()
        {
            var setupLog = new SetupLog();
            var stringWriter = new StringWriter();
            using (TestServer testServer = CreateTestServer(stringWriter, setupLog))
            {
                var task = testServer.CreateRequest("/exception").GetAsync();
                var response = task.Result; // Wait for the call to complete
            }

            testOutputHelper.WriteLine(stringWriter.ToString());

            Assert.NotEmpty(setupLog);
            Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
        }


        /// <summary>
        /// Sets up a TestServer with more complex logging, same handlers as in BaseOwinTest
        /// </summary>
        private class OwinTestWith3LogTargets : BaseOwinTest
        {

            public OwinTestWith3LogTargets(ITestOutputHelper testOutputHelper)
                : base(testOutputHelper)
            {}

            protected override void ConfigureLogging(IAppBuilder appBuilder, TextWriter logTarget, bool backgroundThreadLogging)
            {
                var logManagerConfig = appBuilder.GetLogManagerConfig();
                var traceManagerConfig = appBuilder.GetTraceManagerConfig();
                var traceSwitches = new SwitchSet()
                                    {
                                        { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Verbose) }
                                    };

                // Log to debugger
                var debuggerConfig = logManagerConfig.UseDebugger();
                debuggerConfig.BackgroundLogging = backgroundThreadLogging;

                // Log to xunit without trace timestamps (default)
                var testOutputConfig = logManagerConfig.UseTestOutput(testOutputHelper);
                testOutputConfig.BackgroundLogging = backgroundThreadLogging;

                // Log to TextWriter with Trace timestamps
                var textLogConfig = logManagerConfig.UseTextWriter(logTarget)
                                                    .Format(new DefaultTraceFormatter());

                traceManagerConfig.TraceTo(new ILogWriterConfig[] { debuggerConfig, testOutputConfig, textLogConfig }, traceSwitches);
                appBuilder.LogHttpRequestsToAll();
                appBuilder.UseOwinTracerLogging();
            }

        }

    }

}
