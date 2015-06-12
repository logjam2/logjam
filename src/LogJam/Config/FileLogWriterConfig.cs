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
	/// Configures a log writer that writes to the specified <see cref="File"/>.
	/// </summary>
	public sealed class FileLogWriterConfig : LogWriterConfig
	{
		public string Directory { get; set; }

		public string File { get; set; }

		public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
		{
			throw new System.NotImplementedException();
		}

	}

}