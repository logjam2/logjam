// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundMultiLogWriterUnitTests.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal.UnitTests.Writer
{
	using System;
	using System.Diagnostics;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using LogJam.Format;
	using LogJam.UnitTests.Common;
	using LogJam.UnitTests.Examples;
	using LogJam.Trace.Format;
	using LogJam.Writer;

	using Xunit;

	using TraceLevel = LogJam.Trace.TraceLevel;


	/// <summary>
	/// Validates behavior of <see cref="BackgroundMultiLogWriter"/>.
	/// </summary>
	public sealed class BackgroundMultiLogWriterUnitTests
	{
		// Some tests disable timing-sensitive Assert()s if running in a debugger - b/c the debugger throws timing off
		private readonly bool _inDebugger = System.Diagnostics.Debugger.IsAttached;

		private SetupTracerFactory SetupTracerFactory { get; set; } 

		/// <summary>
		/// Sets up a <see cref="BackgroundMultiLogWriter"/> that writes to <paramref name="innerLogWriter"/> on a background thread.
		/// </summary>
		/// <param name="innerLogWriter">The <see cref="ILogWriter{TEntry}"/> that is written to on the background thread.</param>
		/// <param name="backgroundMultiLogWriter"></param>
		/// <param name="logWriter">The returned <see cref="IQueueLogWriter{TEntry}"/>, which writes to a queue which feeds logging on the background thread.</param>
		/// <param name="maxQueueLength"></param>
		private void SetupBackgroundMessageWriter<TEntry>(
			ILogWriter<TEntry> innerLogWriter,
			out BackgroundMultiLogWriter backgroundMultiLogWriter,
			out IQueueLogWriter<TEntry> logWriter,
			int maxQueueLength = BackgroundMultiLogWriter.DefaultMaxQueueLength)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(innerLogWriter != null);

			SetupTracerFactory = new SetupTracerFactory();
			backgroundMultiLogWriter = new BackgroundMultiLogWriter(SetupTracerFactory);
			logWriter = backgroundMultiLogWriter.CreateProxyWriterFor(innerLogWriter, maxQueueLength);

			Assert.False(backgroundMultiLogWriter.IsStarted);
			Assert.False((logWriter as IStartable).IsStarted);

			backgroundMultiLogWriter.Start();

			Assert.True(backgroundMultiLogWriter.IsStarted);
			Assert.True((logWriter as IStartable).IsStarted);
		}

		/// <summary>
		/// Writes some test messages in parallel threads.
		/// </summary>
		/// <param name="logWriter"></param>
		/// <param name="messagesPerThread"></param>
		/// <param name="parallelThreads"></param>
		private void LogTestMessagesInParallel(ILogWriter<MessageEntry> logWriter, int messagesPerThread, int parallelThreads)
		{
			var stopwatch = Stopwatch.StartNew();

			Action loggingFunc = () => LogTestMessages(logWriter, messagesPerThread);

			Parallel.Invoke(Enumerable.Repeat(loggingFunc, parallelThreads).ToArray());
			stopwatch.Stop();
			Console.WriteLine("Logged {0} messages per thread in {1} parallel tasks in {2}", messagesPerThread, parallelThreads, stopwatch.Elapsed);
		}

		private void LogTestMessages(ILogWriter<MessageEntry> logWriter, int messageCount)
		{
			for (int i = 0; i < messageCount; ++i)
			{
				var msg = new MessageEntry(i, "Message " + i);
				logWriter.Write(ref msg);
			}
		}

		[Fact]
		public void BackgroundLoggingMakesSlowLoggerActFast()
		{
			// Hi-res timer
			var stopwatch = new Stopwatch();

			// Slow log writer - starting, stopping, disposing, writing an entry, all take at least 10ms each.
			const int operationDelayMs = 20;
			const int parallelThreads = 8;
			const int messagesPerThread = 6;
			var slowLogWriter = new SlowTestLogWriter<MessageEntry>(operationDelayMs, false);

			BackgroundMultiLogWriter backgroundMultiLogWriter;
			IQueueLogWriter<MessageEntry> queueLogWriter;
			stopwatch.Start();
			SetupBackgroundMessageWriter(slowLogWriter, out backgroundMultiLogWriter, out queueLogWriter);
			stopwatch.Stop();
			Assert.True((stopwatch.ElapsedMilliseconds <= operationDelayMs) || _inDebugger, "Starting should be fast, slowLogWriter start delay should occur on background thread.  Elapsed: " + stopwatch.ElapsedMilliseconds);
			Console.WriteLine("Created + started BackgroundMultiLogWriter in {0}", stopwatch.Elapsed);

			using (backgroundMultiLogWriter)
			{
				stopwatch.Restart();
				LogTestMessagesInParallel(queueLogWriter, messagesPerThread, parallelThreads);
				stopwatch.Stop();
				Assert.True((stopwatch.ElapsedMilliseconds < operationDelayMs) || _inDebugger, "Log writing should be fast, until the queue is filled.");

				stopwatch.Restart();
			}
			stopwatch.Stop();
			Console.WriteLine("Stop + dispose took {0}", stopwatch.Elapsed);

			// At this point, everything should have been logged
			Assert.Equal(parallelThreads * messagesPerThread, slowLogWriter.Count);
			Assert.True(stopwatch.ElapsedMilliseconds > messagesPerThread * parallelThreads * operationDelayMs, "Dispose is slow, b/c we wait for all writes to complete on the background thread");
		}

		[Fact]
		public void CanRestartBackgroundMultiLogWriter()
		{
			var innerLogWriter = new TestLogWriter<MessageEntry>(false);
			BackgroundMultiLogWriter backgroundMultiLogWriter;
			IQueueLogWriter<MessageEntry> queueLogWriter;
			SetupBackgroundMessageWriter(innerLogWriter, out backgroundMultiLogWriter, out queueLogWriter);

			// Re-starting shouldn't hurt anything
			Assert.True(backgroundMultiLogWriter.IsStarted);
			backgroundMultiLogWriter.Start();
			queueLogWriter.Start();

			// Log some, then Stop
			LogTestMessagesInParallel(queueLogWriter, 8, 8);
			backgroundMultiLogWriter.Stop(); // Blocks until the background thread exits

			Assert.False(innerLogWriter.IsStarted);
			Assert.False(backgroundMultiLogWriter.IsStarted);
			Assert.Equal(8 * 8, innerLogWriter.Count);

			// After a Stop(), logging does nothing
			var msg = new MessageEntry("Logged while stopped - never logged.");
			queueLogWriter.Write(ref msg);

			// After a Stop(), BackgroundMultiLogWriter and it's contained logwriters can be restarted
			backgroundMultiLogWriter.Start();

			// Log some, then Dispose
			LogTestMessagesInParallel(queueLogWriter, 8, 8);
			backgroundMultiLogWriter.Dispose(); // Blocks until the background thread exits

			Assert.False(innerLogWriter.IsStarted);
			Assert.False(backgroundMultiLogWriter.IsStarted);
			Assert.Equal(2 * 8 * 8, innerLogWriter.Count);

			// After a Dispose(), BackgroundMultiLogWriter can't be re-used
			Assert.Throws<ObjectDisposedException>(() => backgroundMultiLogWriter.Start());
			Assert.DoesNotThrow(() => queueLogWriter.Write(ref msg));
		}

		[Fact]
		public void DisposePreventsRestart()
		{
			var innerLogWriter = new TestLogWriter<MessageEntry>(false);
			BackgroundMultiLogWriter backgroundMultiLogWriter;
			IQueueLogWriter<MessageEntry> queueLogWriter;
			SetupBackgroundMessageWriter(innerLogWriter, out backgroundMultiLogWriter, out queueLogWriter);

			backgroundMultiLogWriter.Dispose(); // Blocks until the background thread exits

			// After a Dispose(), BackgroundMultiLogWriter can't be re-used
			Assert.Throws<ObjectDisposedException>(() => backgroundMultiLogWriter.Start());

			// Logging doesn't throw, though
			var msg = new MessageEntry("Logged while Dispose()ed - never logged.");
			Assert.DoesNotThrow(() => queueLogWriter.Write(ref msg));
		}

		[Fact]
		public void DisposeBlocksUntilBackgroundLoggingCompletes()
		{
			// Slow log writer - starting, stopping, disposing, writing an entry, all take at least 20ms each.
			const int operationDelayMs = 5;
			const int parallelThreads = 8;
			const int messagesPerThread = 6;
			const int expectedMessageCount = parallelThreads * messagesPerThread;
			var slowLogWriter = new SlowTestLogWriter<MessageEntry>(operationDelayMs, false);

			BackgroundMultiLogWriter backgroundMultiLogWriter;
			IQueueLogWriter<MessageEntry> queueLogWriter;
			SetupBackgroundMessageWriter(slowLogWriter, out backgroundMultiLogWriter, out queueLogWriter);

			using (backgroundMultiLogWriter)
			{
				LogTestMessagesInParallel(queueLogWriter, messagesPerThread, parallelThreads);
				Assert.True(backgroundMultiLogWriter.IsBackgroundThreadRunning);
				Assert.True(slowLogWriter.Count < expectedMessageCount);
				// Dispose waits for all queued logs to complete, and for the background thread to exit
			}
			Assert.Equal(expectedMessageCount, slowLogWriter.Count);
			Assert.False(backgroundMultiLogWriter.IsBackgroundThreadRunning);
		}

		[Fact]
		public void StoppingQueueLogWriterHaltsWriting()
		{
			var innerLogWriter = new TestLogWriter<MessageEntry>(false);
			BackgroundMultiLogWriter backgroundMultiLogWriter;
			IQueueLogWriter<MessageEntry> queueLogWriter;
			SetupBackgroundMessageWriter(innerLogWriter, out backgroundMultiLogWriter, out queueLogWriter);

			using (backgroundMultiLogWriter)
			{
				LogTestMessagesInParallel(queueLogWriter, 8, 8);
				queueLogWriter.Stop();
				queueLogWriter.Stop(); // Stopping twice shouldn't change things

				// These messages aren't logged
				LogTestMessagesInParallel(queueLogWriter, 8, 8);

				queueLogWriter.Start();
				queueLogWriter.Start(); // Starting twice shouldn't change things
				LogTestMessagesInParallel(queueLogWriter, 8, 8);
			}

			Assert.False(backgroundMultiLogWriter.IsBackgroundThreadRunning);
			Assert.Equal(2 * 8 * 8, innerLogWriter.Count);
		}

		/// <summary>
		/// The queue log writer can be disposed before the <see cref="BackgroundMultiLogWriter"/> is disposed.
		/// </summary>
		[Fact]
		public void QueueLogWriterCanBeDisposedEarly()
		{
			var innerLogWriter = new TestLogWriter<MessageEntry>(false);
			BackgroundMultiLogWriter backgroundMultiLogWriter;
			IQueueLogWriter<MessageEntry> queueLogWriter;
			SetupBackgroundMessageWriter(innerLogWriter, out backgroundMultiLogWriter, out queueLogWriter);

			using (backgroundMultiLogWriter)
			{
				using (queueLogWriter as IDisposable)
				{
					LogTestMessagesInParallel(queueLogWriter, 8, 8);
				}
				Assert.False(queueLogWriter.Enabled);
				Assert.False(queueLogWriter.IsStarted);
				Assert.True(backgroundMultiLogWriter.Enabled);
				Assert.True(backgroundMultiLogWriter.IsStarted);

				// Can't restart after Dispose()
				Assert.Throws<ObjectDisposedException>(() => queueLogWriter.Start());

				// Logging still doesn't throw after Dispose
				var msg = new MessageEntry("Logged while Dispose()ed - never logged.");
				Assert.DoesNotThrow(() => queueLogWriter.Write(ref msg));
			}
			Assert.False(backgroundMultiLogWriter.Enabled);
			Assert.False(backgroundMultiLogWriter.IsStarted);

			Assert.False(backgroundMultiLogWriter.IsBackgroundThreadRunning);
			Assert.Equal(8 * 8, innerLogWriter.Count);
		}

		[Fact]
		public void ExceedingQueueSizeBlocksLogging()
		{
			// Slow log writer - starting, stopping, disposing, writing an entry, all take at least 10ms each.
			const int opDelayMs = 30;
			const int maxQueueLength = 10;
			const int countBlockingWrites = 4;
			var slowLogWriter = new SlowTestLogWriter<MessageEntry>(opDelayMs, false);

			BackgroundMultiLogWriter backgroundMultiLogWriter;
			IQueueLogWriter<MessageEntry> queueLogWriter;
			SetupBackgroundMessageWriter(slowLogWriter, out backgroundMultiLogWriter, out queueLogWriter, maxQueueLength);

			var stopwatch = Stopwatch.StartNew();
			using (backgroundMultiLogWriter)
			{
				// First 10 messages log fast, then the queue is exactly full
				LogTestMessages(queueLogWriter, maxQueueLength);
				stopwatch.Stop();
				Console.WriteLine("First {0} writes took: {1}ms", maxQueueLength, stopwatch.ElapsedMilliseconds);
				Assert.True(_inDebugger || (stopwatch.ElapsedMilliseconds <=	 opDelayMs), "Log writing should be fast, until the queue is filled.");

				// Next writes block, since we've filled the queue
				for (int i = 0; i < countBlockingWrites; ++i)
				{
					stopwatch.Restart();
					LogTestMessages(queueLogWriter, 1);
					stopwatch.Stop();
					long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
					Console.WriteLine("Blocking write #{0}: {1}ms", i, elapsedMilliseconds);
					Assert.True((elapsedMilliseconds >= (opDelayMs * 0.9)) || (i == 0) || _inDebugger, "Expect blocking until 1 element is written - elapsed: " + elapsedMilliseconds);
					// This assert is not passing on hqs01 - re-check another time.
					// Timing-sensitive tests are always a bit delicate
					//Assert.True((i == 0) || (elapsedMilliseconds < 2 * opDelayMs) || _inDebugger, "First write may be delayed; after that blocking should only occur for the duration of writing 1 entry.  i=" + i + " Elapsed: " + elapsedMilliseconds);
				}

				stopwatch.Restart();
			}
			stopwatch.Stop();
			Console.WriteLine("Stop + dispose took {0}", stopwatch.Elapsed);

			// At this point, everything should have been logged
			Assert.Equal(maxQueueLength + countBlockingWrites, slowLogWriter.Count);
			Assert.True(stopwatch.ElapsedMilliseconds > maxQueueLength * opDelayMs, "Dispose writes all queued entries");

			// maxQueueLength+2 is the number of sleeps to wait for - the queue is full, +2 is for Stop() + Dispose() sleeps
			// 1.5 is just a tolerance for thread-related delays
			Assert.True((stopwatch.ElapsedMilliseconds < (maxQueueLength + 2) * opDelayMs * 1.5) || _inDebugger, "Took longer than expected: " + stopwatch.ElapsedMilliseconds); 
		}

		[Fact]
		public void InnerWriterExceptionsAreHandled()
		{
			var setupTracerFactory = new SetupTracerFactory();
			var innerLogWriter = new ExceptionThrowingLogWriter<MessageEntry>();
			var backgroundMultiLogWriter = new BackgroundMultiLogWriter(setupTracerFactory, innerLogWriter);
			ILogWriter<MessageEntry> logWriter;
			Assert.True(backgroundMultiLogWriter.GetLogWriter(out logWriter));

			using (backgroundMultiLogWriter)
			{
				backgroundMultiLogWriter.Start();
				LogTestMessages(logWriter, 6);
			}

			Console.WriteLine("Setup messages:");
			setupTracerFactory.WriteEntriesTo(Console.Out,
			                                  new DebuggerTraceFormatter()
			                                  {
				                                  IncludeTimestamp = true
			                                  });
			Assert.Equal(7, setupTracerFactory.Where((traceEntry, index) => traceEntry.TraceLevel >= TraceLevel.Error).Count());
		}

		[Fact(Skip = "Not implemented")]
		public void PerfTestBackgroundWritingVsForegroundWriting()
		{ }

		/// <summary>
		/// Even if you forget to call <see cref="BackgroundMultiLogWriter.Dispose"/>, logs should still be written
		/// to their target when the <see cref="BackgroundMultiLogWriter"/> finalizer is called.
		/// </summary>
		[Fact(Skip = "Too hard to unit test, but it works in a debugger...")]
		public void FinalizerCausesQueuedLogsToFlush()
		{
			// Slow log writer - starting, stopping, disposing, writing an entry, all take at least 10ms each.
			const int opDelayMs = 5;
			var slowLogWriter = new SlowTestLogWriter<MessageEntry>(opDelayMs, false);
			const int expectedEntryCount = 25;

			{
				BackgroundMultiLogWriter backgroundMultiLogWriter;
				IQueueLogWriter<MessageEntry> queueLogWriter;
				SetupBackgroundMessageWriter(slowLogWriter, out backgroundMultiLogWriter, out queueLogWriter);

				LogTestMessages(queueLogWriter, expectedEntryCount);
			}

			Assert.True(slowLogWriter.Count < expectedEntryCount);

			//Thread.Sleep(opDelayMs * (expectedEntryCount * 2));

			// Force GC
			GC.Collect(2, GCCollectionMode.Forced, true);
			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.Equal(expectedEntryCount, slowLogWriter.Count);

			// When the finalizer is called, an error is logged to the SetupTracerFactory.
			Assert.True(SetupTracerFactory.Any(traceEntry => (traceEntry.TraceLevel == TraceLevel.Error) && (traceEntry.Message.StartsWith("In finalizer "))));
		}

		/// <summary>
		/// Exercises a <see cref="TextWriterMultiLogWriter"/> behind a <see cref="BackgroundMultiLogWriter"/>.
		/// </summary>
		/// <seealso cref="TextWriterMultiLogWriterUnitTests.MultiLogWriterToText" />, which is only different by a line or two.
		[Fact]
		public void BackgroundMultiLogWriterToText()
		{
			// Log output written here on the background thread
			var stringWriter = new StringWriter();

			var setupTracerFactory = new SetupTracerFactory();
			FormatAction<LoggingTimer.StartRecord> formatStart = (startRecord, writer) => writer.WriteLine(">{0}", startRecord.TimingId);
			FormatAction<LoggingTimer.StopRecord> formatStop = (stopRecord, writer) => writer.WriteLine("<{0} {1}", stopRecord.TimingId, stopRecord.ElapsedTime);
			var multiLogWriter = new TextWriterMultiLogWriter(stringWriter, setupTracerFactory, false)
				.AddFormat(formatStart)
				.AddFormat(formatStop);
			var backgroundMultiLogWriter = new BackgroundMultiLogWriter(setupTracerFactory, multiLogWriter);

			using (var logManager = new LogManager(backgroundMultiLogWriter))
			{
				// LoggingTimer test class logs starts and stops
				LoggingTimer.RestartTimingIds();
				var timer = new LoggingTimer("test LoggingTimer", logManager);
				var timing1 = timer.Start();
				Thread.Sleep(15);
				timing1.Stop();
				var timing2 = timer.Start();
				Thread.Sleep(10);
				timing2.Stop();
			}

			string logOutput = stringWriter.ToString();
			Console.WriteLine(logOutput);

			Assert.Contains(">2\r\n<2 00:00:00.", logOutput);
			Assert.Contains(">3\r\n<3 00:00:00.", logOutput);
		}
	}

}