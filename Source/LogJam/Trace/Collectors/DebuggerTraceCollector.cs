// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="DebuggerTraceCollector.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace.Collectors
{
	using System;
	using System.Diagnostics;

	using LogJam.Trace.Formatters;

	/// <summary>
	/// An <see cref="ITraceCollector"/> that writes trace output to the debug window, if and when a debugger is attached.
	/// </summary>
	public class DebuggerTraceCollector : ITraceCollector
	{
		#region Static Fields

		private static readonly DebuggerTraceCollector s_instance = new DebuggerTraceCollector();

		#endregion

		#region Constructors and Destructors

		private DebuggerTraceCollector()
		{
			Formatter = new DebuggerTraceFormatter();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the singleton <see cref="DebuggerTraceCollector"/> instance.  A singleton is used to prevent multiple <c>DebuggerTraceCollector</c>
		/// instances from writing to the debug window at the same time, thus intermixing text from multiple sources.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static DebuggerTraceCollector Instance
		{
			get
			{
				return s_instance;
			}
		}

		/// <summary>
		/// Gets the formatter.
		/// </summary>
		/// <value>
		/// The formatter.
		/// </value>
		public TraceFormatter Formatter { get; private set; }

		/// <summary>
		/// Gets a value indicating whether is active.
		/// </summary>
		/// <value>
		/// TODO The is active.
		/// </value>
		public bool IsActive
		{
			get
			{
				return Debugger.IsAttached;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The append.
		/// </summary>
		/// <param name="tracer">
		/// The tracer.
		/// </param>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		public void Append(Tracer tracer, TraceLevel traceLevel, string message, Exception exception)
		{
			string debuggerMessage = Formatter.FormatTrace(tracer.Name, traceLevel, message, exception);
			Debug.WriteLine(debuggerMessage);
		}

		/// <summary>
		/// TODO The append message.
		/// </summary>
		/// <param name="tracer">
		/// TODO The tracer.
		/// </param>
		/// <param name="traceLevel">
		/// TODO The trace level.
		/// </param>
		/// <param name="message">
		/// TODO The message.
		/// </param>
		/// <param name="exception">
		/// TODO The exception.
		/// </param>
		/// <exception cref="NotImplementedException">
		/// </exception>
		public void AppendMessage(Tracer tracer, TraceLevel traceLevel, string message, Exception exception)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}