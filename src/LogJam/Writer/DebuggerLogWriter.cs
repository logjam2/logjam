// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Writer
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Threading;

	using LogJam.Trace;


	/// <summary>
	/// A <see cref="TextLogWriter"/> that writes output to a debugger window.
	/// </summary>
	internal class DebuggerLogWriter : TextLogWriter
	{

		private readonly string _newLine;

		private const int c_debuggerAttachedCheckInterval = 1000; // Every second
		private bool _debuggerIsAttached;
		private int _lastDebuggerAttachedCheck; // The time, in ticks, of the last debugger attached check

		public DebuggerLogWriter(ITracerFactory setupTracerFactory, bool synchronize = false)
			: base(setupTracerFactory, synchronize)
		{
			_newLine = Console.Out.NewLine;
			_lastDebuggerAttachedCheck = int.MaxValue;
		}

#if (! PORTABLE)
		public override bool IsEnabled
		{
			get
			{
				// Periodically checks whether the debugger is attached.
				int nowTicks = Environment.TickCount;
				if ((nowTicks < _lastDebuggerAttachedCheck)
					|| (nowTicks >= _lastDebuggerAttachedCheck + c_debuggerAttachedCheckInterval))
				{
					_debuggerIsAttached = Debugger.IsLogging() || IsDebuggerPresent();
					_lastDebuggerAttachedCheck = nowTicks;
				}
				return _debuggerIsAttached && IsStarted;
			}
		}
#endif

		protected override void WriteFormattedEntry(string formattedEntry)
		{
			bool lockTaken = false;
			if (! formattedEntry.EndsWith(_newLine))
			{
				formattedEntry += _newLine;
			}

			if (_isSynchronized)
			{
				Monitor.Enter(this, ref lockTaken);
			}
			try
			{
#if (PORTABLE)
				// REVIEW: This isn't reliable - it is conditionally compiled in debug builds; but it's all that's available in the portable profile.
				Debug.Write(formattedEntry);
#else
				if (Debugger.IsLogging())
				{
					Debugger.Log(0, null, formattedEntry);
				}
				else if (IsDebuggerPresent())
				{
					try
					{
						OutputDebugString(formattedEntry);
					}
					catch (EntryPointNotFoundException)
					{
						// Disable logging with OutputDebugString for a bit.
						_debuggerIsAttached = false;
					}
				}
#endif
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(this);
				}
			}
		}

#if !PORTABLE
		[DllImport("kernel32.dll")]
		internal static extern bool IsDebuggerPresent();

		[DllImport("kernel32.dll")]
		internal static extern void OutputDebugString(string message);
#endif

	}

}
