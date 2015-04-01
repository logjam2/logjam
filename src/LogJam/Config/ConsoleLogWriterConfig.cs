// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogWriterConfig.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using LogJam.Config.Json;
	using LogJam.Trace;
	using LogJam.Util;
	using LogJam.Writer;


	/// <summary>
	/// Configures a log writer that writes log output to the console, aka stdout.
	/// </summary>
	[JsonTypeHint("Target", "Console")]
	public sealed class ConsoleLogWriterConfig : TextLogWriterConfig
	{

		/// <summary>
		/// Creates a new <see cref="ConsoleLogWriterConfig"/>.
		/// </summary>
		public ConsoleLogWriterConfig()
		{
			// Default Synchronized to false
			Synchronized = false;
		}

		/// <summary>
		/// Gets or sets a value that specifies whether created <see cref="ConsoleLogWriter"/>s will write colored text output.
		/// </summary>
		public bool UseColor { get; set; }

		public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
		{
			var writer = new ConsoleLogWriter(setupTracerFactory, UseColor, Synchronized);
			ApplyConfiguredFormatters(writer);
			return writer;
		}

		public override bool Equals(ILogWriterConfig other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			var otherSameType = other as ConsoleLogWriterConfig;
			if (otherSameType == null)
			{
				return false;
			}

			return base.Equals(otherSameType) && (UseColor == otherSameType.UseColor);
		}

	}

}