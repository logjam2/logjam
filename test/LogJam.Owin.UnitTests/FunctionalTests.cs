// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionalTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.UnitTests
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Owin;

    using LogJam.Config;
    using LogJam.Owin.Http;
    using LogJam.Trace;
    using LogJam.Trace.Config;
    using LogJam.Trace.Format;
    using LogJam.Trace.Switches;

    using Microsoft.Owin.Testing;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Functional tests for <see cref="LogJam.Owin" />.
    /// </summary>
    public sealed class FunctionalTests : BaseOwinTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FunctionalTests(ITestOutputHelper testOutputHelper)
        {
            Contract.Requires<ArgumentNullException>(testOutputHelper != null);

            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void SingleRequestWithTracing()
        {
            var setupLog = new SetupLog();
            var stringWriter = new StringWriter();
            using (TestServer testServer = CreateTestServer(stringWriter, setupLog))
            {
                IssueTraceRequest(testServer, 2);
            }

            _testOutputHelper.WriteLine(stringWriter.ToString());

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

            _testOutputHelper.WriteLine(stringWriter.ToString());

            Assert.NotEmpty(setupLog);
            Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
        }

        [Fact]
        public void SingleRequestWithTracingWith3LogTargets()
        {
            var setupLog = new SetupLog();
            var stringWriter = new StringWriter();
            using (TestServer testServer = new OwinTestWith3LogTargets().CreateTestServer(stringWriter, setupLog, true))
            {
                IssueTraceRequest(testServer, 2);
            }
            _testOutputHelper.WriteLine("Logging complete.");

            _testOutputHelper.WriteLine(string.Empty);
            _testOutputHelper.WriteLine("StringWriter contents:");
            _testOutputHelper.WriteLine(stringWriter.ToString());

            Assert.NotEmpty(setupLog);

			_testOutputHelper.WriteEntries(setupLog);
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

            _testOutputHelper.WriteLine(stringWriter.ToString());

            Assert.NotEmpty(setupLog);
            Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
        }


        /// <summary>
        /// Sets up a TestServer with more complex logging, same handlers as in BaseOwinTest
        /// </summary>
        private class OwinTestWith3LogTargets : BaseOwinTest
        {

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

                // Log to textwriter without trace timestamps (default)
                var textLogConfig0 = logManagerConfig.UseTextWriter(new StringWriter());
                textLogConfig0.BackgroundLogging = backgroundThreadLogging;

                // Log to TextWriter with Trace timestamps
                var textLogConfig1 = logManagerConfig.UseTextWriter(logTarget)
                                                    .Format(new DefaultTraceFormatter()
                                                            {
                                                                IncludeTimestamp = true
                                                            });
                traceManagerConfig.TraceTo(textLogConfig1, traceSwitches);

                appBuilder.TraceTo(new ILogWriterConfig[] {debuggerConfig, textLogConfig0}, traceSwitches);
                appBuilder.LogHttpRequestsToAll();
                appBuilder.UseOwinTracerLogging();
            }

        }

    }

}
