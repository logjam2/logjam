// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupLog.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using LogJam.Trace;
	using LogJam.Trace.Switches;
	using LogJam.Writer;
	using System.Collections;
	using System.Collections.Generic;


	/// <summary>
	/// Default <see cref="ITracerFactory"/> for tracking logjam configuration, startup, shutdown, logging exceptions, and other status.
	/// </summary>
	public sealed class SetupLog : ITracerFactory, IEnumerable<TraceEntry>, IEntryWriter<TraceEntry>
	{
		private readonly Dictionary<string, Tracer> _tracers;
		private readonly List<TraceEntry> _listEntries;
		private readonly TraceWriter _traceWriter;
		private readonly TraceWriter[] _traceWriters;

		/// <summary>
		/// Creates a new <see cref="SetupLog"/> instance which records all status entries.
		/// </summary>
		public SetupLog()
			: this(new OnOffTraceSwitch(true))
		{}

		/// <summary>
		/// Creates a new <see cref="SetupLog"/> instance which records status entries according to <paramref name="statusTraceSwitch"/>.
		/// </summary>
		/// <param name="statusTraceSwitch"></param>
		public SetupLog(ITraceSwitch statusTraceSwitch)
		{
			_tracers = new Dictionary<string, Tracer>();
			_listEntries = new List<TraceEntry>();
			_traceWriter = new TraceWriter(statusTraceSwitch, this, null);
			_traceWriters = new[] { _traceWriter };
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
			return _listEntries.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Enabled { get { return true; } }

		public void Write(ref TraceEntry entry)
		{
			lock (_listEntries)
			{
				_listEntries.Add(entry);				
			}
		}

		/// <summary>
		/// Clears all the accumulated trace entries.
		/// </summary>
		public void Clear()
		{
			lock (_listEntries)
			{
				_listEntries.Clear();
			}
		}
	}

}