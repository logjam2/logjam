// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogFileConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using LogJam.Shared.Internal;

namespace LogJam.Config
{
    using System;
    using System.IO;


    /// <summary>
    /// Configures the directory, filename, and behavior of log files.
    /// </summary>
    /// <remarks>
    /// This configuration type is used for both text log files, and binary log files.
    /// </remarks>
    public class LogFileConfig
    {

        private bool? _append;

        /// <summary>
        /// The default filename, used if both <see cref="Filename"/> and <see cref="FilenameFunc"/> are not set.
        /// </summary>
        public const string DefaultFilename = "LogJam.log";

        /// <summary>
        /// The default buffer size for writing to log files.
        /// </summary>
        public const int DefaultBufferSize = 4096;

        /// <summary>
        /// The default directory function, used if both <see cref="Directory"/> and <see cref="DirectoryFunc"/> are not set.
        /// </summary>
        public static readonly Func<string> DefaultDirectoryFunc = System.IO.Directory.GetCurrentDirectory;

        /// <summary>
        /// Gets or sets the directory containing the log file.
        /// </summary>
        public string Directory {
            get
            {
                var directoryFunc = DirectoryFunc ?? DefaultDirectoryFunc;
                return directoryFunc();
            }
            set
            {
                Arg.NotNullOrWhitespace(value, nameof(Directory));

                DirectoryFunc = () => value;
            }
        }

        /// <summary>
        /// Gets or sets the log file directory function, which is used for just-in-time log file directory resolution.
        /// </summary>
        public Func<string> DirectoryFunc { get; set; }

        /// <summary>
        /// Gets or sets the log file name.
        /// </summary>
        public string Filename
        {
            get
            {
                var filenameFunc = FilenameFunc;
                return filenameFunc != null ? filenameFunc() : DefaultFilename;
            }
            set
            {
                Arg.NotNullOrWhitespace(value, nameof(Filename));

                FilenameFunc = () => value;
            }
        }

        /// <summary>
        /// Gets or sets the log filename function, which is used for just-in-time log file name resolution.
        /// </summary>
        public Func<string> FilenameFunc { get; set; }

        /// <summary>
        /// Set to <c>true</c> to append to an existing log file; set to <c>false</c> to overwrite an existing log file.
        /// Default is <c>true</c>.
        /// </summary>
        public bool Append { get { return _append ?? true; } set { _append = value; } }

        /// <summary>
        /// The size of the buffer used for writing to the log file. Larger buffers are generally faster,
        /// smaller buffers ensure that log entries are written to disk more quickly.
        /// </summary>
        public int BufferSize { get; set; } = DefaultBufferSize;

        /// <summary>
        /// Returns the full path for a new log file.
        /// </summary>
        /// <returns></returns>
        public string GetNewLogFileFullPath()
        {
            return Path.Combine(Directory, Filename);
        }

        /// <summary>
        /// Creates and returns a <see cref="FileStream"/> which writes to a new logfile.
        /// </summary>
        /// <returns></returns>
        public FileStream CreateNewLogFileStream()
        {
            return new FileStream(GetNewLogFileFullPath(), Append ? FileMode.Append : FileMode.CreateNew, FileAccess.Write, FileShare.Read, BufferSize);
        }

    }

}
