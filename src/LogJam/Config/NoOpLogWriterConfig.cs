// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using LogJam.Config.Json;
	using LogJam.Trace;
	using LogJam.Writer;


	/// <summary>
	/// Log writer configuration that creates a <see cref="NoOpEntryWriter{TEntry}"/>
	/// </summary>
	[JsonTypeHint("Target", "null")]
	public class NoOpLogWriterConfig : LogWriterConfig
	{

		public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
		{
			return new NoOpLogWriter(setupTracerFactory);
		}

	}

}