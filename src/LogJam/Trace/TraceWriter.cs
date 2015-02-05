// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="traceLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Threading;

	using LogJam.Writers;


	/// <summary>
	/// Writes <see cref="TraceEntry"/>s to a <see cref="ILogWriter{TEntry}"/>, only if the
	/// <see cref="ITraceSwitch"/> allows the write.
	/// </summary>
	/// <remarks><c>TraceWriter</c> instances are thread-safe.</remarks>
	/// <seealso cref="FanOutTraceWriter"/>
	internal sealed class TraceWriter : DelegatingLogWriter<TraceEntry>, ITraceWriter
	{
		private readonly ITraceSwitch _traceSwitch;
		private readonly ITracerFactory _setupTracerFactory;
		// TODO: Expose this via status, instead of adding a single Setup tracer message
		private long _countWritingExceptions;

		/// <summary>
		/// Creates a new <see cref="TraceWriter"/> using the specified <paramref name="traceSwitch"/> and <paramref name="traceLogWriter"/>.
		/// </summary>
		/// <param name="traceSwitch"></param>
		/// <param name="traceLogWriter"></param>
		/// <param name="setupTracerFactory">The <see cref="ITracerFactory"/> to use to report exceptions.  If <c>null</c>, logging exceptions are not reported.</param>
		public TraceWriter(ITraceSwitch traceSwitch, ILogWriter<TraceEntry> traceLogWriter, ITracerFactory setupTracerFactory)
			: base(traceLogWriter)
		{
			Contract.Requires<ArgumentNullException>(traceSwitch != null);

			_traceSwitch = traceSwitch;
			_setupTracerFactory = setupTracerFactory;
			_countWritingExceptions = 0;
		}

		/// <inheritdoc />
		public bool IsTraceEnabled(string tracerName, TraceLevel traceLevel)
		{
			return InnerLogWriter.Enabled && _traceSwitch.IsEnabled(tracerName, traceLevel);
		}

		public TraceWriter[] ToTraceWriterArray()
		{
			return new[] { this };
		}

		/// <inheritdoc />
		public override void Write(ref TraceEntry entry)
		{
			if (IsTraceEnabled(entry.TracerName, entry.TraceLevel))
			{
				try
				{
					InnerLogWriter.Write(ref entry);
				}
				catch (Exception excp)
				{
					if (1 == Interlocked.Increment(ref _countWritingExceptions))
					{
						// TODO: Replace this with status polling
						if (_setupTracerFactory != null)
						{
							_setupTracerFactory.TracerFor(this).Error(excp, "At least one exception occurred while writing trace entyrs to " + InnerLogWriter);
						}
					}
				}
			}
		}

	}

}