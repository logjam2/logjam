// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharedTestFixtureTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2.UnitTests
{
    using System;
    using System.Diagnostics.Contracts;

    using LogJam.Shared.Internal;
    using LogJam.Trace;
    using LogJam.Trace.Config;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// This test handles the case where multiple test cases share a fixture, and the fixture holds the <see cref="LogManager"/>.
    /// In such cases, the <see cref="ITestOutputHelper"/> is changed for each test, b/c each test has its own test output.
    /// </summary>
    public sealed class SharedTestFixtureTests : IClassFixture<SharedTestFixtureTests.TestFixture>
    {

        private readonly TestFixture _fixture;

        public SharedTestFixtureTests(TestFixture fixture)
        {
            Arg.NotNull(fixture, nameof(fixture));

            _fixture = fixture;
        }

        [Theory]
        [InlineData("message1")]
        [InlineData("message2")]
        public void EachTestCanHaveItsOwnOutput(string traceMessage)
        {
            var testOutput = new FakeTestOutputHelper();
            _fixture.TestOutputAccessor.TestOutput = testOutput;

            var tracer = _fixture.TraceManager.GetTracer("tracerName");
            tracer.Info(traceMessage);

            // Clear the test output
            _fixture.TestOutputAccessor.TestOutput = null;

            var testOutputLines = testOutput.GetTestOutput().Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            Assert.Single(testOutputLines);
            Assert.Equal(traceMessage, testOutputLines[0]);
        }

        /// <summary>
        /// A test fixture shared between multiple tests.
        /// </summary>
        public class TestFixture : IDisposable
        {

            public TraceManager TraceManager { get; }
            public ITestOutputAccessor TestOutputAccessor { get; }

            public TestFixture()
            {
                var testOutputConfig = new TestOutputLogWriterConfig() // No ITestOutputHelper is passed in; it's set later by each test
                                       {
                                           BackgroundLogging = false
                                       };
                TraceManager = new TraceManager(testOutputConfig);

                // Use very simple formatting b/c it's easier to test
                testOutputConfig.Format<TraceEntry>((traceEntry, formatWriter) =>
                                                    {
                                                        formatWriter.BeginEntry();
                                                        formatWriter.WriteField(traceEntry.Message);
                                                        formatWriter.EndEntry();
                                                    });

                TestOutputAccessor = testOutputConfig.TestOutputAccessor;
            }

            public void Dispose()
            {
                TraceManager.Dispose();
            }

        }
    }

}
