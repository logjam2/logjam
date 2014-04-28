// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="Tracer.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Trace.Collectors;

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

		private ITraceCollector _traceCollector;

		private ITraceSwitch _traceSwitch;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Tracer"/> class.
		/// </summary>
		/// <param name="name">
		/// N
		/// The name.
		/// </param>
		/// <param name="traceSwitch">
		/// The trace Switch.
		/// </param>
		/// <param name="traceCollector">
		/// The message Collector.
		/// </param>
		internal Tracer(string name, ITraceSwitch traceSwitch, ITraceCollector traceCollector)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(name));
			Contract.Requires<ArgumentNullException>(traceSwitch != null);
			Contract.Requires<ArgumentNullException>(traceCollector != null);

			_name = name.Trim();
			_traceSwitch = traceSwitch;
			_traceCollector = traceCollector;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the trace switch.
		/// </summary>
		/// <value>
		/// TODO The trace switch.
		/// </value>
		public ITraceSwitch Switch
		{
			get
			{
				return _traceSwitch;
			}
		}

		/// <summary>
		/// Gets the collector.
		/// </summary>
		/// <value>
		/// TODO The collector.
		/// </value>
		public ITraceCollector Collector
		{
			get
			{
				return _traceCollector;
			}
		}

		/// <summary>
		/// Gets a value indicating whether debug tracing is enabled.
		/// </summary>
		/// <value>
		/// The is debug enabled.
		/// </value>
		public bool IsDebugEnabled
		{
			get
			{
				return IsTraceEnabled(TraceLevel.Debug);
			}
		}

		/// <summary>
		/// Gets a value indicating whether is error enabled.
		/// </summary>
		/// <value>
		/// The is error enabled.
		/// </value>
		public bool IsErrorEnabled
		{
			get
			{
				return IsTraceEnabled(TraceLevel.Error);
			}
		}

		/// <summary>
		/// Gets a value indicating whether is info enabled.
		/// </summary>
		/// <value>
		/// The is info enabled.
		/// </value>
		public bool IsInfoEnabled
		{
			get
			{
				return IsTraceEnabled(TraceLevel.Info);
			}
		}

		public bool IsSevereEnabled
		{
			get
			{
				return IsTraceEnabled(TraceLevel.Severe);
			}
		}

		/// <summary>
		/// Gets a value indicating whether is verbose enabled.
		/// </summary>
		/// <value>
		/// The is verbose enabled.
		/// </value>
		public bool IsVerboseEnabled
		{
			get
			{
				return IsTraceEnabled(TraceLevel.Verbose);
			}
		}

		/// <summary>
		/// Gets a value indicating whether is warn enabled.
		/// </summary>
		/// <value>
		/// The is warn enabled.
		/// </value>
		public bool IsWarnEnabled
		{
			get
			{
				return IsTraceEnabled(TraceLevel.Warn);
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The debug.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		public void Debug(string message)
		{
			Trace(TraceLevel.Debug, null, message);
		}

		/// <summary>
		/// The debug.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		public void Debug(string message, object arg0)
		{
			Trace(TraceLevel.Debug, null, message, arg0);
		}

		/// <summary>
		/// The debug.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		/// <param name="arg1">
		/// The arg 1.
		/// </param>
		public void Debug(string message, object arg0, object arg1)
		{
			Trace(TraceLevel.Debug, null, message, arg0, arg1);
		}

		/// <summary>
		/// The debug.
		/// </summary>
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
		public void Debug(string message, object arg0, object arg1, object arg2)
		{
			Trace(TraceLevel.Debug, null, message, arg0, arg1, arg2);
		}

		/// <summary>
		/// The debug.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public void Debug(string message, params object[] args)
		{
			Trace(TraceLevel.Debug, null, message, args);
		}

		/// <summary>
		/// The error.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		public void Error(string message)
		{
			Trace(TraceLevel.Error, null, message);
		}

		/// <summary>
		/// The error.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		public void Error(string message, object arg0)
		{
			Trace(TraceLevel.Error, null, message, arg0);
		}

		/// <summary>
		/// The error.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		/// <param name="arg1">
		/// The arg 1.
		/// </param>
		public void Error(string message, object arg0, object arg1)
		{
			Trace(TraceLevel.Error, null, message, arg0, arg1);
		}

		/// <summary>
		/// The error.
		/// </summary>
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
		public void Error(string message, object arg0, object arg1, object arg2)
		{
			Trace(TraceLevel.Error, null, message, arg0, arg1, arg2);
		}

		/// <summary>
		/// The error.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public void Error(string message, params object[] args)
		{
			Trace(TraceLevel.Error, null, message, args);
		}

		/// <summary>
		/// The error.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		public void Error(Exception exception, string message)
		{
			Trace(TraceLevel.Error, exception, message);
		}

		/// <summary>
		/// The error.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		public void Error(Exception exception, string message, object arg0)
		{
			Trace(TraceLevel.Error, exception, message, arg0);
		}

		/// <summary>
		/// The error.
		/// </summary>
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
		public void Error(Exception exception, string message, object arg0, object arg1)
		{
			Trace(TraceLevel.Error, exception, message, arg0, arg1);
		}

		/// <summary>
		/// The error.
		/// </summary>
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
		public void Error(Exception exception, string message, object arg0, object arg1, object arg2)
		{
			Trace(TraceLevel.Error, exception, message, arg0, arg1, arg2);
		}

		/// <summary>
		/// The error.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public void Error(Exception exception, string message, params object[] args)
		{
			Trace(TraceLevel.Error, exception, message, args);
		}

		/// <summary>
		/// The info.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		public void Info(string message)
		{
			Trace(TraceLevel.Info, null, message);
		}

		/// <summary>
		/// The info.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		public void Info(string message, object arg0)
		{
			Trace(TraceLevel.Info, null, message, arg0);
		}

		/// <summary>
		/// The info.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		/// <param name="arg1">
		/// The arg 1.
		/// </param>
		public void Info(string message, object arg0, object arg1)
		{
			Trace(TraceLevel.Info, null, message, arg0, arg1);
		}

		/// <summary>
		/// The info.
		/// </summary>
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
		public void Info(string message, object arg0, object arg1, object arg2)
		{
			Trace(TraceLevel.Info, null, message, arg0, arg1, arg2);
		}

		/// <summary>
		/// The info.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public void Info(string message, params object[] args)
		{
			Trace(TraceLevel.Info, null, message, args);
		}

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
			return _traceCollector.IsActive && _traceSwitch.IsEnabled(this, traceLevel);
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

			if (_traceCollector.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				_traceCollector.Append(this, traceLevel, message, exception);
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

			if (_traceCollector.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				message = string.Format(message, arg0);
				_traceCollector.Append(this, traceLevel, message, exception);
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

			if (_traceCollector.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				message = string.Format(message, arg0, arg1);
				_traceCollector.Append(this, traceLevel, message, exception);
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

			if (_traceCollector.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				message = string.Format(message, arg0, arg1, arg2);
				_traceCollector.Append(this, traceLevel, message, exception);
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

			if (_traceCollector.IsActive && _traceSwitch.IsEnabled(this, traceLevel))
			{
				message = string.Format(message, args);
				_traceCollector.Append(this, traceLevel, message, exception);
			}
		}

		/// <summary>
		/// The verbose.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		public void Verbose(string message)
		{
			Trace(TraceLevel.Verbose, null, message);
		}

		/// <summary>
		/// The verbose.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		public void Verbose(string message, object arg0)
		{
			Trace(TraceLevel.Verbose, null, message, arg0);
		}

		/// <summary>
		/// The verbose.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		/// <param name="arg1">
		/// The arg 1.
		/// </param>
		public void Verbose(string message, object arg0, object arg1)
		{
			Trace(TraceLevel.Verbose, null, message, arg0, arg1);
		}

		/// <summary>
		/// The verbose.
		/// </summary>
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
		public void Verbose(string message, object arg0, object arg1, object arg2)
		{
			Trace(TraceLevel.Verbose, null, message, arg0, arg1, arg2);
		}

		/// <summary>
		/// The verbose.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public void Verbose(string message, params object[] args)
		{
			Trace(TraceLevel.Verbose, null, message, args);
		}

		/// <summary>
		/// The verbose.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		public void Verbose(Exception exception, string message)
		{
			Trace(TraceLevel.Verbose, exception, message);
		}

		/// <summary>
		/// The verbose.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		public void Verbose(Exception exception, string message, object arg0)
		{
			Trace(TraceLevel.Verbose, exception, message, arg0);
		}

		/// <summary>
		/// The verbose.
		/// </summary>
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
		public void Verbose(Exception exception, string message, object arg0, object arg1)
		{
			Trace(TraceLevel.Verbose, exception, message, arg0, arg1);
		}

		/// <summary>
		/// The verbose.
		/// </summary>
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
		public void Verbose(Exception exception, string message, object arg0, object arg1, object arg2)
		{
			Trace(TraceLevel.Verbose, exception, message, arg0, arg1, arg2);
		}

		/// <summary>
		/// The verbose.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public void Verbose(Exception exception, string message, params object[] args)
		{
			Trace(TraceLevel.Verbose, exception, message, args);
		}

		/// <summary>
		/// The warn.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		public void Warn(string message)
		{
			Trace(TraceLevel.Warn, null, message);
		}

		/// <summary>
		/// The warn.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		public void Warn(string message, object arg0)
		{
			Trace(TraceLevel.Warn, null, message, arg0);
		}

		/// <summary>
		/// The warn.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		/// <param name="arg1">
		/// The arg 1.
		/// </param>
		public void Warn(string message, object arg0, object arg1)
		{
			Trace(TraceLevel.Warn, null, message, arg0, arg1);
		}

		/// <summary>
		/// The warn.
		/// </summary>
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
		public void Warn(string message, object arg0, object arg1, object arg2)
		{
			Trace(TraceLevel.Warn, null, message, arg0, arg1, arg2);
		}

		/// <summary>
		/// The warn.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public void Warn(string message, params object[] args)
		{
			Trace(TraceLevel.Warn, null, message, args);
		}

		/// <summary>
		/// The warn.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		public void Warn(Exception exception, string message)
		{
			Trace(TraceLevel.Warn, exception, message);
		}

		/// <summary>
		/// The warn.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="arg0">
		/// The arg 0.
		/// </param>
		public void Warn(Exception exception, string message, object arg0)
		{
			Trace(TraceLevel.Warn, exception, message, arg0);
		}

		/// <summary>
		/// The warn.
		/// </summary>
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
		public void Warn(Exception exception, string message, object arg0, object arg1)
		{
			Trace(TraceLevel.Warn, exception, message, arg0, arg1);
		}

		/// <summary>
		/// The warn.
		/// </summary>
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
		public void Warn(Exception exception, string message, object arg0, object arg1, object arg2)
		{
			Trace(TraceLevel.Warn, exception, message, arg0, arg1, arg2);
		}

		/// <summary>
		/// The warn.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="args">
		/// The args.
		/// </param>
		public void Warn(Exception exception, string message, params object[] args)
		{
			Trace(TraceLevel.Warn, exception, message, args);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Configuration method called by <see cref="TraceManager"/> after when <see cref="TraceManager.RegisterTracer"/> is called,
		/// or when the configuration is changed for this <c>Tracer</c>.
		/// </summary>
		/// <param name="traceSwitch">
		/// </param>
		/// <param name="traceCollector">
		/// </param>
		internal void Configure(ITraceSwitch traceSwitch, ITraceCollector traceCollector)
		{
			// REVIEW: This is a lockless set - ok?
			// The expectation is that these values are set infrequently, and it doesn't matter if the switch changes before the collector does
			_traceSwitch = traceSwitch;
			_traceCollector = traceCollector;
		}

		#endregion
	}
}