// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writers
{
	using System.IO;

	using LogJam.Config;
	using LogJam.Config.Json;
	using LogJam.Format;


	/// <summary>
	/// Configures a log writer that writes to the debugger window.
	/// </summary>
	[JsonTypeHint("Target", "Debugger")]
	public sealed class DebuggerLogWriterConfig<TEntry> : TextWriterLogWriterConfig<TEntry> where TEntry : ILogEntry
	{

		/// <summary>
		/// Creates a new <see cref="DebuggerLogWriterConfig{TEntry}"/>.  The <see cref="TextWriterLogWriterConfig{TEntry}.Formatter"/> property must be set.
		/// </summary>
		public DebuggerLogWriterConfig()
		{}

		/// <summary>
		/// Creates a new <see cref="DebuggerLogWriterConfig{TEntry}"/> using the specified <paramref name="formatter"/>.
		/// </summary>
		/// <param name="formatter"></param>
		public DebuggerLogWriterConfig(LogFormatter<TEntry> formatter)
		{
			Formatter = formatter;
		}

		protected override TextWriter CreateTextWriter()
		{
			return new DebuggerTextWriter();
		}

	}

}