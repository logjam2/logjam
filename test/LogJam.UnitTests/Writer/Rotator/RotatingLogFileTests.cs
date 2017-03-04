// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotatingLogFileTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Writer.Rotator
{
    using System;
    using System.IO;
    using System.Linq;

    using LogJam.Config;
    using LogJam.Internal.UnitTests.Examples;
    using LogJam.Trace.Format;
    using LogJam.Writer.Rotator;

    using NSubstitute;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Exercises <see cref="RotatingLogFileWriter"/>.
    /// </summary>
    public sealed class RotatingLogFileTests
    {

        private readonly ITestOutputHelper _testOutput;

        public RotatingLogFileTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhenRotateIsCalledPreviousLogWriterIsClosed(bool backgroundLogging)
        {
            var file1 = new FileInfo("1.log");
            var file2 = new FileInfo("2.log");
            var rotator = Substitute.For<ILogFileRotator>();
            rotator.CurrentLogFile.Returns(file1);
            rotator.Rotate(Arg.Any<RotatingLogFileWriter>(), Arg.Any<RotateLogFileEventArgs>()).Returns(callInfo =>
                                                                                                        {
                                                                                                            var rotatingLogFileWriter = (RotatingLogFileWriter) callInfo[0];
                                                                                                            rotatingLogFileWriter.SwitchLogFileWriterTo(file2);
                                                                                                            rotator.CurrentLogFile.Returns(file2);
                                                                                                            return null; // No cleanup action
                                                                                                        });

            var fakeLogFileWriterConfig = new FakeLogFileLogWriter<TestEntry>.Config();

            var logManager = new LogManager();
            using (logManager)
            {
                var rotatingLogFileWriterConfig = logManager.Config.UseRotatingLogFileWriter(rotator, fakeLogFileWriterConfig);
                rotatingLogFileWriterConfig.BackgroundLogging = backgroundLogging;
                logManager.Start();
                Assert.True(logManager.IsHealthy);

                var logWriter = logManager.GetEntryWriter<TestEntry>();
                int counter = 0;
                LoadHelper.LogTestEntries(ref counter, logWriter, 1);

                // Trigger rotation
                rotator.TriggerRotate += Raise.EventWith(new RotateLogFileEventArgs(rotator, rotator.CurrentLogFile, file2));

                LoadHelper.LogTestEntries(ref counter, logWriter, 1);
            }
            Assert.True(logManager.IsHealthy);
        }

        /// <summary>
        /// Test log file rotation using FakeLogFileLogWriter instead of writing to real log files, with log writing occurring simultaneously on multiple threads.
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
                // TODO: Should this be provided by EntryCountLogFileRotator.Config ?
                rotatingLogFileWriterConfig.Initializers.Add((dependencies) =>
                                                             {
                                                                 logFileRotator = dependencies.Get<EntryCountLogFileRotator>();
                                                                 fakeLogFileWriterConfig.EntryLogged += (sender, args) => logFileRotator.IncrementCount();
                                                             });

                logManager.Start();
                Assert.True(logManager.IsHealthy);

                LoadHelper.LogTestEntriesInParallel(logManager.GetEntryWriter<TestEntry>(), entriesPerThread, parallelLogThreads);
            }

            _testOutput.WriteEntries(logManager.SetupLog,
                                     new DefaultTraceFormatter()
                                     {
                                         IncludeTimestamp = true
                                     });

            Assert.True(logManager.IsHealthy);
            Assert.Equal(parallelLogThreads * entriesPerThread, logFileRotator.Count);

            int expectedLogFiles = (int) Math.Ceiling(parallelLogThreads * entriesPerThread * 1.0 / entriesPerLogFile);
            Assert.InRange(fakeLogFileWriterConfig.CreatedLogWriters.Count(), expectedLogFiles, expectedLogFiles + 1);

            foreach (var fakeLogFileWriter in fakeLogFileWriterConfig.CreatedLogWriters)
            {
                _testOutput.WriteLine("{0}: {1} entries", fakeLogFileWriter.LogFile.Name, fakeLogFileWriter.Count);
            }

            // Most important assert - each file contains exactly c_entriesPerLogFile, or is the lastLogFile
            int lastLogFileCount = parallelLogThreads * entriesPerThread % entriesPerLogFile;
            Assert.True(fakeLogFileWriterConfig.CreatedLogWriters.All(fakeLogFileWriter =>
                                                                          (fakeLogFileWriter.Count == entriesPerLogFile) || (fakeLogFileWriter.Count == lastLogFileCount)));
        }

    }

}