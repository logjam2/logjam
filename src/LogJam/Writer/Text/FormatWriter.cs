// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;
#if CODECONTRACTS
    using System.Diagnostics.Contracts;
#endif
    using System.Text;
    using System.Threading;

    using LogJam.Shared.Internal;
    using LogJam.Trace;
    using LogJam.Util;
    using LogJam.Util.Text;


    /// <summary>
    /// Abstract base class for writing formatted log output to a text target. A <c>FormatWriter</c> is primarily used by one
    /// or more <see cref="EntryFormatter{TEntry}" />s to write formatted text. <c>FormatWriter</c> is the primary abstraction for
    /// writing to a text target.
    /// <para>
    /// Text targets can be colorized and are generally optimized for readability. In contrast, binary targets are generally
    /// optimized for efficient and precise writing and parsing.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <c>FormatWriter</c> is <u>not</u> threadsafe. It assumes that writes are synchronized at a higher level (typically
    /// using a synchronizing logwriter like <see cref="SynchronizingProxyLogWriter"/> or <see cref="BackgroundMultiLogWriter"/>),
    /// so that the last entry is completely formatted/written before the next entry starts.
    /// <see cref="BeginEntry" /> and <see cref="EndEntry" /> provide basic checks for this assertion.
    /// </remarks>
    public abstract class FormatWriter : Startable, IDisposable
    {

        /// <summary>
        /// The default field delimiter is 2 spaces.
        /// </summary>
        public const string DefaultFieldDelimiter = "  ";

        /// <summary>
        /// The default number of spaces per indent level is 3.
        /// </summary>
        public const int DefaultSpacesPerIndent = 3;

        /// <summary>
        /// End-of-line chars.
        /// </summary>
        public readonly static char[] EolChars = { '\r', '\n' };

        /// <summary>
        /// Characters that are removed from field text.
        /// </summary>
        private static readonly char[] s_invalidFieldChars = { '\r', '\n', '\t' };

        #region Fields

        /// <summary>
        /// TimeZone to use for formatting dates and times.
        /// </summary>
        private TimeZoneInfo _outputTimeZone = TimeZoneInfo.Local;

        /// <summary>
        /// SetupLog <see cref="Tracer" />.
        /// </summary>
        protected readonly Tracer setupTracer;

        /// <summary>
        /// Delimiter between fields.
        /// </summary>
        private string _fieldDelimiter;

        /// <summary>
        /// A text buffer used by markup formatting methods.
        /// </summary>
        private readonly StringBuilder _markupBuffer;

        /// <summary>
        /// A text buffer that can be used by field formatting methods.
        /// </summary>
        private readonly StringBuilder _fieldBuffer;

        /// <summary>
        /// Set to <c>true</c> when text writing is positioned at the beginning of a line.
        /// </summary>
        protected bool atBeginningOfLine;

        /// <summary>
        /// The number of started and not completed entries. Should be 0 or 1 at all times (this is a class invariant).
        /// </summary>
        private int _startedEntries;

        /// <summary>
        /// The indent level before the current entry started.
        /// </summary>
        private int _previousIndentLevel;

        #endregion

        /// <summary>
        /// Initialize the <see cref="FormatWriter" />.
        /// </summary>
        /// <param name="setupTracerFactory">The <see cref="ITracerFactory" /> to use for logging setup operations.</param>
        /// <param name="fieldDelimiter">The field delimiter for formatted text output.</param>
        /// <param name="spacesPerIndentLevel">The number of spaces per indent level. Can be 0 for no indenting.</param>
        protected FormatWriter(ITracerFactory setupTracerFactory, string fieldDelimiter = DefaultFieldDelimiter, int spacesPerIndentLevel = DefaultSpacesPerIndent)
        {
            Arg.NotNull(setupTracerFactory, nameof(setupTracerFactory));
            Arg.NotNull(fieldDelimiter, nameof(fieldDelimiter));

            setupTracer = setupTracerFactory.TracerFor(this);
            _fieldDelimiter = fieldDelimiter;
            SpacesPerIndent = spacesPerIndentLevel;
            _markupBuffer = new StringBuilder(256);
            _fieldBuffer = new StringBuilder(512);
            _startedEntries = 0;

            // Set defaults
            IncludeTime = true;
            IncludeDate = false;
        }

        /// <summary>
        /// Must return <c>true</c> when the <c>FormatWriter</c> to available to write formatted output.
        /// </summary>
        public abstract bool IsEnabled { get; }

        public virtual void Dispose()
        {}

        #region Properties that control formatting

        /// <summary>
        /// The delimiter used to separate fields - eg single space, 2 spaces (default value), tab, comma.
        /// </summary>
        public string FieldDelimiter
        {
            internal set
            {
                Arg.NotNull(value, nameof(value));
                _fieldDelimiter = value;
            }
            get
            {
#if CODECONTRACTS
                Contract.Ensures(Contract.Result<string>() != null);
#endif
                return _fieldDelimiter;
            }
        }

        /// <summary>
        /// A buffer that can be used by custom formatting methods.
        /// </summary>
        /// <remarks>
        /// This buffer can be reused by every field formatting method, under the assumption that no two fields will
        /// be formatted at the same time. This invariant is maintained by synchronization at the <see cref="ILogWriter" /> level.
        /// <para>
        /// Continually reusing the same buffer is a performance optimization - it reduces memory allocation and GC activity
        /// significantly.
        /// </para>
        /// </remarks>
        public StringBuilder FieldBuffer
        {
            get
            {
                Contract.Ensures(Contract.Result<StringBuilder>() != null);
                return _fieldBuffer;
            }
        }

        /// <summary>
        /// Derived classes must return the line delimiter for the log target, eg "\r\n".
        /// </summary>
        public abstract string LineDelimiter { get; }

        /// <summary>
        /// When this <c>IsColorEnabled</c> returns <c>true</c>, <see cref="EntryFormatter{TEntry}" />s
        /// should determine and pass in <see cref="ColorCategory" /> values for all text. When
        /// <c>IsColorEnabled</c> returns <c>false</c>, any <see cref="ColorCategory" /> values
        /// are ignored.
        /// </summary>
        public abstract bool IsColorEnabled { get; }

        /// <summary>
        /// <c>true</c> to include the Date when formatting log entries with a date/time field.
        /// </summary>
        public bool IncludeDate { get; set; }

        /// <summary>
        /// <c>true</c> to include the Timestamp when formatting log entries with a date/time field.
        /// </summary>
        public bool IncludeTime { get; set; }

        /// <summary>
        /// The number of spaces for each indent level.
        /// </summary>
        public int SpacesPerIndent { get; internal set; }

        /// <summary>
        /// The current indent level. May be increased or decreased as needed by the formatter.
        /// </summary>
        public int IndentLevel { get; set; }

        /// <summary>
        /// Specifies the TimeZone to use when formatting the timestamp for a log entry. Defaults to local time.
        /// </summary>
        public TimeZoneInfo OutputTimeZone
        {
            get { return _outputTimeZone; }
            set
            {
                Arg.NotNull(value, nameof(value));

                _outputTimeZone = value;
            }
        }

#endregion

#region Public methods to write formatted log text

        /// <summary>
        /// Marks the start of a new entry.
        /// </summary>
        public virtual void BeginEntry()
        {
            if (Interlocked.Increment(ref _startedEntries) != 1)
            {
                setupTracer.Error("FormatWriter invariant violated: Only one entry must be written at a time.");
            }

            // Handle incorrect indentation
            if (IndentLevel < 0)
            {
                IndentLevel = 0;
            }
            _previousIndentLevel = IndentLevel;

            if (! atBeginningOfLine)
            {
                WriteEndLine();
            }
        }

        /// <summary>
        /// Marks the start of a new entry.
        /// </summary>
        public void BeginEntry(int indentLevel)
        {
            BeginEntry();
            IndentLevel = indentLevel;
        }

        public virtual void WriteField(string text, ColorCategory colorCategory = ColorCategory.None, int padWidth = 0)
        {
            if (atBeginningOfLine)
            {
                WriteLinePrefix(IndentLevel);
            }
            else
            {
                WriteText(FieldDelimiter, ColorCategory.Markup);
            }

            if (text != null)
            {
                WriteText(text, colorCategory, true);
                padWidth -= text.Length;
            }

            if (padWidth > 0)
            {
                WriteSpaces(padWidth);
            }
        }

        public virtual void WriteField(StringBuilder buffer, ColorCategory colorCategory = ColorCategory.None, int padWidth = 0)
        {
            Arg.NotNull(buffer, nameof(buffer));

            if (atBeginningOfLine)
            {
                WriteLinePrefix(IndentLevel);
            }
            else
            {
                WriteText(FieldDelimiter, ColorCategory.Markup);
            }

            WriteText(buffer, 0, buffer.Length, colorCategory);
            padWidth -= buffer.Length;

            if (padWidth > 0)
            {
                WriteSpaces(padWidth);
            }
        }

        public virtual void WriteField(StringBuilder buffer, int startIndex, int length, ColorCategory colorCategory = ColorCategory.None, int padWidth = 0)
        {
            Arg.NotNull(buffer, nameof(buffer));

            if (atBeginningOfLine)
            {
                WriteLinePrefix(IndentLevel);
            }
            else
            {
                WriteText(FieldDelimiter, ColorCategory.Markup);
            }

            WriteText(buffer, startIndex, length, colorCategory);
            padWidth -= buffer.Length;

            if (padWidth > 0)
            {
                WriteSpaces(padWidth);
            }
        }

        public virtual void WriteField(Action<StringBuilder> formatFieldAction, ColorCategory colorCategory = ColorCategory.None, int padWidth = 0)
        {
            Arg.NotNull(formatFieldAction, nameof(formatFieldAction));

            if (atBeginningOfLine)
            {
                WriteLinePrefix(IndentLevel);
            }
            else
            {
                WriteText(FieldDelimiter, ColorCategory.Markup);
            }

            _fieldBuffer.Clear();
            formatFieldAction(_fieldBuffer);
            WriteText(_fieldBuffer, 0, _fieldBuffer.Length, colorCategory);
            padWidth -= _fieldBuffer.Length;

            if (padWidth > 0)
            {
                WriteSpaces(padWidth);
            }
        }

        public virtual void WriteLines(string lines, ColorCategory colorCategory = ColorCategory.None, int relativeIndentLevel = 1)
        {
            if (! atBeginningOfLine)
            {
                WriteEndLine();
            }

            int indexStartLine = 0;
            while (indexStartLine < lines.Length)
            {
                int indexEol = lines.IndexOfAny(EolChars, indexStartLine);
                if (indexEol < 0)
                { // No more EOL chars in remainder of the string
                    indexEol = lines.Length;
                }

                // REVIEW: When there are multiple EolChars adjacent to each other (eg CrLfCrLf), nothing is written
                // If the caller wants blank lines, put a single space or tab on each blank line.
                if (indexEol > indexStartLine)
                {
                    WriteLinePrefix(IndentLevel + relativeIndentLevel);
                    WriteText(lines, indexStartLine, indexEol - indexStartLine, colorCategory);
                    WriteEndLine();
                }
                indexStartLine = indexEol + 1;
            }
        }

        public virtual void WriteLines(StringBuilder linesBuffer, ColorCategory colorCategory = ColorCategory.None, int relativeIndentLevel = 1)
        {
            if (! atBeginningOfLine)
            {
                WriteEndLine();
            }

            int indexStartLine = 0;
            int bufLen = linesBuffer.Length;
            while (indexStartLine < bufLen)
            {
                int indexEol = linesBuffer.IndexOfAny(EolChars, indexStartLine);
                if (indexEol < 0)
                { // No more EOL chars in remainder of linesBuffer
                    indexEol = bufLen;
                }

                // REVIEW: When there are multiple EolChars adjacent to each other (eg CrLfCrLf), nothing is written
                // If the caller wants blank lines, put a single space or tab on each blank line.
                if (indexEol > indexStartLine)
                {
                    WriteLinePrefix(IndentLevel + relativeIndentLevel);
                    WriteText(linesBuffer, indexStartLine, indexEol - indexStartLine, colorCategory);
                    WriteEndLine();
                }
                indexStartLine = indexEol + 1;
            }
        }

        public virtual void WriteLine(StringBuilder line, ColorCategory colorCategory = ColorCategory.None, int relativeIndentLevel = 1)
        {
            if (! atBeginningOfLine)
            {
                WriteEndLine();
            }

            WriteLinePrefix(IndentLevel + relativeIndentLevel);
            WriteText(line, 0, line.Length, colorCategory);
            WriteEndLine();
        }

        /// <summary>
        /// Ends an entry.
        /// </summary>
        public virtual void EndEntry()
        {
            if (! atBeginningOfLine)
            {
                WriteEndLine();
            }
            // Restore the indent level from before the entry started
            IndentLevel = _previousIndentLevel;

            Interlocked.Decrement(ref _startedEntries);
        }

        public virtual void Flush()
        {}

#endregion

#region Public methods to format field primitives

        public virtual void WriteDate(DateTime dateTimeUtc, ColorCategory colorCategory = ColorCategory.Detail)
        {
            DateTime outputDateTime = TimeZoneInfo.ConvertTime(dateTimeUtc, _outputTimeZone);

            // Format date
            _fieldBuffer.Clear();
            _fieldBuffer.AppendPadZeroes(outputDateTime.Year, 4);
            _fieldBuffer.Append('/');
            _fieldBuffer.AppendPadZeroes(outputDateTime.Month, 2);
            _fieldBuffer.Append('/');
            _fieldBuffer.AppendPadZeroes(outputDateTime.Day, 2);

            WriteField(_fieldBuffer, colorCategory);
        }

        public virtual void WriteTimestamp(DateTime timestampUtc, ColorCategory colorCategory = ColorCategory.Detail)
        {
            DateTime outputTimestamp = TimeZoneInfo.ConvertTime(timestampUtc, _outputTimeZone);

            // Format time
            _fieldBuffer.Clear();
            _fieldBuffer.AppendPadZeroes(outputTimestamp.Hour, 2);
            _fieldBuffer.Append(':');
            _fieldBuffer.AppendPadZeroes(outputTimestamp.Minute, 2);
            _fieldBuffer.Append(':');
            _fieldBuffer.AppendPadZeroes(outputTimestamp.Second, 2);
            _fieldBuffer.Append('.');
            _fieldBuffer.AppendPadZeroes(outputTimestamp.Millisecond, 3);

            WriteField(_fieldBuffer, colorCategory);
        }

        public virtual void WriteTimestamp(DateTimeOffset timestamp, ColorCategory colorCategory = ColorCategory.Detail)
        {
            DateTimeOffset outputTimestamp = TimeZoneInfo.ConvertTime(timestamp, _outputTimeZone);

            // Format time
            _fieldBuffer.Clear();
            _fieldBuffer.AppendPadZeroes(outputTimestamp.Hour, 2);
            _fieldBuffer.Append(':');
            _fieldBuffer.AppendPadZeroes(outputTimestamp.Minute, 2);
            _fieldBuffer.Append(':');
            _fieldBuffer.AppendPadZeroes(outputTimestamp.Second, 2);
            _fieldBuffer.Append('.');
            _fieldBuffer.AppendPadZeroes(outputTimestamp.Millisecond, 3);

            WriteField(_fieldBuffer, colorCategory);
        }

        public virtual void WriteLine()
        {
            WriteEndLine();
        }

        public virtual void WriteLinePrefix(int indentLevel)
        {
            WriteSpaces(indentLevel * SpacesPerIndent);
            atBeginningOfLine = false;
        }

        public virtual void WriteSpaces(int countSpaces)
        {
            if (countSpaces <= 0)
            {
                return;
            }

            _markupBuffer.Clear();
            _markupBuffer.Append(' ', countSpaces);
            WriteText(_markupBuffer, 0, countSpaces, ColorCategory.None);
        }

        /// <summary>
        /// Writes the specified text to the target.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="colorCategory"></param>
        /// <param name="normalizeFieldText">
        /// If <c>true</c>, <paramref name="s" /> should be normalized for display as a field, eg
        /// tabs and line breaks removed.
        /// </param>
        public virtual void WriteText(string s, ColorCategory colorCategory, bool normalizeFieldText = false)
        {
            if (s == null)
            { // Do nothing
            }
            else if (normalizeFieldText && (s.IndexOfAny(s_invalidFieldChars) >= 0))
            {
                _fieldBuffer.Clear();
                _fieldBuffer.Append(s);
                _fieldBuffer.RemoveAll(s_invalidFieldChars);
                _fieldBuffer.TrimSpaces();
                WriteText(_fieldBuffer, 0, _fieldBuffer.Length, colorCategory);
            }
            else
            {
                if (normalizeFieldText)
                {
                    s = s.Trim();
                }
                WriteText(s, colorCategory);
            }
        }

        public virtual void WriteEndLine()
        {
            WriteText(LineDelimiter, ColorCategory.None);
            atBeginningOfLine = true;
        }

#endregion

#region Abstract write methods

        protected abstract void WriteText(string s, ColorCategory colorCategory);

        public abstract void WriteText(string s, int startIndex, int length, ColorCategory colorCategory);

        public abstract void WriteText(StringBuilder sb, int startIndex, int length, ColorCategory colorCategory);

#endregion
    }

}
