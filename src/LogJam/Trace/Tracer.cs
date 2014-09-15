// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tracer.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq;


	/// <summary>
	/// API for logging trace messages.
	/// </summary>
	/// <example>
	/// <c>Tracer</c> instances are typically used as follows:
	/// <code>
	/// private readonly Tracer _tracer = myTraceManager.GetTracer(GetType());
	/// ...
	/// _tracer.Info("User entered '{0}'", userText);
	/// ...
	/// _tracer.Warn(excp, "Exception caught for user {0}", User.Identity.Name);
	/// </code>
	/// </example>
	/// <c>Tracer</c> instances cannot be directly instantiated - to create or obtain a <c>Tracer</c>, always use
	///  <see cref="TraceManager.GetTracer(string)"/>.
	public sealed class Tracer
	{

		/// <summary>
		/// The <c>Tracer</c> <see cref="Name"/> pattern that matches all <see cref="Tracer"/>s.
		/// </summary>
		public const string All = "";

		#region Fields

		private readonly string _name;

		// TODO: Which provides better perf - volatile, or lock(this)?
		private ITraceWriter _writer;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Tracer"/> class.
		/// </summary>
		/// <param name="name">The <see cref="Tracer"/> name.  Uniquely identifies a <see cref="Tracer"/> within an <see cref="ITracerFactory"/>.
		/// Often is the class name or namespace name.</param>
		/// <param name="traceWriter">
		/// The <see cref="TraceWriter"/>s to attach to this <c>Tracer</c>.
		/// </param>
		internal Tracer(string name, TraceWriter[] traceWriters)
		{
			Contract.Requires<ArgumentNullException>(name != null);
			Contract.Requires<ArgumentNullException>(traceWriters != null);

			_name = name.Trim();
			Configure(traceWriters);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The name of this <see cref="Tracer"/>.  This is usually the full name of the class that is calling <c>Tracer</c> methods.
		/// </summary>
		public string Name { get { return _name; } }

		/// <summary>
		/// Gets and sets the <see cref="ITraceWriter"/> that writes <c>Tracer</c> output.
		/// </summary>
		internal ITraceWriter Writer
		{
			get { return _writer; }
			set { _writer = value; }
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Returns <c>true</c> if a trace call with <paramref name="traceLevel"/> will be logged.
		/// </summary>
		/// <param name="traceLevel">
		/// A <see cref="TraceLevel"/> for a trace call.
		/// </param>
		/// <returns>
		/// <c>true</c> if the specified <paramref name="traceLevel"/> will be logged.
		/// </returns>
		public bool IsTraceEnabled(TraceLevel traceLevel)
		{
			return _writer.IsTraceEnabled(_name, traceLevel);
		}

		private void WriteTraceEntry(ref TraceEntry traceEntry)
		{
			_writer.Write(ref traceEntry);
		}

		public void Trace(TraceLevel traceLevel, object details, string message)
		{
			Contract.Requires<ArgumentNullException>(message != null);

			if (IsTraceEnabled(traceLevel))
			{
				TraceEntry traceEntry = new TraceEntry()
				                        {
					                        TimestampUtc = DateTime.UtcNow,
					                        TracerName = Name,
					                        TraceLevel = traceLevel,
					                        Message = message,
											Details = details
				                        };
				WriteTraceEntry(ref traceEntry);
			}
		}

		public void Trace(TraceLevel traceLevel, Exception exception, string message, object arg0)
		{
			Contract.Requires<ArgumentNullException>(message != null);

			if (IsTraceEnabled(traceLevel))
			{
				message = string.Format(message, arg0);
				TraceEntry traceEntry = new TraceEntry()
				{
					TimestampUtc = DateTime.UtcNow,
					TracerName = Name,
					TraceLevel = traceLevel,
					Message = message,
					Details = exception
				};
				WriteTraceEntry(ref traceEntry);
			}
		}

		public void Trace(TraceLevel traceLevel, Exception exception, string message, object arg0, object arg1)
		{
			Contract.Requires<ArgumentNullException>(message != null);

			if (IsTraceEnabled(traceLevel))
			{
				message = string.Format(message, arg0, arg1);
				TraceEntry traceEntry = new TraceEntry()
				{
					TimestampUtc = DateTime.UtcNow,
					TracerName = Name,
					TraceLevel = traceLevel,
					Message = message,
					Details = exception
				};
				WriteTraceEntry(ref traceEntry);
			}
		}

		public void Trace(TraceLevel traceLevel, Exception exception, string message, object arg0, object arg1, object arg2)
		{
			Contract.Requires<ArgumentNullException>(message != null);

			if (IsTraceEnabled(traceLevel))
			{
				message = string.Format(message, arg0, arg1, arg2);
				TraceEntry traceEntry = new TraceEntry()
				{
					TimestampUtc = DateTime.UtcNow,
					TracerName = Name,
					TraceLevel = traceLevel,
					Message = message,
					Details = exception
				};
				WriteTraceEntry(ref traceEntry);
			}
		}

		public void Trace(TraceLevel traceLevel, Exception exception, string message, params object[] args)
		{
			Contract.Requires<ArgumentNullException>(message != null);
			Contract.Requires<ArgumentNullException>(args != null);

			if (IsTraceEnabled(traceLevel))
			{
				message = string.Format(message, args);
				TraceEntry traceEntry = new TraceEntry()
				{
					TimestampUtc = DateTime.UtcNow,
					TracerName = Name,
					TraceLevel = traceLevel,
					Message = message,
					Details = exception
				};
				WriteTraceEntry(ref traceEntry);
			}
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Configuration method called by <see cref="TraceManager"/> after <see cref="TraceManager.GetTracer"/> is called,
		/// or when the configuration is changed for this <c>Tracer</c>.
		/// </summary>
		/// <param name="traceWriters">The array of 0 or more <see cref="TraceWriter"/>s used by this <see cref="Tracer"/> to write trace messages.</param>
		/// <returns>The previous <see cref="TraceWriter"/>s, or <c>null</c> if no <see cref="TraceWriter"/>s were previously configured..</returns>
		internal TraceWriter[] Configure(TraceWriter[] traceWriters)
		{
			Contract.Requires<ArgumentNullException>(traceWriters != null);
			Contract.Requires<ArgumentException>(traceWriters.All(writer => writer != null));

			ITraceWriter newWriter;
			if (traceWriters.Length == 0)
			{
				newWriter = new NoOpTraceWriter();
			}
			else if (traceWriters.Length == 1)
			{
				newWriter = traceWriters[0];
			}
			else
			{
				newWriter = new MultiTraceWriter(traceWriters);
			}

			TraceWriter[] previousTraceWriters = _writer == null ? null : _writer.ToTraceWriterArray();

			// REVIEW: This is a lockless set - ok?
			// The expectation is that these values are set infrequently, and it doesn't matter if the switch changes before the traceLogWriter does
			_writer = newWriter;

			return previousTraceWriters;
		}

		#endregion
	}
}
