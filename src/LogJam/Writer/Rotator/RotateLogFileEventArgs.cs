// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotateLogFileEventArgs.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.IO;

using LogJam.Shared.Internal;

namespace LogJam.Writer.Rotator
{

    /// <summary>
    /// Event args for <see cref="ILogFileRotator.TriggerRotate" />.
    /// </summary>
    public class RotateLogFileEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new <see cref="RotateLogFileEventArgs"/> instance.
        /// </summary>
        /// <param name="logFileRotator">The <see cref="ILogFileRotator"/> triggering the rotation (the source of this event)</param>
        /// <param name="currentLogFile">The current log file. May be <c>null</c>, eg when the first log file is opened.</param>
        /// <param name="nextLogFile">The next log file. Must not be <c>null</c>.</param>
        /// <param name="priority">The priority of the rotation. Use <see cref="LogWriterActionPriority.Normal"/> for queue order, meaning the switch
        /// to the new log file should occur after currently queued log entries are written. Use <see cref="LogWriterActionPriority.High"/>
        /// to switch to the new log file before the next log entry is written.</param>
        public RotateLogFileEventArgs(ILogFileRotator logFileRotator, FileInfo currentLogFile, FileInfo nextLogFile, LogWriterActionPriority priority)
        {
            Arg.NotNull(logFileRotator, nameof(logFileRotator));
            Arg.NotNull(nextLogFile, nameof(nextLogFile));

            LogFileRotator = logFileRotator;
            CurrentLogFile = currentLogFile;
            NextLogFile = nextLogFile;
            Priority = priority;
        }

        /// <summary>
        /// The <see cref="ILogFileRotator"/> triggering the rotation (the source of this event)
        /// </summary>
        public ILogFileRotator LogFileRotator { get; }

        /// <summary>
        /// The current log file.
        /// </summary>
        public FileInfo CurrentLogFile { get; }

        /// <summary>
        /// The next log file (the file to switch to).
        /// </summary>
        public FileInfo NextLogFile { get; }

        /// <summary>
        /// The priority of the rotation. Use <see cref="LogWriterActionPriority.Normal"/> for queue order, meaning the switch
        /// to the new log file should occur after currently queued log entries are written. Use <see cref="LogWriterActionPriority.High"/>
        /// to switch to the new log file before the next log entry is written.
        /// </summary>
        public LogWriterActionPriority Priority { get; }

    }
}
