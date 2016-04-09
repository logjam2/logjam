// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinSetupTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using global::Owin;

    using LogJam.Config;
    using LogJam.Owin.Http;
    using LogJam.Trace;
    using LogJam.Trace.Config;
    using LogJam.XUnit2;

    using Microsoft.Owin.Testing;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Exercises different <see cref="LogJam.Owin"/> setup use-cases.
    /// </summary>
    public sealed class OwinSetupTests
    {

        private readonly ITestOutputHelper _testOutput;

        public OwinSetupTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public async Task SettingUpHttpLoggingTwiceWorks()
        {
            LogManager logManager = null;

            // Log targets
            var stringWriter = new StringWriter();
            var listRequestEntries = new List<HttpRequestEntry>();
            var listResponseEntries = new List<HttpResponseEntry>();
            var listTraceEntries = new List<TraceEntry>();

            var testServer = TestServer.Create(owinAppBuilder =>
                                               {
                                                   // Save this for asserts below
                                                   logManager = owinAppBuilder.GetLogManager();

                                                   var logManagerConfig = owinAppBuilder.GetLogManagerConfig();
                                                   logManagerConfig.UseTextWriter(stringWriter);
                                                   logManagerConfig.UseTestOutput(_testOutput);
                                                   Assert.Equal(2, logManagerConfig.Writers.Count);

                                                   // Enable HTTP logging #1
                                                   owinAppBuilder.LogHttpRequestsToAll();                                                   

                                                   // Add ListLogWriters for requests and responses
                                                   var requestListConfig = new ListLogWriterConfig<HttpRequestEntry>()
                                                                           {
                                                                               List = listRequestEntries
                                                                           };
                                                   var responseListConfig = new ListLogWriterConfig<HttpResponseEntry>()
                                                                            {
                                                                                List = listResponseEntries
                                                                            };
                                                   logManagerConfig.Writers.Add(requestListConfig);
                                                   logManagerConfig.Writers.Add(responseListConfig);

                                                   // Enable HTTP logging #2
                                                   owinAppBuilder.LogHttpRequests(new ILogWriterConfig[] { requestListConfig, responseListConfig });

                                                   // Trace to list, too
                                                   owinAppBuilder.GetTraceManagerConfig().TraceToList(listTraceEntries);
                                               });
            using (testServer)
            {
                // Single request, expect a 404 as no handler was setup
                var response = await testServer.HttpClient.GetAsync("no-page-here.html");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

                // Second request, expect a 404 as no handler was setup
                response = await testServer.HttpClient.GetAsync("no-page-here2.html");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            // There should be no setup errors
            Assert.Empty(logManager.SetupLog.Where(traceEntry => traceEntry.TraceLevel > TraceLevel.Info));

            Assert.NotEmpty(listRequestEntries);
            Assert.NotEmpty(listResponseEntries);

            Assert.Equal(404, listResponseEntries.First().HttpStatusCode);
        }

    }

}