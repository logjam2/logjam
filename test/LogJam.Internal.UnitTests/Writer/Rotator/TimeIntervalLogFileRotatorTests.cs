// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeIntervalLogFileRotatorTests.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;

using LogJam.Writer.Rotator;

using Xunit;
using Xunit.Abstractions;

namespace LogJam.Internal.UnitTests.Writer.Rotator
{

    /// <summary>
    /// Validates behavior of <see cref="TimeIntervalLogFileRotator"/>.
    /// </summary>
    public sealed class TimeIntervalLogFileRotatorTests
    {

        private readonly ITestOutputHelper _testOutput;

        public TimeIntervalLogFileRotatorTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Theory]
        [InlineData("12/31/2017 2:10 PM", 60, "\\logs\\2017\\12\\31\\unittest.20171231-14.log")]
        [InlineData("1/1/2018 2:00 AM", 60, "\\logs\\2018\\01\\01\\unittest.20180101-02.log")]
        [InlineData("1/1/2018 1:59 AM", 60, "\\logs\\2018\\01\\01\\unittest.20180101-01.log")]
        [InlineData("1/1/2018 12:01 AM", 60, "\\logs\\2018\\01\\01\\unittest.20180101-00.log")]
        public void DefaultLogFilePaths(string datetimeString, int rotateIntervalMinutes, string expectedLogFilePath)
        {
            // Parse dateTime in PST
            var dateTime = DateTime.Parse(datetimeString);
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var dateTimeOffset = new DateTimeOffset(dateTime, timeZone.GetUtcOffset(dateTime));

            var rotator = new TimeIntervalLogFileRotator(TimeSpan.FromMinutes(rotateIntervalMinutes), "unittest");

            // Test
            var logFile = rotator.DetermineLogFileFor(dateTimeOffset);

            _testOutput.WriteLine(logFile.FullName);
            Assert.EndsWith(expectedLogFilePath, logFile.FullName);
        }

    }

}
