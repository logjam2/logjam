// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextLogFileWriterConfig.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;

	using LogJam.Trace;
	using LogJam.Writer;


	/// <summary>
	/// Configures an <see cref="ILogWriter"/> that writes to text log files.
	/// </summary>
	public class TextLogFileWriterConfig : TextLogWriterConfig, ILogFileWriterConfig
	{

		public TextLogFileWriterConfig()
		{}

		public string Directory { get; set; }
		public string File { get; set; }

		public Action<FileInfo, TextWriter> WriteHeader { get; set; }

		public Action<FileInfo, TextWriter> WriteFooter { get; set; }

		public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
		{
			throw new System.NotImplementedException();
		}

	}

}