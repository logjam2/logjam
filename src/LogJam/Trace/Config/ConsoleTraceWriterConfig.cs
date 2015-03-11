// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTraceWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
	using LogJam.Config;
	using LogJam.Format;
	using LogJam.Trace.Format;
	using LogJam.Writer;


	/// <summary>
	/// Trace log writer configuration that creates a <see cref="ConsoleTraceWriter"/>.
	/// </summary>
	public sealed class ConsoleTraceWriterConfig : LogWriterConfig<TraceEntry>
	{

		/// <summary>
		/// Gets or sets a value that specifies whether created <see cref="ConsoleTraceWriter"/>s will write colored text output.
		/// </summary>
		public bool UseColor { get; set; }

		/// <summary>
		/// Gets or sets the formatter for trace entries.  If not set, <see cref="DebuggerTraceFormatter"/> is used
		/// with default values.
		/// </summary>
		public LogFormatter<TraceEntry> Formatter { get; set; }

		public override ILogWriter<TraceEntry> CreateLogWriter(ITracerFactory setupTracerFactory)
		{
			return new ConsoleTraceWriter(UseColor, Formatter);
		}

		public override bool Equals(ILogWriterConfig other)
		{
			if (! base.Equals(other))
			{
				return false;
			}
			var otherSameType = other as ConsoleTraceWriterConfig;
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