// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.ConsoleTester
{
    using System;
    using System.Linq;

    using LogJam.Trace;

    using Xunit;
    using Xunit.Sdk;


    /// <summary>
    /// Command-line app for console testing.
    /// </summary>
    public sealed class App
    {

        private static int Main(string[] args)
        {
            if (args.Contains("console-write"))
            {
                Console.WriteLine("Started LogJam.ConsoleTester.");
            }

            try
            {
                using (var traceManager = new TraceManager())
                {
                    var testSetup = new ConsoleTestSetup(traceManager);
                    if (args.Contains("setup-trace-objectgraph"))
                    {
                        testSetup.ObjectGraphConfigForTrace();
                    }
                    else if (args.Contains("setup-trace-fluent"))
                    {
                        testSetup.FluentConfigForTrace();
                    }
                    else if (args.Contains("setup-trace-objectgraph-all-levels"))
                    {
                        testSetup.ObjectGraphConfigForTraceEnableAllLevels();
                    }
                    else if (args.Contains("setup-trace-fluent-all-levels"))
                    {
                        testSetup.FluentConfigForTraceEnableAllLevels();
                    }

                    testSetup.TraceTimestamps(args.Contains("trace-timestamps"));

                    if (args.Contains("setup-color"))
                    {
                        if (testSetup.ConsoleLogWriterConfig != null)
                        {
                            testSetup.ConsoleLogWriterConfig.UseColor = true;
                        }
                    }

                    var tests = new ConsoleTestCases(traceManager);
                    if (args.Contains("simpletrace"))
                    {
                        tests.SimpleTrace();
                    }
                    if (args.Contains("trace-debug"))
                    {
                        tests.TraceDebug(true);
                    }
                    if (args.Contains("trace-all-levels"))
                    {
                        tests.TraceAllLevels();
                    }
                    if (args.Contains("warn-exception"))
                    {
                        tests.WarnException();
                    }

                    Assert.True(traceManager.IsHealthy);

                    if (args.Contains("dump-setuplog"))
                    {
                        traceManager.SetupLog.WriteEntriesTo(Console.Out);
                    }
                }

                if (args.Contains("console-write"))
                {
                    Console.WriteLine("Exiting LogJam.ConsoleTester...");
                }

                return 0;
            }
            catch (XunitException xunitException)
            {
                Console.Error.WriteLine("XUnit Exception caught in LogJam.ConsoleTester:");
                Console.Error.WriteLine(xunitException);
                return -1;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Exception caught in LogJam.ConsoleTester:");
                Console.Error.WriteLine(exception);
                return -2;
            }
        }

    }

}
