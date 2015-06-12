// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System.IO;

	using LogJam.Config.Json;
	using LogJam.Trace;
	using LogJam.Util;
	using LogJam.Writer;


	/// <summary>
	/// Configures a log writer that writes to the debugger window.
	/// </summary>
	[JsonTypeHint("Target", "Debugger")]
	public sealed class DebuggerLogWriterConfig : TextLogWriterConfig
	{

		/// <summary>
		/// Creates a new <see cref="DebuggerLogWriterConfig"/>.
		/// </summary>
		public DebuggerLogWriterConfig()
		{
			// Default Synchronized to false
			Synchronized = false;
		}

		public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
		{
			var writer = new DebuggerLogWriter(setupTracerFactory, Synchronized);
			ApplyConfiguredFormatters(writer);
			return writer;
		}

	}

}