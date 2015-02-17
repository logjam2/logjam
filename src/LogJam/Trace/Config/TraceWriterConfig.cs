// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Runtime.Serialization;

	using LogJam.Config;
	using LogJam.Util;
	using LogJam.Writer;


	/// <summary>
	/// Holds configuration for a single trace writer.
	/// </summary>
	public sealed class TraceWriterConfig : IEquatable<TraceWriterConfig>
	{

		private LogWriterConfig<TraceEntry> _tracelogWriterConfig;
		private readonly SwitchSet _switches;

		/// <summary>
		/// Creates a new <see cref="TraceWriterConfig"/> using all default values.
		/// If <see cref="LogWriterConfig"/> is not subsequently set, a <see cref="NoOpLogWriter{TEntry}"/> will be
		/// used.
		/// </summary>
		public TraceWriterConfig()
		{
			_switches = new SwitchSet();
		}

		public TraceWriterConfig(LogWriterConfig<TraceEntry> logWriterConfig, SwitchSet switches = null)
		{
			Contract.Requires<ArgumentNullException>(logWriterConfig != null);

			_tracelogWriterConfig = logWriterConfig;
			_switches = switches ?? new SwitchSet();
		}

		public TraceWriterConfig(ILogWriter<TraceEntry> logWriter, SwitchSet switches = null)
			: this(new UseExistingLogWriterConfig<TraceEntry>(logWriter), switches)
		{
			Contract.Requires<ArgumentNullException>(logWriter != null);
		}

		[DataMember(Name = "LogWriter")]
		public LogWriterConfig<TraceEntry> LogWriterConfig
		{
			get
			{
				Contract.Ensures(Contract.Result<LogWriterConfig<TraceEntry>>() != null);
				if (_tracelogWriterConfig == null)
				{
					_tracelogWriterConfig = new NoOpLogWriterConfig<TraceEntry>();
				}

				return _tracelogWriterConfig;
			}
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);

				_tracelogWriterConfig = value;
			}
		}

		public SwitchSet Switches
		{
			get { return _switches; }	
		}

		internal SwitchSet GetSwitchList()
		{
			return _switches;
		}

		public bool Equals(TraceWriterConfig other)
		{
			return other != null && LogWriterConfig.Equals(other.LogWriterConfig) && Switches.Equals(other.Switches);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TraceWriterConfig);
		}

		public override int GetHashCode()
		{
			return LogWriterConfig.Hash(3) + Switches.GetHashCode();
		}

	}

}