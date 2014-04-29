

using System.Diagnostics;

namespace LogJam.Trace.Writers
{

	/// <summary>
	/// An <see cref="ITraceWriter"/> that does nothing.
	/// </summary>
	public sealed class NoOpTraceWriter : ITraceWriter
	{

		public bool IsActive { get { return false; } }

		public void Write(Tracer tracer, TraceLevel traceLevel, string message, object details)
		{}

	}

}