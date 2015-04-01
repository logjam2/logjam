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

		public static TextWriterLogWriterConfig UseTextWriter(this LogManagerConfig logManagerConfig, TextWriter textWriter)
		{
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);
			Contract.Requires<ArgumentNullException>(textWriter != null);

			var writerConfig = new TextWriterLogWriterConfig(textWriter);
			logManagerConfig.Writers.Add(writerConfig);
			return writerConfig;
		}

		public static TextWriterLogWriterConfig UseTextWriter(this LogManagerConfig logManagerConfig, Func<TextWriter> createTextWriterFunc)
		{
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);
			Contract.Requires<ArgumentNullException>(createTextWriterFunc != null);

			var writerConfig = new TextWriterLogWriterConfig(createTextWriterFunc);
			logManagerConfig.Writers.Add(writerConfig);
			return writerConfig;
		}

		public static DebuggerLogWriterConfig UseDebugger(this LogManagerConfig logManagerConfig)
		{
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);

			var debuggerConfig = new DebuggerLogWriterConfig();
			logManagerConfig.Writers.Add(debuggerConfig);
			return debuggerConfig;
		}

		public static ConsoleLogWriterConfig UseConsole(this LogManagerConfig logManagerConfig, bool colorize = true)
		{
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);

			var consoleWriterConfig = new ConsoleLogWriterConfig()
			                          {
				                          UseColor = colorize
			                          };
			logManagerConfig.Writers.Add(consoleWriterConfig);
			return consoleWriterConfig;
		}

		public static UseExistingLogWriterConfig UseLogWriter(this LogManagerConfig logManagerConfig, ILogWriter logWriter)
		{
			Contract.Requires<ArgumentNullException>(logManagerConfig != null);
			Contract.Requires<ArgumentNullException>(logWriter != null);

			var useExistingConfig = new UseExistingLogWriterConfig(logWriter);
			logManagerConfig.Writers.Add(useExistingConfig);
			return useExistingConfig;
		}
	}

}