// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="TraceManager.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Threading;

	using LogJam.Trace.Config;

	/// <summary>
	/// Entry point for everything related to tracing.
	/// </summary>
	public class TraceManager : ITracerFactory
	{
		#region Static fields

		private static TraceManager s_instance;

		#endregion
		#region Instance fields

		private readonly TraceManagerConfig _traceConfig = new TraceManagerConfig();

		private readonly Dictionary<string, WeakReference> _tracers = new Dictionary<string, WeakReference>(100);

		#endregion

		public static TraceManager Instance
		{
			get
			{
				if (s_instance != null)
				{
					return s_instance;
				}

				Interlocked.CompareExchange(ref s_instance, new TraceManager(), null);
				return s_instance;
			}
		}

		#region Constructors and Destructors

		/// <summary>
		/// Creates a new <see cref="TraceManager"/> instance.
		/// </summary>
		public TraceManager()
		{
			_traceConfig.TracerConfigAdded += OnTracerConfigAddedOrRemoved;
			_traceConfig.TracerConfigRemoved += OnTracerConfigAddedOrRemoved;
		}

		public void Dispose()
		{
			// TODO: Add support for cleanup.
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the config.
		/// </summary>
		/// <value>
		/// The config.
		/// </value>
		public TraceManagerConfig Config
		{
			get
			{
				return _traceConfig;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The get tracer.
		/// </summary>
		/// <param name="name">
		/// The name.
		/// </param>
		/// <returns>
		/// The <see cref="Tracer"/>.
		/// </returns>
		public Tracer GetTracer(string name)
		{
			//Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(name));

			name = name.Trim();

			// Lookup the Tracer, or add a new one
			WeakReference weakRefTracer;
			lock (_tracers)
			{
				if (_tracers.TryGetValue(name, out weakRefTracer))
				{
					object objTracer = weakRefTracer.Target;
					if (objTracer == null)
					{
						_tracers.Remove(name);
					}
					else
					{
						// Return the existing Tracer
						return (Tracer)objTracer;
					}
				}

				// Create a new Tracer and register it
				TracerConfig tracerConfig = _traceConfig.RootTracerConfig.FindNearestParentOf(name);
				Tracer tracer = new Tracer(name, tracerConfig.TraceSwitch, tracerConfig.TraceWriter);
				_tracers[name] = new WeakReference(tracer);
				return tracer;
			}
		}

		#endregion

		#region Methods

		private List<Tracer> FindMatchingTracersFor(TracerConfig tracerConfig)
		{
			List<Tracer> matchingTracers = new List<Tracer>();
			lock (_tracers)
			{
				foreach (KeyValuePair<string, WeakReference> kvp in _tracers)
				{
					Tracer tracer = (Tracer)kvp.Value.Target;
					if (tracer == null)
					{
						_tracers.Remove(kvp.Key);
					}
					else
					{
						if (tracer.Name.StartsWith(tracerConfig.NamePrefix))
						{
							matchingTracers.Add(tracer);
						}
					}
				}
			}

			return matchingTracers;
		}

		private void OnTracerConfigAddedOrRemoved(object sender, ConfigChangedEventArgs<TracerConfig> e)
		{
			// Re-configure each existing Tracer that prefix matches the TracerConfig being added or removed
			foreach (Tracer tracer in FindMatchingTracersFor(e.ConfigChanged))
			{
				// REVIEW: It's probably best if TracerConfig TraceSwitch and TraceWriter are always populated
				ITraceSwitch traceSwitch = e.ConfigChanged.TraceSwitch ?? tracer.Switch;
				ITraceWriter traceWriter = e.ConfigChanged.TraceWriter ?? tracer.Writer;
				tracer.Configure(traceSwitch, traceWriter);
			}
		}

		#endregion
	}
}