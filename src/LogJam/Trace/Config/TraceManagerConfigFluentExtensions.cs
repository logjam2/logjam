// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfigFluentExtensions.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Config;
	using LogJam.Format;
	using LogJam.Trace.Format;
	using LogJam.Trace.Switches;
	using LogJam.Writer;


	/// <summary>
	/// Extension methods for fluent configuration of <see cref="TraceManagerConfig"/>.
	/// </summary>
	public static class TraceManagerConfigFluentExtensions
	{

		/// <summary>
		/// Use an existing <paramref name="logWriter"/> along with the specified <paramref name="switchSet"/>.
		/// </summary>
		/// <param name="config"></param>
		/// <param name="logWriter"></param>
		/// <param name="switchSet"></param>
		public static TraceWriterConfig UseLogWriter(this TraceManagerConfig config, ILogWriter<TraceEntry> logWriter, SwitchSet switchSet)
		{
			Contract.Requires<ArgumentNullException>(config != null);
			Contract.Requires<ArgumentNullException>(logWriter != null);
			Contract.Requires<ArgumentNullException>(switchSet != null);

			var traceWriterConfig = new TraceWriterConfig(logWriter, switchSet);
			config.Writers.Add(traceWriterConfig);
			return traceWriterConfig;
		}

		public static TraceWriterConfig UseLogWriter(this TraceManagerConfig config, ILogWriter<TraceEntry> logWriter, Type type, ITraceSwitch traceSwitch = null)
		{
			Contract.Requires<ArgumentNullException>(config != null);
			Contract.Requires<ArgumentNullException>(logWriter != null);
			Contract.Requires<ArgumentNullException>(type != null);

			if (traceSwitch == null)
			{
				traceSwitch = new ThresholdTraceSwitch(TraceLevel.Info);
			}
			var switches = new SwitchSet()
			               {
				               { type, traceSwitch }
			               };
			return UseLogWriter(config, logWriter, switches);
		}

		public static TraceWriterConfig UseLogWriter(this TraceManagerConfig config, ILogWriter<TraceEntry> logWriter, string tracerName = Tracer.All, ITraceSwitch traceSwitch = null)
		{
			Contract.Requires<ArgumentNullException>(config != null);
			Contract.Requires<ArgumentNullException>(logWriter != null);
			Contract.Requires<ArgumentNullException>(tracerName != null);

			if (traceSwitch == null)
			{
				traceSwitch = new ThresholdTraceSwitch(TraceLevel.Info);
			}
			var switches = new SwitchSet()
			               {
				               { tracerName, traceSwitch }
			               };
			return UseLogWriter(config, logWriter, switches);
		}

		public static TraceWriterConfig TraceToConsole(this TraceManagerConfig config, SwitchSet switchSet, LogFormatter<TraceEntry> traceFormatter = null)
		{
			Contract.Requires<ArgumentNullException>(config != null);
			Contract.Requires<ArgumentNullException>(switchSet != null);

			if (traceFormatter == null)
			{
				traceFormatter = new DebuggerTraceFormatter();
			}

			var traceWriterConfig = new TraceWriterConfig(new ConsoleTraceWriterConfig(), switchSet);
			config.Writers.Add(traceWriterConfig);
			return traceWriterConfig;			
		}

		public static TraceWriterConfig TraceToConsole(this TraceManagerConfig config, string tracerName = Tracer.All, ITraceSwitch traceSwitch = null, LogFormatter<TraceEntry> traceFormatter = null)
		{
			Contract.Requires<ArgumentNullException>(config != null);
			Contract.Requires<ArgumentNullException>(tracerName != null);

			if (traceSwitch == null)
			{
				traceSwitch = new ThresholdTraceSwitch(TraceLevel.Info);
			}
			var switches = new SwitchSet()
			               {
				               { tracerName, traceSwitch }
			               };
			return TraceToConsole(config, switches, traceFormatter);
		}
	}

}