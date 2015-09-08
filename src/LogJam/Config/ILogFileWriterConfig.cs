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
	/// Common configuration for configuring a log writer that writes to the specified <see cref="File"/>.
	/// </summary>
	public interface ILogFileWriterConfig : ILogWriterConfig
	{
		string Directory { get; set; }

		string File { get; set; }

	}

}