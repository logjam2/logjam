// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerFormatWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    using LogJam.Format;
    using LogJam.Trace;


    /// <summary>
    /// A <see cref="FormatWriter" /> that writes output to a debugger window.
    /// </summary>
    internal class DebuggerFormatWriter : FormatWriter
    {

        private readonly string _newLine;

        private const int c_debuggerAttachedCheckInterval = 1000; // Every second
        private bool _debuggerIsAttached;
        private int _lastDebuggerAttachedCheck; // The time, in ticks, of the last debugger attached check

        public DebuggerFormatWriter(ITracerFactory setupTracerFactory, string fieldDelimiter = DefaultFieldDelimiter, int spacesPerIndentLevel = 4)
            : base(setupTracerFactory, fieldDelimiter, spacesPerIndentLevel)
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
                return _debuggerIsAttached;
            }
        }
#endif

        public override string LineDelimiter { get { return _newLine; } }

        public override bool IsColorEnabled { get { return false; } }

        protected override void WriteText(string s, int startIndex, int length, ColorCategory colorCategory)
        {
            string text;
            if ((startIndex == 0) && (length == s.Length))
            {
                text = s;
            }
            else
            {
                text = s.Substring(startIndex, length);
            }

#if (PORTABLE)
    // REVIEW: This isn't reliable - it is conditionally compiled in debug builds; but it's all that's available in the portable profile.
			Debug.Write(text);
#else
            if (Debugger.IsLogging())
            {
                Debugger.Log(0, null, text);
            }
            else if (IsDebuggerPresent())
            {
                try
                {
                    OutputDebugString(text);
                }
                catch (EntryPointNotFoundException)
                {
                    // Disable logging with OutputDebugString for a bit.
                    _debuggerIsAttached = false;
                }
            }
#endif
        }

        protected override void WriteText(StringBuilder sb, ColorCategory colorCategory)
        {
            string s = sb.ToString();
            WriteText(s, 0, s.Length, colorCategory);
        }

#if !PORTABLE
        [DllImport("kernel32.dll")]
        internal static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll")]
        internal static extern void OutputDebugString(string message);
#endif

    }

}
