// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="TraceConfig.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace.Config
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;

	using LogJam.Trace.TraceSwitch;
	using LogJam.Trace.Writers;

	/// <summary>
	/// Manages global configuration settings for the <c>LogJam.Trace</c> subsystem.
	/// </summary>
	public sealed class TraceManagerConfig
	{
		#region Static Fields

		/// <summary>
		/// Fallback <see cref="TracerConfig"/>, use if all other configuration is deleted.
		/// </summary>
		private static readonly TracerConfig s_fallbackTracerConfig = new TracerConfig(
			string.Empty, new ThresholdTraceSwitch(TraceLevel.Info), DebuggerTraceWriter.Instance);

		#endregion

		#region Fields

		/// <summary>
		/// The tree of all active <see cref="TracerConfig"/> instances.
		/// </summary>
		private readonly TracerConfig _rootTracerConfig = new TracerConfig(
			string.Empty, new ThresholdTraceSwitch(TraceLevel.Info), DebuggerTraceWriter.Instance);

		#endregion

		#region Constructors and Destructors

		internal TraceManagerConfig()
		{}

		#endregion

		#region Public Events

		/// <summary>
		/// The global config changed.
		/// </summary>
		public event EventHandler<ConfigChangedEventArgs<TraceManagerConfig>> GlobalConfigChanged;

		/// <summary>
		/// The tracer config added.
		/// </summary>
		public event EventHandler<ConfigChangedEventArgs<TracerConfig>> TracerConfigAdded;

		/// <summary>
		/// The tracer config removed.
		/// </summary>
		public event EventHandler<ConfigChangedEventArgs<TracerConfig>> TracerConfigRemoved;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the tracer config items.
		/// </summary>
		/// <value>
		/// The tracer config items.
		/// </value>
		public TracerConfig RootTracerConfig
		{
			get
			{
				return _rootTracerConfig;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The add tracer config.
		/// </summary>
		/// <param name="tracerConfig">
		/// The tracer config.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// </exception>
		public void AddTracerConfig(TracerConfig tracerConfig)
		{
			Contract.Requires<ArgumentNullException>(tracerConfig != null);
			Contract.Requires<ArgumentException>(tracerConfig.NamePrefix.Length > 0, "Adding a root TracerConfig is not supported; one already exists.");

			lock (_rootTracerConfig)
			{
				_rootTracerConfig.InsertNode(tracerConfig);
			}

			if (TracerConfigAdded != null)
			{
				TracerConfigAdded(this, new ConfigChangedEventArgs<TracerConfig>(tracerConfig));
			}
		}

		/// <summary>
		/// The remove tracer config.
		/// </summary>
		/// <param name="tracerConfig">
		/// The tracer config.
		/// </param>
		public void RemoveTracerConfig(TracerConfig tracerConfig)
		{
			Contract.Requires<ArgumentNullException>(tracerConfig != null);
			Contract.Requires<ArgumentException>(tracerConfig.NamePrefix.Length > 0, "Removing the root TracerConfig is not supported.");

			lock (_rootTracerConfig)
			{
				if (!_rootTracerConfig.RemoveDescendent(tracerConfig))
				{
					return;
				}
			}

			if (TracerConfigRemoved != null)
			{
				TracerConfigRemoved(this, new ConfigChangedEventArgs<TracerConfig>(tracerConfig));
			}
		}

		#endregion

		#region Methods

		#endregion
	}
}