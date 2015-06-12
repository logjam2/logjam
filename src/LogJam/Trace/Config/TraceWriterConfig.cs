// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
	using LogJam.Config;
	using LogJam.Writer;

	using System;
	using System.Diagnostics.Contracts;
	using System.Runtime.Serialization;


	/// <summary>
	/// Holds configuration for a single trace writer.
	/// </summary>
	/// <remarks>
	/// <c>TraceWriterConfig</c> subclasses should generally not override <see cref="object.GetHashCode"/> or <see cref="object.Equals(object)"/>, 
	/// because they are identified by reference.  It should be valid to have two <c>TraceWriterConfig</c> objects with the same values stored
	/// in a set or dictionary.
	/// </remarks>
	public sealed class TraceWriterConfig
	{

		private ILogWriterConfig _tracelogWriterConfig;
		private readonly SwitchSet _switches;

		/// <summary>
		/// Creates a new <see cref="TraceWriterConfig"/> using all default values.
		/// If <see cref="LogWriterConfig"/> is not subsequently set, a <see cref="NoOpEntryWriter{TEntry}"/> will be
		/// used.
		/// </summary>
		public TraceWriterConfig()
		{
			_switches = new SwitchSet();
		}

		public TraceWriterConfig(ILogWriterConfig logWriterConfig, SwitchSet switches = null)
		{
			Contract.Requires<ArgumentNullException>(logWriterConfig != null);

			_tracelogWriterConfig = logWriterConfig;
			_switches = switches ?? new SwitchSet();
		}

		public TraceWriterConfig(ILogWriter logWriter, SwitchSet switches = null)
			: this(new UseExistingLogWriterConfig(logWriter), switches)
		{
			Contract.Requires<ArgumentNullException>(logWriter != null);
		}

		[DataMember(Name = "LogWriter")]
		public ILogWriterConfig LogWriterConfig
		{
			get
			{
				Contract.Ensures(Contract.Result<ILogWriterConfig>() != null);
				if (_tracelogWriterConfig == null)
				{
					_tracelogWriterConfig = new NoOpLogWriterConfig();
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

	}

}