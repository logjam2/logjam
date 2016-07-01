// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System;

    using LogJam.Trace;
    using LogJam.Writer;


    /// <summary>
    /// Configures a log writer that writes to a file.
    /// </summary>
    public abstract class FileLogWriterConfig : LogWriterConfig
    {

        private bool? _append;

        /// <summary>
        /// The default filename, used if both <see cref="Filename"/> and <see cref="FilenameFunc"/> are not set.
        /// </summary>
        public const string DefaultFilename = "LogJam.log";

        /// <summary>
        /// The default directory to create log files in, used if both <see cref="Directory"/> and <see cref="DirectoryFunc"/> are not set.
        /// </summary>
        public static readonly string DefaultDirectory = Environment.CurrentDirectory;

        public string Directory { get; set; }

        public Func<string> DirectoryFunc { get; set; }

        public string Filename
        {
            get {} set;
        }

        public Func<string> FilenameFunc { get; set; }

        /// <summary>
        /// Set to <c>true</c> to append to an existing log file; set to <c>false</c> to overwrite an existing log file.
        /// Default is <c>true</c>.
        /// </summary>
        public bool Append { get; set; }

    }

}
