// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeIntervalLogFileRotator.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using LogJam.Config;
using LogJam.Internal;

namespace LogJam.Writer.Rotator
{

    /// <summary>
    /// A log file rotator that rotates log files on a time interval.
    /// </summary>
    public sealed class TimeIntervalLogFileRotator : Startable, ILogFileRotator, IDisposable
    {

        /// <summary>
        /// The default directory pattern for time-interval based log file rotator. It results in a different directory 
        /// being created each day.
        /// </summary>
        public const string DefaultDirectoryPattern = "yyyy/MM/dd";

        /// <summary>
        /// The default filename pattern for time-interval based log file rotation. This pattern is appended to the f
        /// </summary>
        public const string DefaultFileNameTimestampPattern = "'.'yyyyMMdd-HH";

        /// <summary>
        /// If we're within this margin of the actual rotation time, and rotate (or new logfile) is opened, round up to the next rotation.
        /// </summary>
        private readonly TimeSpan _rotateTimeMargin = TimeSpan.FromSeconds(5);

        private IDisposable _timer;
        private FileInfo _currentLogFile;

        public TimeIntervalLogFileRotator(TimeSpan rotateInterval,
                                          string logfileName,
                                          string logfileExtension = LogFileConfig.DefaultFilenameExtension,
                                          string rootDirectory = LogFileConfig.DefaultLogFileDirectory,
                                          string directoryPattern = DefaultDirectoryPattern,
                                          string logFileNameTimestampPattern = DefaultFileNameTimestampPattern,
                                          TimeZoneInfo timeZone = null)
        {
            if (rotateInterval < TimeSpan.FromMinutes(1))
            {
                throw new ArgumentException("Must be at least 1 minute", nameof(rotateInterval));
            }
            if (string.IsNullOrWhiteSpace(logfileName))
            {
                throw new ArgumentException("Cannot be empty", nameof(logfileName));
            }
            if (string.IsNullOrEmpty(directoryPattern) && string.IsNullOrEmpty(logFileNameTimestampPattern))
            {
                throw new ArgumentException("At least one of DirectoryPattern or LogFileNameTimestampPattern must be set.");
            }

            // Ensure that formatting works for directoryPattern and logFileNameTimestampPattern
            if (! string.IsNullOrWhiteSpace(directoryPattern))
            {
                DateTimeOffset.UtcNow.ToString(directoryPattern, CultureInfo.InvariantCulture);
            }
            if (! string.IsNullOrWhiteSpace(logFileNameTimestampPattern))
            {
                DateTimeOffset.UtcNow.ToString(logFileNameTimestampPattern, CultureInfo.InvariantCulture);
            }

            RotateInterval = rotateInterval;
            LogfileName = logfileName;
            LogfileExtension = logfileExtension;
            RootDirectory = rootDirectory;
            DirectoryPattern = directoryPattern;
            LogFileNameTimestampPattern = logFileNameTimestampPattern;
            TimeZone = timeZone ?? TimeZoneInfo.Local;
        }

        public void Dispose()
        {
            InternalStop();
            State = StartableState.Disposed;
        }

        internal ISystemClock SystemClock { get; set; } = new SystemClock();

        public TimeSpan RotateInterval { get; }

        public string LogfileName { get; }
        public string LogfileExtension { get; }
        public string RootDirectory { get; }

        public string DirectoryPattern { get; }

        public string LogFileNameTimestampPattern { get; }

        public TimeZoneInfo TimeZone { get; }

        #region ILogFileRotator

        public event EventHandler<RotateLogFileEventArgs> TriggerRotate;

        public Action Rotate(RotatingLogFileWriter rotatingLogFileWriter, RotateLogFileEventArgs rotateEventArgs)
        {
            Action cleanupAction = rotatingLogFileWriter.SwitchLogFileWriterTo(rotateEventArgs.NextLogFile);
            _currentLogFile = rotateEventArgs.NextLogFile;
            return cleanupAction;
        }

        public FileInfo CurrentLogFile => _currentLogFile;

        public IEnumerable<FileInfo> EnumerateLogFiles
        {
            get
            {
                // TODO: Iterate all files in directories under RootDirectory that match the pattern
                yield return null;
            }
        }

        #endregion

        #region Startable

        protected override void InternalStart()
        {
            // Determine the amount of time before the first Timer callback
            DateTimeOffset utcNow = SystemClock.UtcNow;
            DateTimeOffset nowInTimeZone = utcNow.ToOffset(TimeZone.GetUtcOffset(utcNow));
            DateTime todayInTimeZone = nowInTimeZone.Date;
            long ticksToNextRotate = RotateInterval.Ticks - ((nowInTimeZone.Ticks - todayInTimeZone.Ticks) % RotateInterval.Ticks);

            // Don't rotate if within _rotateTimeMargin from the next rotation (to avoid race conditions etc)
            if (ticksToNextRotate <= _rotateTimeMargin.Ticks)
            {
                ticksToNextRotate += RotateInterval.Ticks;
            }

            _timer = SystemClock.CreateTimer(TimerCallback, this, new TimeSpan(ticksToNextRotate), RotateInterval);

            _currentLogFile = DetermineLogFileFor(utcNow);
        }

        protected override void InternalStop()
        {
            _timer?.Dispose();
            _timer = null;
        }

        #endregion

        private void TimerCallback(object state)
        {
            // Use now + rotateTimeMargin for the current time (in case Timer calls slightly before the correct time)
            FileInfo nextLogFile = DetermineLogFileFor(SystemClock.UtcNow.Add(_rotateTimeMargin));

            // In some cases the timer call may not result in a rotation
            // - eg spring daylight savings transition, where 2 adjacent hours have the same time
            if (! string.Equals(CurrentLogFile?.FullName, nextLogFile.FullName, StringComparison.OrdinalIgnoreCase))
            {
                var rotateEventArgs = new RotateLogFileEventArgs(this, CurrentLogFile, nextLogFile, LogWriterActionPriority.Normal);
                TriggerRotate?.Invoke(this, rotateEventArgs);
            }
        }

        /// <summary>
        /// Determines the log file for <paramref name="dateTime"/>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal FileInfo DetermineLogFileFor(DateTimeOffset dateTime)
        {
            // Ensure dateTime is in the correct TimeZone
            dateTime = dateTime.ToOffset(TimeZone.GetUtcOffset(dateTime));

            string directory = RootDirectory;
            if (! string.IsNullOrWhiteSpace(DirectoryPattern))
            {
                directory = Path.Combine(directory, dateTime.ToString(DirectoryPattern, CultureInfo.InvariantCulture));
            }

            string filename;
            if (string.IsNullOrWhiteSpace(LogFileNameTimestampPattern))
            {
                filename = LogfileName + LogfileExtension;
            }
            else
            {
                filename = LogfileName + dateTime.ToString(LogFileNameTimestampPattern, CultureInfo.InvariantCulture) + LogfileExtension;
            }

            return new FileInfo(Path.Combine(directory, filename));
        }

    }

}
