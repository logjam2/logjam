// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerConfigFluentExtensions.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;

	using LogJam.Writer;


	/// <summary>
	/// Extension methods to <see cref="LogManagerConfig"/> for configuring using fluent syntax.
	/// </summary>
	public static class LogManagerConfigFluentExtensions
	{

		public static TextWriterMultiLogWriterConfig UseMultiTextWriter(this LogManagerConfig logManagerConfig, TextWriter textWriter)
		{
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);
			Contract.Requires<ArgumentNullException>(textWriter != null);

			var writerConfig = new TextWriterMultiLogWriterConfig(textWriter);
			logManagerConfig.Writers.Add(writerConfig);
			return writerConfig;
		}

		public static TextWriterMultiLogWriterConfig UseMultiTextWriter(this LogManagerConfig logManagerConfig, Func<TextWriter> createTextWriterFunc)
		{
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);
			Contract.Requires<ArgumentNullException>(createTextWriterFunc != null);

			var writerConfig = new TextWriterMultiLogWriterConfig(createTextWriterFunc);
			logManagerConfig.Writers.Add(writerConfig);
			return writerConfig;
		}

		public static TextWriterMultiLogWriterConfig UseMultiDebugger(this LogManagerConfig logManagerConfig)
		{
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);

			return logManagerConfig.UseMultiTextWriter(new DebuggerTextWriter());
		}

	}

}