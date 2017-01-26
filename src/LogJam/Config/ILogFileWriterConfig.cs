// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using LogJam.Trace;
	using LogJam.Util;
	using LogJam.Writer;


	/// <summary>
	/// Common configuration for configuring a log writer that writes to a log file. This interface 
	/// is used for configuring text log files and binary log files.
	/// </summary>
	public interface ILogFileWriterConfig : ILogWriterConfig
	{

        /// <summary>
        /// Returns the <see cref="LogFileConfig"/>, which is used to configure the log file.
        /// </summary>
        LogFileConfig LogFile { get; }

    }

}