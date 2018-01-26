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
using System.Linq;
using System.Threading;

using LogJam.Config;
using LogJam.Internal.UnitTests.Examples;
using LogJam.Test.Shared;

using NSubstitute;

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
        public void TimeIntervalRotatingLogFileWorks()
        {
            string logFileName = nameof(TimeIntervalRotatingLogFileWorks);
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

            // Ensure log file handles are closed
            void AssertFileIsClosed(FileStream filestream)
            {
                Assert.False(filestream.CanWrite);
                Assert.Throws<ObjectDisposedException>(() => filestream.SafeFileHandle.IsClosed);
            }
            Assert.Collection(testLogFileConfig.CreatedFileStreams, AssertFileIsClosed, AssertFileIsClosed);

            // Ensure log files exist + are not empty
            // REVIEW: This reads the file instead of just checking the file size b/c file size takes some time to show up
            void AssertFileIsNotEmpty(FileInfo file)
            {
                file.Refresh();
                Assert.NotEqual(0, file.Length);
            }
            //Action<FileInfo> assertFileIsNotEmpty = file => Assert.NotEqual(0, File.ReadAllText(file.FullName).Length);
            Assert.Collection(GetTestLogFiles(logSubdirectory, logFileName), AssertFileIsNotEmpty, AssertFileIsNotEmpty);
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
