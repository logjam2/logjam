// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeIntervalRotatorConfig.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.IO;

using LogJam.Internal;
using LogJam.Shared.Internal;
using LogJam.Writer.Rotator;

namespace LogJam.Config
{

    /// <summary>
    /// Configures a log file rotator and log file naming for writing rotating
    /// log files on a regular time interval. Also supports creating date
    /// </summary>
    public sealed class TimeIntervalRotatorConfig : LogFileRotatorConfig
    {

        private TimeSpan _rotateInterval;
        private string _logfileName;
        private string _directoryPattern = TimeIntervalLogFileRotator.DefaultDirectoryPattern;
        private string _logFileNameTimestampPattern = TimeIntervalLogFileRotator.DefaultFileNameTimestampPattern;

        //public TimeIntervalRotatorConfig(TimeSpan rotateInterval, ILogFileConfig innerLogFileConfig)
        //{ }

        internal ISystemClock SystemClock { get; set; } = new SystemClock();

        public TimeSpan RotateInterval
        {
            get
            {
                return _rotateInterval;
            }
            set
            {
                if (value < TimeSpan.FromMinutes(1))
                {
                    throw new ArgumentException("Must be at least 1 minute", nameof(RotateInterval));
                }

                _rotateInterval = value;
            }
        }

        public string LogfileName
        {
            get
            {
                return _logfileName;
            }
            set
            {
                Arg.NotNullOrWhitespace(value, nameof(LogfileName));

                _logfileName = value;
            }
        }

        public string RootDirectory { get; set; } = Directory.GetCurrentDirectory();

        public string DirectoryPattern { get { return _directoryPattern; } set { _directoryPattern = value; } }

        public string LogFileNameTimestampPattern { get { return _logFileNameTimestampPattern; } set { _logFileNameTimestampPattern = value; } }

        public TimeZoneInfo TimeZone { get; set; }

        public override ILogFileRotator CreateLogFileRotator(ILogFileWriterConfig logFileWriterConfig)
        {
            return new TimeIntervalLogFileRotator(RotateInterval, LogfileName, logFileWriterConfig.LogFile.FilenameExtension, RootDirectory, DirectoryPattern, LogFileNameTimestampPattern, TimeZone)
                   {
                       SystemClock = this.SystemClock
                   };
        }

    }

}
