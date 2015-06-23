// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerTests.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Threading;

	using LogJam.Config;
	using LogJam.Format;
	using LogJam.Internal.UnitTests.Examples;
	using LogJam.Trace;
	using LogJam.UnitTests.Common;
	using LogJam.UnitTests.Examples;
	using LogJam.Writer;

	using Xunit;


	/// <summary>
	/// Unit tests for <see cref="LogManager"/>.
	/// </summary>
	public sealed class LogManagerTests
	{
		[Fact]
		public void DefaultLogManagerTracksAllLogJamOperationsToStatusTraces()
		{
			string testMessage = "test LogJam setup message";
			var traceLevel = TraceLevel.Info;

			using (var logManager = new LogManager())
			{
				// Trace a message
				var internalTracer = logManager.SetupTracerFactory.TracerFor(this);
				internalTracer.Trace(traceLevel, testMessage);

				// Verify the message can be found in LogJamTraceEntries
				var traceEntry = logManager.SetupLog.First(e => e.Message == testMessage);
				Assert.Equal(traceLevel, traceEntry.TraceLevel);
				Assert.Equal(internalTracer.Name, traceEntry.TracerName);
				int countTraces1 = logManager.SetupLog.Count();

				// Messing with the SetupLog doesn't force a Start()
				Assert.False(logManager.IsStarted);

				// Start and stop create trace records
				logManager.Start();
				int countTracesAfterStart = logManager.SetupLog.Count();
				Assert.True(countTracesAfterStart > countTraces1);

				logManager.Stop();
				int countTracesAfterStop = logManager.SetupLog.Count();
				Assert.True(countTracesAfterStop > countTracesAfterStart);
			}
		}

		[Fact(Skip = "Not implemented")]
		public void LogManager_internal_operations_can_be_logged_to_another_target()
		{	
		}

		[Fact]
		public void FinalizerCausesQueuedLogsToFlush()
		{
			var setupLog = new SetupLog();

			// Slow log writer - starting, stopping, disposing, writing an entry, all take at least 10ms each.
			const int opDelayMs = 5;
			var slowLogWriter = new SlowTestLogWriter<MessageEntry>(setupLog, opDelayMs, false);
			const int countLoggingThreads = 5;
			const int countMessagesPerThread = 5;
			const int expectedEntryCount = countLoggingThreads * countMessagesPerThread;

			// Run the test code in a delegate so that local variable references can be GCed
			Action logTestMessages = () =>
			                         {
				                         var logManager = new LogManager(new LogManagerConfig(), setupLog);
				                         logManager.Config.UseLogWriter(slowLogWriter).BackgroundLogging = true;
				                         var entryWriter = logManager.GetEntryWriter<MessageEntry>();

				                         ExampleHelper.LogTestMessagesInParallel(entryWriter, countMessagesPerThread, countLoggingThreads);

				                         // Key point: The LogManager is never disposed, and it has a number of queued
				                         // entries that haven't been written
			                         };
			logTestMessages();

			Assert.True(slowLogWriter.Count < expectedEntryCount);

			// Force a GC cyle, and wait for finalizers to complete.
			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.Equal(expectedEntryCount, slowLogWriter.Count);

			// When the finalizer is called, an error is logged to the SetupLog.
			Assert.True(setupLog.Any(traceEntry => (traceEntry.TraceLevel == TraceLevel.Error) && (traceEntry.Message.StartsWith("In finalizer "))));
		}


	}

}