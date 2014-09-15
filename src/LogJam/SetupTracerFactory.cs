// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupTracerFactory.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using System.Collections;
	using System.Collections.Generic;

	using LogJam.Trace;
	using LogJam.Trace.Switches;
	using LogJam.Writers;


	/// <summary>
	/// Default <see cref="ITracerFactory"/> for tracking logjam configuration, startup, shutdown, logging exceptions, and other status.
	/// </summary>
	public sealed class SetupTracerFactory : ITracerFactory, IEnumerable<TraceEntry>
	{
		private readonly Dictionary<string, Tracer> _tracers;
		private readonly ListLogWriter<TraceEntry> _listLogWriter;
		private readonly TraceWriter _traceWriter;
		private readonly TraceWriter[] _traceWriters;

		/// <summary>
		/// Creates a new <see cref="SetupTracerFactory"/> instance which records all status entries.
		/// </summary>
		public SetupTracerFactory()
			: this(new OnOffTraceSwitch(true))
		{}

		/// <summary>
		/// Creates a new <see cref="SetupTracerFactory"/> instance which records status entries according to <paramref name="statusTraceSwitch"/>.
		/// </summary>
		/// <param name="statusTraceSwitch"></param>
		public SetupTracerFactory(ITraceSwitch statusTraceSwitch)
		{
			_tracers = new Dictionary<string, Tracer>();
			_listLogWriter = new ListLogWriter<TraceEntry>(true);
			_listLogWriter.Start(); // It's always started/enabled.
			_traceWriter = new TraceWriter(statusTraceSwitch, _listLogWriter, null);
			_traceWriters = new TraceWriter[] { _traceWriter };
		}

		public void Dispose()
		{}

		/// <inheritdoc />
		public Tracer GetTracer(string name)
		{
			if (name == null)
			{
				name = string.Empty;
			}

			name = name.Trim();

			lock (_tracers)
			{
				Tracer tracer;
				if (_tracers.TryGetValue(name, out tracer))
				{
					return tracer;
				}

				// Create a new Tracer 
				tracer = new Tracer(name, _traceWriters);
				return tracer;
			}
		}

		public IEnumerator<TraceEntry> GetEnumerator()
		{
			return _listLogWriter.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

	}

}