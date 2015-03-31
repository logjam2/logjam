// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
	using LogJam.Config;
	using LogJam.Writer;


	/// <summary>
	/// Trace log writer configuration that creates a <see cref="ConsoleLogWriter"/>.
	/// </summary>
	public sealed class ConsoleLogWriterConfig : TextWriterLogWriterConfig
	{

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
			if (! base.Equals(other))
			{
				return false;
			}
			var otherSameType = other as ConsoleLogWriterConfig;
			if (otherSameType == null)
			{
				return false;
			}

			return UseColor == otherSameType.UseColor;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ UseColor.GetHashCode();
		}

	}

}