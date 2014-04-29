// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerTraceWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace.Writers
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;

	using LogJam.Trace.Formatters;

	using TraceLevel = LogJam.Trace.TraceLevel;


	/// <summary>
	/// An <see cref="ITraceWriter"/> that writes trace output to the debug window, if and when a debugger is attached.
	/// </summary>
	public class DebuggerTraceWriter : ITraceWriter
	{
		#region Static Fields

		private static readonly DebuggerTraceWriter s_instance = new DebuggerTraceWriter();

		#endregion

		#region Constructors and Destructors

		private DebuggerTraceWriter()
		{
			Formatter = new DebuggerTraceFormatter()
			            {
				            IncludeTimestamp = true
			            };
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the singleton <see cref="DebuggerTraceWriter"/> instance.  A singleton is used to prevent multiple <c>DebuggerTraceCollector</c>
		/// instances from writing to the debug window at the same time, thus intermixing text from multiple sources.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static DebuggerTraceWriter Instance { get { return s_instance; } }

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
			get { return true; // System.Diagnostics.Debugger.IsLogging();
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
		/// <param name="details">
		/// Trace message details, like an exception.
		/// </param>
		public void Write(Tracer tracer, TraceLevel traceLevel, string message, object details = null)
		{
			DateTime traceTimestampUtc = DateTime.UtcNow;
			string debuggerMessage = Formatter.FormatTrace(traceTimestampUtc, tracer.Name, traceLevel, message, details);

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
