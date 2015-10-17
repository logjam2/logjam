// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterFormatWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;

    using LogJam.Format;
    using LogJam.Trace;


    /// <summary>
    /// A <see cref="FormatWriter"/> that writes to a <see cref="TextWriter"/>.
    /// </summary>
    public class TextWriterFormatWriter : FormatWriter
    {
        private readonly TextWriter _textWriter;
        private readonly bool _disposeWriter;
        private bool _isDisposed;
        private readonly string _lineDelimiter;
        private readonly char[] _charBuffer;

        /// <summary>
        /// Creates a new <see cref="TextWriterFormatWriter" />.
        /// </summary>
        /// <param name="setupTracerFactory">The <see cref="ITracerFactory" /> to use for logging setup operations.</param>
        /// <param name="textWriter">The <see cref="TextWriter" /> to write formatted log output to.</param>
        /// <param name="disposeWriter">
        /// Whether to dispose <paramref name="textWriter" /> when the <c>TextWriterFormatWriter</c> is
        /// disposed.
        /// </param>
        /// <param name="fieldDelimiter">The field delimiter for formatted text output.</param>
        /// <param name="spacesPerIndentLevel">The number of spaces per indent level.  Can be 0 for no indenting.</param>
        public TextWriterFormatWriter(ITracerFactory setupTracerFactory,
                                      TextWriter textWriter,
                                      bool disposeWriter,
                                      string fieldDelimiter = DefaultFieldDelimiter,
                                      int spacesPerIndentLevel = 4)
            : base(setupTracerFactory, fieldDelimiter, spacesPerIndentLevel)
        {
            Contract.Requires<ArgumentNullException>(textWriter != null);

            _textWriter = textWriter;
            _disposeWriter = disposeWriter;
            _lineDelimiter = _textWriter.NewLine;
            _charBuffer = new char[base.buffer.Capacity];
        }

        public override bool IsEnabled { get { return ! _isDisposed; } }

        public override string LineDelimiter { get { return _lineDelimiter; } }
        public override bool IsColorEnabled { get { return false; } }

        public override void Flush()
        {
            _textWriter.Flush();
        }

        public override void Dispose()
        {
            if (! _isDisposed)
            {
                _textWriter.Flush();
                if (_disposeWriter)
                {
                    _textWriter.Dispose();
                }
                _isDisposed = true;
            }
        }

        protected override void WriteText(string s, int startIndex, int length, ColorCategory colorCategory)
        {
            int strLen = s.Length;
            int bufLen = _charBuffer.Length;
            for (int i = 0; i < strLen; i += bufLen)
            {
                int lenCopy = Math.Min(strLen - i, bufLen);
                s.CopyTo(i, _charBuffer, 0, lenCopy);

                _textWriter.Write(_charBuffer, 0, lenCopy);
            }
        }

        protected override void WriteText(StringBuilder sb, ColorCategory colorCategory)
        {
            int sbLen = sb.Length;
            int bufLen = _charBuffer.Length;
            for (int i = 0; i < sbLen; i += bufLen)
            {
                int lenCopy = Math.Min(sbLen - i, bufLen);
                sb.CopyTo(i, _charBuffer, 0, lenCopy);

                _textWriter.Write(_charBuffer, 0, lenCopy);
            }
        }

    }

}