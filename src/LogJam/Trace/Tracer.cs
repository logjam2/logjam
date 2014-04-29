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


	/// <summary>
	/// API for logging trace messages.
	/// </summary>
	/// <example>
	/// <c>Tracer</c> instances are typically used as follows:
	/// <code>
	/// private readonly Tracer _tracer = TraceManager.GetTracer(GetType());
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
		#region Fields

		private readonly string _name;

		private ITraceSwitch _traceSwitch;
		private ITraceWriter _traceWriter;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Tracer"/> class.
		/// </summary>
		/// <param name="name">The <see cref="Tracer"/> name.  Uniquely identifies a <see cref="Tracer"/> within an <see cref="ITracerFactory"/>.
		/// Often is the class name or namespace name.</param>
		/// <param name="traceSwitch">
		/// The trace Switch.
		/// </param>
		/// <param name="traceWriter">
		/// The <see cref="ITraceWriter"/>.
		/// </param>
		internal Tracer(string name, ITraceSwitch traceSwitch, ITraceWriter traceWriter)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(name));
			Contract.Requires<ArgumentNullException>(traceSwitch != null);
			Contract.Requires<ArgumentNullException>(traceWriter != null);

			_name = name.Trim();
			_traceSwitch = traceSwitch;
			_traceWriter = traceWriter;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get { return _name; } }

		/// <summary>
		/// Gets the <see cref="ITraceSwitch"/>.
		/// </summary>
		/// <value>
		/// TODO The trace switch.
		/// </value>
		public ITraceSwitch Switch { get { return _traceSwitch; } }

		/// <summary>
		/// Gets the <see cref="ITraceWriter"/>.
		/// </summary>
		/// <value>
		/// TODO The collector.
		/// </value>
		public ITraceWriter Writer { get { return _traceWriter; } }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The is trace enabled.
		/// </summary>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool IsTraceEnabled(TraceLevel traceLevel)
		{
			return _traceWriter.IsActive && _traceSwitch.IsEnabled(this, traceLevel);
		}

		/// <summary>
		/// The trace.
		/// </summary>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		public void Trace(TraceLevel traceLevel, Exception exception, string message)
		{
			Contract.Requires<ArgumentNullException>(message != null);

			if (_traceWriter.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				_traceWriter.Write(this, traceLevel, message, exception);
			}
		}

		/// <summary>
		/// The trace.
		/// </summary>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		public void Trace(TraceLevel traceLevel, Exception exception, string message, object arg0)
		{
			Contract.Requires<ArgumentNullException>(message != null);

			if (_traceWriter.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				message = string.Format(message, arg0);
				_traceWriter.Write(this, traceLevel, message, exception);
			}
		}

		/// <summary>
		/// The trace.
		/// </summary>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		/// <param name="arg1">
		/// The arg 1.
		/// </param>
		public void Trace(TraceLevel traceLevel, Exception exception, string message, object arg0, object arg1)
		{
			Contract.Requires<ArgumentNullException>(message != null);

			if (_traceWriter.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				message = string.Format(message, arg0, arg1);
				_traceWriter.Write(this, traceLevel, message, exception);
			}
		}

		/// <summary>
		/// The trace.
		/// </summary>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		/// <param name="arg1">
		/// The arg 1.
		/// </param>
		/// <param name="arg2">
		/// The arg 2.
		/// </param>
		public void Trace(TraceLevel traceLevel, Exception exception, string message, object arg0, object arg1, object arg2)
		{
			Contract.Requires<ArgumentNullException>(message != null);

			if (_traceWriter.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				message = string.Format(message, arg0, arg1, arg2);
				_traceWriter.Write(this, traceLevel, message, exception);
			}
		}

		/// <summary>
		/// The trace.
		/// </summary>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public void Trace(TraceLevel traceLevel, Exception exception, string message, params object[] args)
		{
			Contract.Requires<ArgumentNullException>(message != null);
			Contract.Requires<ArgumentNullException>(args != null);

			if (_traceWriter.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				message = string.Format(message, args);
				_traceWriter.Write(this, traceLevel, message, exception);
			}
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Configuration method called by <see cref="TraceManager"/> after when <see cref="TraceManager.RegisterTracer"/> is called,
		/// or when the configuration is changed for this <c>Tracer</c>.
		/// </summary>
		/// <param name="traceSwitch">
		/// </param>
		/// <param name="traceWriter">
		/// </param>
		internal void Configure(ITraceSwitch traceSwitch, ITraceWriter traceWriter)
		{
			// REVIEW: This is a lockless set - ok?
			// The expectation is that these values are set infrequently, and it doesn't matter if the switch changes before the collector does
			_traceSwitch = traceSwitch;
			_traceWriter = traceWriter;
		}

		#endregion
	}
}
