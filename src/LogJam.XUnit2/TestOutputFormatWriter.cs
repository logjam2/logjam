// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputFormatWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
    using System;
    using System.Text;

    using LogJam.Shared.Internal;
    using LogJam.Trace;
    using LogJam.Util.Text;
    using LogJam.Writer.Text;

    using Xunit.Abstractions;


    /// <summary>
    /// Formats and writes log entries to an <see cref="ITestOutputHelper" />. This enables
    /// capturing trace and other log output and writing it to a test's xunit 2.0 test output.
    /// </summary>
    public sealed class TestOutputFormatWriter : FormatWriter, ITestOutputAccessor
    {

        private const int c_bufferSize = 512;

        private ITestOutputHelper _testOutput;
        private readonly StringBuilder _lineBuffer;
        private readonly char[] _charBuffer;

        /// <summary>
        /// Creates a new <see cref="TestOutputFormatWriter" />.
        /// </summary>
        /// <param name="testOutput">The xunit <see cref="ITestOutputHelper" /> to write formatted log output to.</param>
        /// <param name="setupTracerFactory">The <see cref="ITracerFactory" /> used to trace setup operations.</param>
        /// <param name="fieldDelimiter"></param>
        /// <param name="spacesPerIndentLevel"></param>
        public TestOutputFormatWriter(ITestOutputHelper testOutput,
                                      ITracerFactory setupTracerFactory,
                                      string fieldDelimiter = DefaultFieldDelimiter,
                                      int spacesPerIndentLevel = DefaultSpacesPerIndent)
            : base(setupTracerFactory, fieldDelimiter, spacesPerIndentLevel)
        {
            Arg.NotNull(setupTracerFactory, nameof(setupTracerFactory));

            _testOutput = testOutput ?? new NullTestOutput();
            atBeginningOfLine = true; // Must be true, since test output only writes whole lines.
            _lineBuffer = new StringBuilder(c_bufferSize);
            _charBuffer = new char[c_bufferSize];

            StartTimeUtc = DateTime.UtcNow;

            // Set defaults
            IncludeTime = false;
            IncludeTimeOffset = true;
        }

        #region ITestOutputAccessor

        public ITestOutputHelper TestOutput
        {
            get { return _testOutput; }
            set
            {
                // End the current line on the old test output if needed
                if (! atBeginningOfLine)
                {
                    WriteEndLine();
                }

                if (value == null)
                {
                    value = new NullTestOutput();
                }
                _testOutput = value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// <c>true</c> to include the time offset (since <see cref="StartTimeUtc" />)  when formatting timestamps.
        /// </summary>
        public bool IncludeTimeOffset { get; set; }

        /// <summary>
        /// The start time to use when computing time offsets.
        /// </summary>
        public DateTime StartTimeUtc { get; set; }

        public override bool IsEnabled { get { return true; } }

        /// <summary>
        /// Shouldn't be called b/c <see cref="WriteEndLine" /> is overridden.
        /// </summary>
        public override string LineDelimiter { get { return ""; } }

        public override bool IsColorEnabled { get { return false; } }

        #endregion

        #region Custom timestamp formatting

        /// <summary>
        /// Override timestamp formatting to support timestamp + offset
        /// </summary>
        /// <param name="timestampUtc"></param>
        /// <param name="colorCategory"></param>
        public override void WriteTimestamp(DateTime timestampUtc, ColorCategory colorCategory = ColorCategory.Detail)
        {
            if (IncludeTime)
            {
                base.WriteTimestamp(timestampUtc, colorCategory);
            }

            if (IncludeTimeOffset)
            {
                TimeSpan timeOffset = timestampUtc.Subtract(StartTimeUtc);
                this.WriteTimeOffset(timeOffset, colorCategory);
            }
        }

        /// <summary>
        /// Override timestamp formatting to support timestamp + offset
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="colorCategory"></param>
        public override void WriteTimestamp(DateTimeOffset timestamp, ColorCategory colorCategory = ColorCategory.Detail)
        {
            if (IncludeTime)
            {
                base.WriteTimestamp(timestamp, colorCategory);
            }

            if (IncludeTimeOffset)
            {
                TimeSpan timeOffset = timestamp.Subtract(StartTimeUtc);
                this.WriteTimeOffset(timeOffset, colorCategory);
            }
        }

        #endregion

        public override void WriteEndLine()
        {
            _testOutput.WriteLine(_lineBuffer.ToString());
            _lineBuffer.Clear();
            atBeginningOfLine = true;
        }

        protected override void WriteText(string s, ColorCategory colorCategory)
        {
            _lineBuffer.Append(s);
        }

        public override void WriteText(string s, int startIndex, int length, ColorCategory colorCategory)
        {
            _lineBuffer.Append(s, startIndex, length);
        }

        public override void WriteText(StringBuilder sb, int startIndex, int length, ColorCategory colorCategory)
        {
            _lineBuffer.BufferedAppend(sb, startIndex, length, _charBuffer);
        }

    }

}
