// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotatingLogFileTests.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using LogJam.Config;
using LogJam.Internal.UnitTests.Examples;
using LogJam.Test.Shared;

using Xunit;

namespace LogJam.Internal.UnitTests.Writer.Rotator
{

    /// <summary>
    /// Exercises rotating log file writing.
    /// </summary>
    public sealed class RotatingLogFileTests
    {
        private readonly DirectoryInfo _logFileDirectory = new DirectoryInfo(".\\logs");

        private FileInfo[] GetTestLogFiles(string directory, string filenamePrefix)
        {
            var logSubDirectory = new DirectoryInfo(Path.Combine(_logFileDirectory.FullName, directory));
            if (!logSubDirectory.Exists)
            {
                return new FileInfo[0];
            }
            return logSubDirectory.GetFiles(filenamePrefix + "*");
        }

        [Fact]
        public void HourlyRotatingLogFileWorks()
        {
            string logFileName = nameof(HourlyRotatingLogFileWorks);
            var testSystemClock = new TestSystemClock();
            testSystemClock.UtcNow = new DateTimeOffset(2017,2,2,1,1,1, TimeSpan.FromHours(-8));
            const string logSubdirectory = "2017\\02\\02";

            // Delete log files to allow a clean start
            foreach (var logfile in GetTestLogFiles(logSubdirectory, logFileName))
            {
                logfile.Delete();
            }

            TestLogFileConfig testLogFileConfig;
            using (var logManager = new LogManager())
            {
                var rotatingWriterConfig = logManager.Config.UseHourlyRotatingTextLogFile(logFileName);

                // Tweak settings for test
                ((TimeIntervalRotatorConfig) rotatingWriterConfig.LogFileRotator).SystemClock = testSystemClock;
                var textLogFileWriterConfig = (TextLogFileWriterConfig) rotatingWriterConfig.LogFileWriter;
                textLogFileWriterConfig.IncludeTime = false;
                testLogFileConfig = new TestLogFileConfig(textLogFileWriterConfig.LogFile);
                textLogFileWriterConfig.LogFile = testLogFileConfig;

                logManager.Config.Writers.FormatAll(new MessageEntry.MessageEntryFormatter());

                var entryWriter = logManager.GetEntryWriter<MessageEntry>();

                ExampleHelper.LogTestMessages(entryWriter, 4);

                // Rotate
                testSystemClock.UtcNow += TimeSpan.FromHours(1);
                testSystemClock.TestTimer.InvokeTimer();

                ExampleHelper.LogTestMessages(entryWriter, 4);

                Assert.True(logManager.IsHealthy);
            }

            // Avoid race conditions with following Assert on appveyor. !!
            Thread.Sleep(10);

            // Ensure log file handles are closed
            void AssertFileIsClosed(FileStream filestream)
            {
                Assert.Throws<ObjectDisposedException>(() => filestream.SafeFileHandle.IsClosed);
                Assert.False(filestream.CanWrite);
            }
            Assert.Collection(testLogFileConfig.CreatedFileStreams, AssertFileIsClosed, AssertFileIsClosed);

            // Ensure log files exist + have expected # of lines
            void AssertFileHas4Lines(FileInfo file)
            {
                string fileContents = File.ReadAllText(file.FullName);
                Assert.Equal(4, fileContents.CountOf('\n'));
            }
            Assert.Collection(GetTestLogFiles(logSubdirectory, logFileName), AssertFileHas4Lines, AssertFileHas4Lines);
        }


        private class TestLogFileConfig : LogFileConfig
        {

            public TestLogFileConfig(LogFileConfig copy)
            {
                this.Append = copy.Append;
                this.CreateDirectories = copy.CreateDirectories;
                this.DirectoryFunc = copy.DirectoryFunc;
                this.FilenameFunc = copy.FilenameFunc;
                this.FilenameExtension = copy.FilenameExtension;
            }

            public override FileStream CreateNewLogFileStream()
            {
                var fileStream = base.CreateNewLogFileStream();
                CreatedFileStreams.Add(fileStream);
                return fileStream;
            }

            public List<FileStream> CreatedFileStreams { get; } = new List<FileStream>();
        }

    }

}
