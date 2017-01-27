// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextLogFileWriterTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.File
{
    using System;
    using System.IO;

    using LogJam.Config;
    using LogJam.Internal.UnitTests.Examples;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Verifies that <see cref="TextLogFileWriterConfig"/> works as expected.
    /// </summary>
    public sealed class TextLogFileWriterTests
    {

        private readonly ITestOutputHelper _testOutput;

        public TextLogFileWriterTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void CreateTextLogFile_Works()
        {
            var logFileDirectory = new DirectoryInfo(nameof(TextLogFileWriterTests));
            logFileDirectory.Create();
            _testOutput.WriteLine("Writing test files in directory {0}...", logFileDirectory.FullName);

            string logFileName = new Random().Next() + ".log";
            using (var logManager = new LogManager())
            {
                var textLogFileConfig = logManager.Config.UseTextLogFile(logFileName, logFileDirectory.Name);
                textLogFileConfig.IncludeTime = false;
                textLogFileConfig.Format<MessageEntry>();

                var messageEntryWriter = logManager.GetEntryWriter<MessageEntry>();
                Assert.True(messageEntryWriter.IsEnabled);

                // Write a single entry
                var entry = new MessageEntry("msg");
                messageEntryWriter.Write(ref entry);

                Assert.True(logManager.IsHealthy);
            }

            FileInfo logFile = new FileInfo(Path.Combine(logFileDirectory.FullName, logFileName));
            Assert.True(logFile.Exists);

            // Verify file contents
            using (var reader = logFile.OpenText())
            {
                string fileContents = reader.ReadToEnd();
                Assert.Equal("[?]  msg\r\n", fileContents);
            }
        }

        [Fact]
        public void CreateTextLogFile_InaccessibleDirectory()
        {
            const string inaccessibleDirectory = @"FOO:\bar\baz";

            using (var logManager = new LogManager())
            {
                var textLogFileConfig = logManager.Config.UseTextLogFile("test-log.log", inaccessibleDirectory);
                textLogFileConfig.Format<MessageEntry>();

                var messageEntryWriter = logManager.GetEntryWriter<MessageEntry>();
                Assert.False(messageEntryWriter.IsEnabled);

                // Write a single entry
                var entry = new MessageEntry("msg");
                messageEntryWriter.Write(ref entry);

                Assert.False(logManager.IsHealthy);
            }

        }

        [Fact]
        public void CreateTextLogFile_InvalidFileName()
        {
            var logFileDirectory = new DirectoryInfo(nameof(TextLogFileWriterTests));
            logFileDirectory.Create();

            string invalidLogFileName = "foo\b>bar.log"; // Backspace char and '>' are invalid in windows filenames.
            using (var logManager = new LogManager())
            {
                var textLogFileConfig = logManager.Config.UseTextLogFile(invalidLogFileName, logFileDirectory.Name);
                textLogFileConfig.Format<MessageEntry>();

                var messageEntryWriter = logManager.GetEntryWriter<MessageEntry>();
                Assert.False(messageEntryWriter.IsEnabled);

                // Write a single entry
                var entry = new MessageEntry("msg");
                messageEntryWriter.Write(ref entry);

                Assert.False(logManager.IsHealthy);
            }
        }

    }

}