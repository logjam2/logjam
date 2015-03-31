// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FanOutTraceWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
	using System.Linq;

	using LogJam.Writer;


	/// <summary>
	/// Writes <see cref="TraceEntry"/>s to a series of <see cref="TraceWriter"/>s.
	/// </summary>
	/// <remarks><c>FanOutTraceWriter</c> instances are immutable.</remarks>
	internal sealed class FanOutTraceWriter : FanOutEntryWriter<TraceEntry>, ITraceWriter
	{

		private readonly TraceWriter[] _traceWriters;

		public FanOutTraceWriter(params TraceWriter[] innerTraceWriters)
			: base(innerTraceWriters.Cast<IEntryWriter<TraceEntry>>().ToArray())
		{
			_traceWriters = innerTraceWriters;
		}

		public bool IsTraceEnabled(string tracerName, TraceLevel traceLevel)
		{
			for (int i = 0; i < _traceWriters.Length; ++i)
			{
				if (_traceWriters[i].IsTraceEnabled(tracerName, traceLevel))
				{
					return true;
				}
			}
			return false;
		}

		public TraceWriter[] ToTraceWriterArray()
		{
			return _traceWriters;
		}

		public override void Write(ref TraceEntry entry)
		{
			for (int i = 0; i < _traceWriters.Length; ++i)
			{
				_traceWriters[i].Write(ref entry);
			}
		}

	}

}