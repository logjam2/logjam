// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerFormatWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
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

    using LogJam.Trace;
    using LogJam.Util.Text;


    /// <summary>
    /// A <see cref="FormatWriter" /> that writes output to a debugger window.
    /// </summary>
    internal class DebuggerFormatWriter : FormatWriter
    {

        private readonly string _newLine;

        private const int c_debuggerAttachedCheckInterval = 2000; // Every 2 seconds
        private bool _debuggerIsAttached;
        private int _lastDebuggerAttachedCheck; // The time, in ticks, of the last debugger attached check
        private readonly StringBuilder _outputBuffer;
        private readonly char[] _charBuffer;

        public DebuggerFormatWriter(ITracerFactory setupTracerFactory, string fieldDelimiter = DefaultFieldDelimiter, int spacesPerIndentLevel = DefaultSpacesPerIndent)
            : base(setupTracerFactory, fieldDelimiter, spacesPerIndentLevel)
        {
            _newLine = Console.Out.NewLine;
            _lastDebuggerAttachedCheck = int.MaxValue;
            _outputBuffer = new StringBuilder(4096);
            _charBuffer = new char[_outputBuffer.Capacity];
        }

        public static bool IsDebuggerActive()
        {
#if PORTABLE
    // In portable builds, there is no way to tell if the debugger is active.
    // One assumption is that portable libs are not normally responsible for configuring LogJam
            return true;
#else
            return Debugger.IsLogging() || IsDebuggerPresent();
#endif
        }

        public void WriteDebuggerText(string s)
        {
#if (PORTABLE)
    // REVIEW: This isn't reliable - it is conditionally compiled in debug builds; but it's all that's available in the portable profile.
			Debug.Write(text);
#else
            if (Debugger.IsLogging())
            {
                Debugger.Log(0, null, s);
            }
            else if (IsDebuggerPresent())
            {
                try
                {
                    OutputDebugString(s);
                }
                catch (EntryPointNotFoundException)
                {
                    // Disable logging with OutputDebugString for a bit.
                    _debuggerIsAttached = false;
                    _lastDebuggerAttachedCheck = Environment.TickCount;
                }
            }
#endif
        }

#if (!PORTABLE)
        public override bool IsEnabled
        {
            get
            {
                // Periodically checks whether the debugger is attached.
                int nowTicks = Environment.TickCount;
                if ((nowTicks < _lastDebuggerAttachedCheck)
                    || (nowTicks >= _lastDebuggerAttachedCheck + c_debuggerAttachedCheckInterval))
                {
                    _debuggerIsAttached = IsDebuggerActive();
                    _lastDebuggerAttachedCheck = nowTicks;
                }
                return _debuggerIsAttached;
            }
        }
#endif

        public override string LineDelimiter { get { return _newLine; } }

        public override bool IsColorEnabled { get { return false; } }

        protected override void WriteText(string s, ColorCategory colorCategory)
        {
            _outputBuffer.Append(s);
        }

        public override void WriteText(string s, int startIndex, int length, ColorCategory colorCategory)
        {
            _outputBuffer.Append(s, startIndex, length);
        }

        public override void WriteText(StringBuilder sb, int startIndex, int length, ColorCategory colorCategory)
        {
            _outputBuffer.BufferedAppend(sb, startIndex, length, _charBuffer);
        }

        public override void Flush()
        {
            if (_outputBuffer.Length > 0)
            {
                WriteDebuggerText(_outputBuffer.ToString());
                _outputBuffer.Clear();
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
