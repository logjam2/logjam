// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadHelper.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
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


	/// <summary>
	/// Shared test logic for applying load (eg logging).
	/// </summary>
	public static class LoadHelper
	{

		/// <summary>
		/// Writes some <see cref="MessageEntry"/>s in parallel threads.
		/// </summary>
		/// <param name="entryWriter"></param>
		/// <param name="messagesPerThread"></param>
		/// <param name="parallelThreads"></param>
		public static void LogTestMessagesInParallel(IEntryWriter<MessageEntry> entryWriter, int messagesPerThread, int parallelThreads)
		{
			var stopwatch = Stopwatch.StartNew();

			Action loggingFunc = () => LogTestMessages(entryWriter, messagesPerThread);

			Parallel.Invoke(Enumerable.Repeat(loggingFunc, parallelThreads).ToArray());
			stopwatch.Stop();
			Console.WriteLine("Logged {0} messages per thread in {1} parallel tasks in {2}", messagesPerThread, parallelThreads, stopwatch.Elapsed);
		}

		public static void LogTestMessages(IEntryWriter<MessageEntry> entryWriter, int messageCount)
		{
			for (int i = 0; i < messageCount; ++i)
			{
				var msg = new MessageEntry(i, "Message " + i);
				entryWriter.Write(ref msg);
			}
		}

		/// <summary>
		/// Writes some <see cref="TestEntry"/>s in parallel threads.
		/// </summary>
		/// <param name="entryWriter"></param>
		/// <param name="messagesPerThread"></param>
		/// <param name="parallelThreads"></param>
		public static void LogTestEntriesInParallel(IEntryWriter<TestEntry> entryWriter, int messagesPerThread, int parallelThreads)
		{
			int counter = 0;
			var stopwatch = Stopwatch.StartNew();

			Action loggingFunc = () => LogTestEntries(ref counter, entryWriter, messagesPerThread);

			Parallel.Invoke(Enumerable.Repeat(loggingFunc, parallelThreads).ToArray());
			stopwatch.Stop();
			Console.WriteLine("Logged {0} test entries per thread in {1} parallel tasks in {2}", messagesPerThread, parallelThreads, stopwatch.Elapsed);
		}

		public static void LogTestEntries(ref int counter, IEntryWriter<TestEntry> entryWriter, int messageCount)
		{
			for (int i = 0; i < messageCount; ++i)
			{
				var msg = new TestEntry(ref counter);
				entryWriter.Write(ref msg);
			}
		}

	}

}