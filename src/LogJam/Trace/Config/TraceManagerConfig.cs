// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace.Config
{
	using LogJam.Trace.Format;
	using LogJam.Trace.Switches;
	using LogJam.Util;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;

	using LogJam.Writer;


	/// <summary>
	/// Holds the configuration settings for a <see cref="TraceManager"/> instance.
	/// </summary>
	public sealed class TraceManagerConfig : IEquatable<TraceManagerConfig>
	{

		#region Fields

		/// <summary>
		/// Holds the configuration for <see cref="TraceWriter"/>s.
		/// </summary>
		private readonly ISet<TraceWriterConfig> _traceWriterConfigs;

		#endregion

		#region Constructors and Destructors

		public static TraceManagerConfig Default
		{
			get { return new TraceManagerConfig(CreateDefaultTraceWriterConfig()); }
		}

		/// <summary>
		/// Empty configuration, no traces written.
		/// </summary>
		public TraceManagerConfig()
		{
			_traceWriterConfigs = new HashSet<TraceWriterConfig>();
		}

		public TraceManagerConfig(TraceWriterConfig traceWriterConfig)
		{
			Contract.Requires<ArgumentNullException>(traceWriterConfig != null);

			_traceWriterConfigs = new HashSet<TraceWriterConfig>();
			_traceWriterConfigs.Add(traceWriterConfig);
		}

		public TraceManagerConfig(params TraceWriterConfig[] traceWriterConfigs)
		{
			Contract.Requires<ArgumentNullException>(traceWriterConfigs != null);
			Contract.Requires<ArgumentException>(traceWriterConfigs.Length > 0);
			Contract.Requires<ArgumentNullException>(traceWriterConfigs.All(config => config != null));

			_traceWriterConfigs = new HashSet<TraceWriterConfig>(traceWriterConfigs);
		}

		/// <summary>
		/// Returns a <see cref="TraceWriterConfig"/> containing default values - trace output is logged to the debugger.
		/// </summary>
		/// <returns></returns>
		private static TraceWriterConfig CreateDefaultTraceWriterConfig()
		{
			return new TraceWriterConfig(new DebuggerLogWriterConfig<TraceEntry>(new DebuggerTraceFormatter()))
			       {
				       Switches =
				       {
					       { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Info) }
				       }
			       };
		}

		#endregion

		#region Public Events

		/// <summary>
		/// The global config changed.
		/// </summary>
		//public event EventHandler<ConfigChangedEventArgs<TraceManagerConfig>> GlobalConfigChanged;

		/// <summary>
		/// An event that is raised when a <see cref="TracerConfig"/> instance is added.
		///// </summary>
		//public event EventHandler<ConfigChangedEventArgs<TracerConfig>> TracerConfigAdded;

		///// <summary>
		///// An event that is raised when a <see cref="TracerConfig"/> instance is modified.
		///// </summary>
		//public event EventHandler<ConfigChangedEventArgs<TracerConfig>> TracerConfigChanged;

		///// <summary>
		///// An event that is raised when a <see cref="TracerConfig"/> instance is removed.
		///// </summary>
		//public event EventHandler<ConfigChangedEventArgs<TracerConfig>> TracerConfigRemoved;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the set of <see cref="TraceWriterConfig"/> objects that the <see cref="TraceManager"/> is configured from.
		/// </summary>
		public ISet<TraceWriterConfig> Writers
		{
			get { return _traceWriterConfigs; }
		}

		#endregion

		#region Public Methods and Operators



		#endregion

		public bool Equals(TraceManagerConfig other)
		{
			if (other == null)
			{
				return false;
			}

			return _traceWriterConfigs.SetEquals(other._traceWriterConfigs);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TraceManagerConfig);
		}

		public override int GetHashCode()
		{
			return (_traceWriterConfigs != null ? _traceWriterConfigs.GetUnorderedCollectionHashCode() : 0);
		}

	}
}
