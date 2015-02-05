// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writers
{
	using LogJam.Config;
	using LogJam.Config.Json;


	/// <summary>
	/// Log writer configuration that creates a <see cref="NoOpLogWriter{TEntry}"/>
	/// </summary>
	[JsonTypeHint("Target", "null")]
	public class NoOpLogWriterConfig<TEntry> : LogWriterConfig<TEntry> where TEntry : ILogEntry
	{

		public override ILogWriter<TEntry> CreateLogWriter()
		{
			return new NoOpLogWriter<TEntry>();
		}

		public override bool Equals(ILogWriterConfig other)
		{
			var otherSameType = other as NoOpLogWriterConfig<TEntry>;
			if (otherSameType == null)
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode();
		}

	}

}