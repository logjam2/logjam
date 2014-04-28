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

	using LogJam.Trace.Collectors;
	using LogJam.Trace.Config;

	/// <summary>
	/// Entry point for everything related to tracing and logging.
	/// </summary>
	public static class TraceManager
	{
		#region Static Fields

		private static readonly TraceConfig s_traceConfig = new TraceConfig();

		private static readonly Dictionary<string, WeakReference> s_tracers = new Dictionary<string, WeakReference>(100);

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes static members of the <see cref="TraceManager"/> class.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static TraceManager()
		{
			s_traceConfig.TracerConfigAdded += OnTracerConfigAddedOrRemoved;
			s_traceConfig.TracerConfigRemoved += OnTracerConfigAddedOrRemoved;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the config.
		/// </summary>
		/// <value>
		/// The config.
		/// </value>
		public static TraceConfig Config
		{
			get
			{
				return s_traceConfig;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The get tracer.
		/// </summary>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <returns>
		/// The <see cref="Tracer"/>.
		/// </returns>
		public static Tracer GetTracer(Type type)
		{
			Contract.Requires<ArgumentNullException>(type != null);

			// Convert generic types to their generic type definition - so the same
			// Tracer is used for ArrayList<T> regardless of the type parameter T.
			if (type.IsGenericType)
			{
				type = type.GetGenericTypeDefinition();
			}

			return GetTracer(type.FullName);
		}

		/// <summary>
		/// The get tracer.
		/// </summary>
		/// <param name="name">
		/// The name.
		/// </param>
		/// <returns>
		/// The <see cref="Tracer"/>.
		/// </returns>
		public static Tracer GetTracer(string name)
		{
			Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(name));

			name = name.Trim();

			// Lookup the Tracer, or add a new one
			WeakReference weakRefTracer;
			lock (s_tracers)
			{
				if (s_tracers.TryGetValue(name, out weakRefTracer))
				{
					object objTracer = weakRefTracer.Target;
					if (objTracer == null)
					{
						s_tracers.Remove(name);
					}
					else
					{
						// Return the existing Tracer
						return (Tracer)objTracer;
					}
				}

				// Create a new Tracer and register it
				TracerConfig tracerConfig = s_traceConfig.RootTracerConfig.FindNearestParentOf(name);
				Tracer tracer = new Tracer(name, tracerConfig.TraceSwitch, tracerConfig.TraceCollector);
				s_tracers[name] = new WeakReference(tracer);
				return tracer;
			}
		}

		#endregion

		#region Methods

		private static List<Tracer> FindMatchingTracersFor(TracerConfig tracerConfig)
		{
			List<Tracer> matchingTracers = new List<Tracer>();
			lock (s_tracers)
			{
				foreach (KeyValuePair<string, WeakReference> kvp in s_tracers)
				{
					Tracer tracer = (Tracer)kvp.Value.Target;
					if (tracer == null)
					{
						s_tracers.Remove(kvp.Key);
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

		private static void OnTracerConfigAddedOrRemoved(object sender, ConfigChangedEventArgs<TracerConfig> e)
		{
			// Re-configure each existing Tracer that prefix matches the TracerConfig being added or removed
			foreach (Tracer tracer in FindMatchingTracersFor(e.ConfigChanged))
			{
				// REVIEW: It's probably best if TracerConfig TraceSwitch and TraceCollector are always populated
				ITraceSwitch traceSwitch = e.ConfigChanged.TraceSwitch ?? tracer.Switch;
				ITraceCollector traceCollector = e.ConfigChanged.TraceCollector ?? tracer.Collector;
				tracer.Configure(traceSwitch, traceCollector);
			}
		}

		#endregion
	}
}