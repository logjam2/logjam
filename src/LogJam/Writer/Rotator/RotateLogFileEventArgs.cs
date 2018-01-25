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

        private readonly FileInfo _currentLogFile;

        private readonly ILogFileRotator _logFileRotator;
        private readonly FileInfo _nextLogFile;

        /// <summary>
        /// Initializes a new <see cref="RotateLogFileEventArgs"/> instance.
        /// </summary>
        /// <param name="logFileRotator"></param>
        /// <param name="currentLogFile">The current log file. May be <c>null</c>, eg when the first log file is opened.</param>
        /// <param name="nextLogFile"></param>
        public RotateLogFileEventArgs(ILogFileRotator logFileRotator, FileInfo currentLogFile, FileInfo nextLogFile)
        {
            Arg.NotNull(logFileRotator, nameof(logFileRotator));
            Arg.NotNull(nextLogFile, nameof(nextLogFile));

            _logFileRotator = logFileRotator;
            _currentLogFile = currentLogFile;
            _nextLogFile = nextLogFile;
        }

        public ILogFileRotator LogFileRotator { get { return _logFileRotator; } }

        public FileInfo CurrentLogFile { get { return _currentLogFile; } }

        public FileInfo NextLogFile { get { return _nextLogFile; } }

    }
}
