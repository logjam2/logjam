// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotatingLogFileUnitTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Writer.Rotator
{
	using System;
	using System.Linq;

	using LogJam.Config;
	using LogJam.Internal.UnitTests.Examples;
	using LogJam.Trace.Format;
	using LogJam.Writer.Rotator;

	using Xunit;


	/// <summary>
	/// Exercises <see cref="RotatingLogFileWriter"/>.
	/// </summary>
	public sealed class RotatingLogFileUnitTests
	{

		private const int c_entriesPerLogFile = 20;
		private const int c_parallelLogThreads = 8;
		private const int c_entriesPerThread = 60;

		/// <summary>
		/// Test log file rotation using FakeLogFileLogWriter instead of writing to real log files, and using SynchronizingProxyLogWriter for synchronization.
		/// </summary>
		[Fact]
		public void TestFixedCountRotationSynchronized()
		{
			var logFileRotatorConfig = new EntryCountLogFileRotator.Config("test.{0}.log", c_entriesPerLogFile);
			var fakeLogFileWriterConfig = new FakeLogFileLogWriter<TestEntry>.Config();
			EntryCountLogFileRotator logFileRotator = null;

			var logManager = new LogManager();
			using (logManager)
			{
				var rotatingLogFileWriterConfig = logManager.Config.UseRotatingLogFileWriter(logFileRotatorConfig, fakeLogFileWriterConfig);
				rotatingLogFileWriterConfig.Initializers.Add((dependencies) =>
				                                             { // Wire the FakeLogFileLogWriter.EntryLogged event to EntryCountLogFileRotator.IncrementCount
																 logFileRotator = dependencies.Get<EntryCountLogFileRotator>();
																 fakeLogFileWriterConfig.EntryLogged += (sender, args) => logFileRotator.IncrementCount();
				                                             });

				logManager.Start();
				Assert.True(logManager.IsHealthy);

				LoadHelper.LogTestEntriesInParallel(logManager.GetEntryWriter<TestEntry>(), c_entriesPerThread, c_parallelLogThreads);
			}

			logManager.SetupLog.WriteEntriesTo(Console.Out, new DefaultTraceFormatter() { IncludeTimestamp = true });

			Assert.True(logManager.IsHealthy);
			Assert.Equal(c_parallelLogThreads * c_entriesPerThread, logFileRotator.Count);

			int expectedLogFiles = (int) Math.Ceiling(c_parallelLogThreads * c_entriesPerThread * 1.0 / c_entriesPerLogFile);
			Assert.InRange(fakeLogFileWriterConfig.CreatedLogWriters.Count(), expectedLogFiles, expectedLogFiles + 1);

			foreach (var fakeLogFileWriter in fakeLogFileWriterConfig.CreatedLogWriters)
			{
				Console.WriteLine("{0}: {1} entries", fakeLogFileWriter.LogFile.Name, fakeLogFileWriter.Count);	
			}

			// Most important assert - each file contains exactly c_entriesPerLogFile, or is the lastLogFile
			int lastLogFileCount = c_parallelLogThreads * c_entriesPerThread % c_entriesPerLogFile;
			Assert.True(fakeLogFileWriterConfig.CreatedLogWriters.All(fakeLogFileWriter => (fakeLogFileWriter.Count == c_entriesPerLogFile) || (fakeLogFileWriter.Count == lastLogFileCount)));
		}

		/// <summary>
		/// Test log file rotation using FakeLogFileLogWriter instead of writing to real log files, and using background logging for synchronization.
		/// </summary>
		[Fact]
		public void TestFixedCountRotationBackground()
		{
			var logFileRotatorConfig = new EntryCountLogFileRotator.Config("test.{0}.log", c_entriesPerLogFile);
			var fakeLogFileWriterConfig = new FakeLogFileLogWriter<TestEntry>.Config();
			EntryCountLogFileRotator logFileRotator = null;

			var logManager = new LogManager();
			using (logManager)
			{
				var rotatingLogFileWriterConfig = logManager.Config.UseRotatingLogFileWriter(logFileRotatorConfig, fakeLogFileWriterConfig);
				rotatingLogFileWriterConfig.BackgroundLogging = true;
				rotatingLogFileWriterConfig.Initializers.Add((dependencies) =>
				                                             {
					                                             logFileRotator = dependencies.Get<EntryCountLogFileRotator>();
				                                             });

				logManager.Start();
				Assert.True(logManager.IsHealthy);

				var countingEntryWriter = logFileRotator.ProxyEntryWriter(logManager.GetEntryWriter<TestEntry>());
				LoadHelper.LogTestEntriesInParallel(countingEntryWriter, c_entriesPerThread, c_parallelLogThreads);
			}

			logManager.SetupLog.WriteEntriesTo(Console.Out, new DefaultTraceFormatter() { IncludeTimestamp = true });

			Assert.True(logManager.IsHealthy);
			Assert.Equal(c_parallelLogThreads * c_entriesPerThread, logFileRotator.Count);

			int expectedLogFiles = (int) Math.Ceiling(c_parallelLogThreads * c_entriesPerThread * 1.0 / c_entriesPerLogFile);
			Assert.InRange(fakeLogFileWriterConfig.CreatedLogWriters.Count(), expectedLogFiles, expectedLogFiles + 1);

			foreach (var fakeLogFileWriter in fakeLogFileWriterConfig.CreatedLogWriters)
			{
				Console.WriteLine("{0}: {1} entries", fakeLogFileWriter.LogFile.Name, fakeLogFileWriter.Count);
			}

			// Use of background logging does not guarantee even numbers of entries per log file - it is unsynchronized until entries are queued.
			// Logged entries should be spread throughout the files, but they ARE NOT not capped at c_entriesPerLogFile.  This is fine for most cases, but not for this simple/test rotator.
			Assert.Equal(c_parallelLogThreads * c_entriesPerThread, fakeLogFileWriterConfig.CreatedLogWriters.Sum(fakeLogFileWriter => fakeLogFileWriter.Count));
		}

	}

}