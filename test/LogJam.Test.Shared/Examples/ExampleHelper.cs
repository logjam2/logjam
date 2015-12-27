// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExampleHelper.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal.UnitTests.Examples
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using LogJam.UnitTests.Examples;
    using LogJam.Writer;

    using Xunit.Abstractions;


    /// <summary>
    /// Shared test logic for using examples.
    /// </summary>
    public static class ExampleHelper
    {

        /// <summary>
        /// Writes some test messages in parallel threads.
        /// </summary>
        /// <param name="entryWriter"></param>
        /// <param name="messagesPerThread"></param>
        /// <param name="parallelThreads"></param>
        public static void LogTestMessagesInParallel(IEntryWriter<MessageEntry> entryWriter, int messagesPerThread, int parallelThreads, ITestOutputHelper testOutputHelper)
        {
            var stopwatch = Stopwatch.StartNew();

            Action loggingFunc = () => LogTestMessages(entryWriter, messagesPerThread);

            Parallel.Invoke(Enumerable.Repeat(loggingFunc, parallelThreads).ToArray());
            stopwatch.Stop();
            testOutputHelper.WriteLine("Logged {0} messages per thread in {1} parallel tasks in {2}", messagesPerThread, parallelThreads, stopwatch.Elapsed);
        }

        public static void LogTestMessages(IEntryWriter<MessageEntry> entryWriter, int messageCount)
        {
            for (int i = 0; i < messageCount; ++i)
            {
                var msg = new MessageEntry(i, "Message " + i);
                entryWriter.Write(ref msg);
            }
        }

    }

}
