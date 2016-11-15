// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogFileWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using LogJam.Writer;


    /// <summary>
    /// Base interface for <see cref="ILogWriterConfig"/> types that configure and create
    /// <see cref="ILogWriter"/>s that write to log files.
    /// </summary>
    public interface ILogFileWriterConfig : ILogWriterConfig
    {

        /// <summary>
        /// Returns the <see cref="LogFileConfig"/>, which is used to configure the log file.
        /// </summary>
        LogFileConfig LogFile { get; }

    }
}
