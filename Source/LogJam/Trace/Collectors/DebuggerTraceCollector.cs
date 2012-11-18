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
	using System.Runtime.InteropServices;
	using System.Runtime.Versioning;

	using LogJam.Trace.Formatters;

	using TraceLevel = LogJam.Trace.TraceLevel;

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
			Formatter = new DebuggerTraceFormatter() 
							{
				                IncludeTimestamp = true
			                };
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
		public ITraceFormatter Formatter { get; private set; }

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
				return true; // System.Diagnostics.Debugger.IsLogging();
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
			DateTime traceTimestampUtc = DateTime.UtcNow;
			string debuggerMessage = Formatter.FormatTrace(traceTimestampUtc, tracer.Name, traceLevel, message, exception);

#if (PORTABLE)
			// REVIEW: This isn't reliable - it is conditionally compiled in debug builds; but it's all that's available in the portable profile.
			Debug.WriteLine(debuggerMessage);
#else
			if (Debugger.IsLogging())
			{
				//Console.Out.WriteLine("Debugger.Log: " + debuggerMessage);
				Debugger.Log(0, null, debuggerMessage);
			}
			else if (IsDebuggerPresent())
			{
				//Console.Out.WriteLine("OutputDebugString: " + debuggerMessage);
				OutputDebugString(debuggerMessage);
			} 
#endif
		}

		#endregion

#if !PORTABLE
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		internal static extern bool IsDebuggerPresent();

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern void OutputDebugString(string message);
#endif
	}
}