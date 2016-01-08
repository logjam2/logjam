// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLoggingTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Writer.Text
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;


    /// <summary>
    /// Tests logging to the console.
    /// </summary>
    public sealed class ConsoleLoggingTests
    {

        private readonly ITestOutputHelper _testOutputHelper;

        public ConsoleLoggingTests(ITestOutputHelper testOutputHelper)
        {
            Contract.Requires<ArgumentNullException>(testOutputHelper != null);

            _testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Calls <c>LogJam.ConsoleTester.exe</c>, passing <paramref name="arguments" />.
        /// </summary>
        /// <param name="arguments"></param>
        private void RunConsoleTester(string arguments, out string stdout, out string stderr)
        {
            string exePath = GetType().Assembly.Location.Replace(@"\test\LogJam.UnitTests\", @"\test\LogJam.ConsoleTester\").Replace(".UnitTests.dll", ".ConsoleTester.exe");
            var process = new Process
                          {
                              StartInfo =
                              {
                                  FileName = exePath,
                                  Arguments = arguments,
                                  CreateNoWindow = true,
                                  UseShellExecute = false,
                                  RedirectStandardOutput = true,
                                  RedirectStandardError = true
                              }
                          };
            _testOutputHelper.WriteLine("Running LogJam.ConsoleTester {0}...", arguments);
            process.Start();

            Task<string> taskReadStdOut = process.StandardOutput.ReadToEndAsync();
            Task<string> taskReadStdErr = process.StandardError.ReadToEndAsync();
            process.WaitForExit(20000);

            if (process.ExitCode == -1)
            {
                throw new XunitException(taskReadStdErr.Result);
            }
            else if (process.ExitCode == -2)
            {
                throw new Exception(taskReadStdErr.Result);
            }

            stdout = taskReadStdOut.Result;
            stderr = taskReadStdErr.Result;
        }

        /// <summary>
        /// Shows reasonable use for tracing to stdout in unit tests.
        /// </summary>
        [Theory]
        [InlineData(ConfigForm.ObjectGraph)]
        [InlineData(ConfigForm.Fluent)]
        public void BasicConsoleTracing(ConfigForm configForm)
        {
            string stdout, stderr;
            string commands = string.Format("setup-trace-{0} setup-color simpletrace", configForm.ToString().ToLower());
            RunConsoleTester(commands, out stdout, out stderr);
            Assert.Matches(@"Info\s+LJ\.CT\.ConsoleTestCases\s+By default info is enabled\r\n", stdout);
            Assert.Empty(stderr);
        }

        [Theory]
        [InlineData(ConfigForm.ObjectGraph)]
        [InlineData(ConfigForm.Fluent)]
        public void BasicConsoleTracingWithDebug(ConfigForm configForm)
        {
            string stdout, stderr;
            string commands = string.Format("setup-trace-{0}-all-levels setup-color trace-debug", configForm.ToString().ToLower());
            RunConsoleTester(commands, out stdout, out stderr);
            Assert.Matches(@"Debug\s+LJ\.CT\.ConsoleTestCases\s+Debug is enabled for this class\r\n", stdout);
            Assert.Empty(stderr);
        }

        [Theory]
        [InlineData(ConfigForm.ObjectGraph)]
        [InlineData(ConfigForm.Fluent)]
        public void TraceWithTimestampsToConsole(ConfigForm configForm)
        {
            string stdout, stderr;
            string commands = string.Format("setup-trace-{0} trace-timestamps setup-color simpletrace", configForm.ToString().ToLower());
            RunConsoleTester(commands, out stdout, out stderr);
            Assert.Matches(@"\d{2}:\d{2}:\d{2}\.\d{3}\s+Info\s+LJ\.CT\.ConsoleTestCases\s+By default info is enabled\r\n", stdout);
            Assert.Empty(stderr);
        }

        [Theory]
        [InlineData(ConfigForm.ObjectGraph)]
        [InlineData(ConfigForm.Fluent)]
        public void TraceAllLevelsToConsole(ConfigForm configForm)
        {
            string stdout, stderr;
            string commands = string.Format("setup-trace-{0}-all-levels trace-timestamps setup-color trace-all-levels", configForm.ToString().ToLower());
            RunConsoleTester(commands, out stdout, out stderr);
            Assert.Contains("Verbose message\r\n", stdout);
            Assert.Contains("Debug message\r\n", stdout);
            Assert.Empty(stderr);
        }

        [Fact]
        public void TraceExceptionToConsole()
        {
            string stdout, stderr;
            RunConsoleTester("setup-trace-fluent setup-color warn-exception", out stdout, out stderr);
            Assert.Matches(@"Warn\s+LJ\.CT\.ConsoleTestCases\s+Warning exception\r\n", stdout);
            Assert.Empty(stderr);
        }

    }

}
