// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writers
{
	using System.Collections.Generic;

	using LogJam.Config;
	using LogJam.Config.Json;


	/// <summary>
	/// Log writer configuration that creates a <see cref="ListLogWriter{TEntry}"/>
	/// </summary>
	[JsonTypeHint("Target", "List")]
	public class ListLogWriterConfig<TEntry> : LogWriterConfig<TEntry> where TEntry : ILogEntry
	{

		public override ILogWriter<TEntry> CreateLogWriter()
		{
			return new ListLogWriter<TEntry>(Synchronized);
		}

	}

}