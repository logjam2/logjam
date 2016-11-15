// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogFileConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System;
    using System.Diagnostics.Contracts;


    /// <summary>
    /// Configures the directory, filename, and behavior of log files.
    /// </summary>
    public class LogFileConfig
    {

        private bool? _append;

        /// <summary>
        /// The default filename, used if both <see cref="Filename"/> and <see cref="FilenameFunc"/> are not set.
        /// </summary>
        public const string DefaultFilename = "LogJam.log";

        /// <summary>
        /// The default directory function, used if both <see cref="Directory"/> and <see cref="DirectoryFunc"/> are not set.
        /// </summary>
        public static readonly Func<string> DefaultDirectoryFunc = () => Environment.CurrentDirectory;

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
                Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(value));
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
                Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(value));
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

    }

}
