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
	using Xunit.Extensions;


	/// <summary>
	/// Exercises <see cref="RotatingLogFileWriter"/>.
	/// </summary>
	public sealed class RotatingLogFileUnitTests
	{

		/// <summary>
		/// Test log file rotation using FakeLogFileLogWriter instead of writing to real log files, and using either SynchronizingProxyLogWriter for synchronization. or background logging for synchronization.
		/// </summary>
		[Theory]
		[InlineData(false, 20, 8, 60)]
		[InlineData(true, 20, 8, 60)]
		[InlineData(false, 11, 16, 45)]
		[InlineData(true, 11, 16, 45)]
		public void TestFixedCountRotation(bool backgroundLogging, int entriesPerLogFile, int parallelLogThreads, int entriesPerThread)
		{
			var logFileRotatorConfig = new EntryCountLogFileRotator.Config("test.{0}.log", entriesPerLogFile);
			var fakeLogFileWriterConfig = new FakeLogFileLogWriter<TestEntry>.Config();
			EntryCountLogFileRotator logFileRotator = null;

			var logManager = new LogManager();
			using (logManager)
			{
				var rotatingLogFileWriterConfig = logManager.Config.UseRotatingLogFileWriter(logFileRotatorConfig, fakeLogFileWriterConfig);
				rotatingLogFileWriterConfig.BackgroundLogging = backgroundLogging;
				rotatingLogFileWriterConfig.Initializers.Add((dependencies) =>
				                                             {
					                                             logFileRotator = dependencies.Get<EntryCountLogFileRotator>();
																 fakeLogFileWriterConfig.EntryLogged += (sender, args) => logFileRotator.IncrementCount();
															 });

				logManager.Start();
				Assert.True(logManager.IsHealthy);

				LoadHelper.LogTestEntriesInParallel(logManager.GetEntryWriter<TestEntry>(), entriesPerThread, parallelLogThreads);
			}

			logManager.SetupLog.WriteEntriesTo(Console.Out, new DefaultTraceFormatter() { IncludeTimestamp = true });

			Assert.True(logManager.IsHealthy);
			Assert.Equal(parallelLogThreads * entriesPerThread, logFileRotator.Count);

			int expectedLogFiles = (int) Math.Ceiling(parallelLogThreads * entriesPerThread * 1.0 / entriesPerLogFile);
			Assert.InRange(fakeLogFileWriterConfig.CreatedLogWriters.Count(), expectedLogFiles, expectedLogFiles + 1);

			foreach (var fakeLogFileWriter in fakeLogFileWriterConfig.CreatedLogWriters)
			{
				Console.WriteLine("{0}: {1} entries", fakeLogFileWriter.LogFile.Name, fakeLogFileWriter.Count);
			}

			// Most important assert - each file contains exactly c_entriesPerLogFile, or is the lastLogFile
			int lastLogFileCount = parallelLogThreads * entriesPerThread % entriesPerLogFile;
			Assert.True(fakeLogFileWriterConfig.CreatedLogWriters.All(fakeLogFileWriter => (fakeLogFileWriter.Count == entriesPerLogFile) || (fakeLogFileWriter.Count == lastLogFileCount)));
		}

	}

}