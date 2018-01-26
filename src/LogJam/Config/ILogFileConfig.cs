// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogFileConfig.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System.IO;

namespace LogJam.Config
{

    /// <summary>
    /// Provides settings used to create a log file. Can be used for text or binary log files.
    /// </summary>
    public interface ILogFileConfig
    {

        /// <summary>
        /// Returns the directory containing the log file. The returned directory path may be temporal.
        /// </summary>
        string Directory { get; }

        /// <summary>
        /// Returns the log file name, omitting the log file extension. The returned log file name may be temporal.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// Returns the log file name extension.
        /// </summary>
        string FilenameExtension { get; }

        /// <summary>
        /// Creates and returns a <see cref="FileStream"/> which writes to a new logfile.
        /// </summary>
        /// <returns></returns>
        FileStream CreateNewLogFileStream();

    }

}
