// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerfTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.UnitTests
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Owin.Testing;

    using Xunit;
    using Xunit.Abstractions;


    using TraceLevel = LogJam.Trace.TraceLevel;


    /// <summary>
    /// Runs perf test on standard <see cref="LogJam.Owin" /> setup.
    /// </summary>
    public sealed class PerfTests : BaseOwinTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PerfTests(ITestOutputHelper testOutputHelper)
        {
            Contract.Requires<ArgumentNullException>(testOutputHelper != null);

            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(4, 1000, 0)]
        [InlineData(4, 1000, 0)]
        [InlineData(4, 1000, 10)]
        //[InlineData(40, 1000, 30)]
        public void ParallelTraceTest(int threads, int requestsPerThread, int tracesPerRequest)
        {
            var setupLog = new SetupLog();
            Stopwatch overallStopwatch = Stopwatch.StartNew();

            // Test logging to TextWriter.Null - which should have no perf overhead
            using (TestServer testServer = CreateTestServer(TextWriter.Null, setupLog))
            {

                Action testThread = () =>
                                    {
                                        int threadId = Thread.CurrentThread.ManagedThreadId;
                                        _testOutputHelper.WriteLine("{0}: Starting requests on thread {1}", overallStopwatch.Elapsed, threadId);
                                        var stopWatch = Stopwatch.StartNew();

                                        for (int i = 0; i < requestsPerThread; ++i)
                                        {
                                            IssueTraceRequest(testServer, tracesPerRequest);
                                        }
                                        stopWatch.Stop();

                                        _testOutputHelper.WriteLine("{0}: Completed {1} requests on thread {2} in {3}",
                                                          overallStopwatch.Elapsed,
                                                          requestsPerThread,
                                                          threadId,
                                                          stopWatch.Elapsed);
                                    };

                Parallel.Invoke(Enumerable.Repeat(testThread, threads).ToArray());
            }

            Assert.NotEmpty(setupLog);
            Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
        }

    }

}
