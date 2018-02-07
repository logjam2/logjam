// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterFormatWriter.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System.IO;
using System.Text;

using LogJam.Shared.Internal;
using LogJam.Trace;
using LogJam.Util.Text;

namespace LogJam.Writer.Text
{

    /// <summary>
    /// A <see cref="FormatWriter" /> that writes to a <see cref="TextWriter" />.
    /// </summary>
    public class TextWriterFormatWriter : FormatWriter
    {

        private TextWriter _textWriter;
        private bool _disposeWriter;
        private string _lineDelimiter;
        private readonly char[] _charBuffer;

        /// <summary>
        /// Creates a new <see cref="TextWriterFormatWriter" /> without a <see cref="TextWriter" />. <see cref="SetTextWriter" />
        /// must be called
        /// before this instance can be used.
        /// </summary>
        /// <param name="setupTracerFactory">The <see cref="ITracerFactory" /> to use for logging setup operations.</param>
        /// <param name="fieldDelimiter">The field delimiter for formatted text output.</param>
        /// <param name="spacesPerIndentLevel">The number of spaces per indent level. Can be 0 for no indenting.</param>
        public TextWriterFormatWriter(ITracerFactory setupTracerFactory,
                                      string fieldDelimiter = DefaultFieldDelimiter,
                                      int spacesPerIndentLevel = DefaultSpacesPerIndent)
            : base(setupTracerFactory, fieldDelimiter, spacesPerIndentLevel)
        {
            _charBuffer = new char[base.FieldBuffer.Capacity];
        }

        /// <summary>
        /// Creates a new <see cref="TextWriterFormatWriter" /> with an existing <see cref="TextWriter" />.
        /// </summary>
        /// <param name="setupTracerFactory">The <see cref="ITracerFactory" /> to use for logging setup operations.</param>
        /// <param name="textWriter">The <see cref="TextWriter" /> to write formatted log output to.</param>
        /// <param name="disposeWriter">
        /// Whether to dispose <paramref name="textWriter" /> when the <c>TextWriterFormatWriter</c> is
        /// disposed.
        /// </param>
        /// <param name="fieldDelimiter">The field delimiter for formatted text output.</param>
        /// <param name="spacesPerIndentLevel">The number of spaces per indent level. Can be 0 for no indenting.</param>
        public TextWriterFormatWriter(ITracerFactory setupTracerFactory,
                                      TextWriter textWriter,
                                      bool disposeWriter = false,
                                      string fieldDelimiter = DefaultFieldDelimiter,
                                      int spacesPerIndentLevel = DefaultSpacesPerIndent)
            : this(setupTracerFactory, fieldDelimiter, spacesPerIndentLevel)
        {
            Arg.NotNull(textWriter, nameof(textWriter));

            SetTextWriter(textWriter, disposeWriter);
        }

        /// <summary>
        /// Sets the <see cref="TextWriter" /> that is written to.
        /// </summary>
        /// <param name="textWriter">An open <see cref="TextWriter" /> to write formatted log output to.</param>
        /// <param name="disposeWriter">
        /// Whether to dispose <paramref name="textWriter" /> when the <c>TextWriterFormatWriter</c> is
        /// disposed.
        /// </param>
        public void SetTextWriter(TextWriter textWriter, bool disposeWriter)
        {
            Arg.NotNull(textWriter, nameof(textWriter));

            _textWriter = textWriter;
            State = StartableState.Started;
            _disposeWriter = disposeWriter;
            _lineDelimiter = _textWriter.NewLine;
            atBeginningOfLine = true;
        }

        protected int BufferLength { get { return _charBuffer.Length; } }

        public override bool IsEnabled { get { return _textWriter != null; } }

        public override string LineDelimiter { get { return _lineDelimiter; } }
        public override bool IsColorEnabled { get { return false; } }

        public override void Flush()
        {
            if (_textWriter != null)
            {
                _textWriter.Flush();
            }
        }

        protected override void InternalStart()
        {
            if (_textWriter == null)
            {
                setupTracer.Error("TextWriter must be set before Start()ing.");
            }
        }

        protected override void InternalStop()
        {
            if (_textWriter != null)
            {
                _textWriter.Flush();
                if (_disposeWriter)
                {
                    _textWriter.Dispose();
                }

                _textWriter = null;
            }
        }

        public override void Dispose()
        {
            if (_textWriter != null && ! IsDisposed)
            {
                State = StartableState.Disposing;
                _textWriter.Flush();
                if (_disposeWriter)
                {
                    _textWriter.Dispose();
                }

                _textWriter = null;
                State = StartableState.Disposed;
            }
        }

        protected override void WriteText(string s, ColorCategory colorCategory)
        {
            if (_textWriter != null)
            {
                _textWriter.BufferedWrite(s, _charBuffer);
            }
        }

        public override void WriteText(string s, int startIndex, int length, ColorCategory colorCategory)
        {
            if (_textWriter != null)
            {
                _textWriter.BufferedWrite(s, startIndex, length, _charBuffer);
            }
        }

        public override void WriteText(StringBuilder sb, int startIndex, int length, ColorCategory colorCategory)
        {
            if (_textWriter != null)
            {
                _textWriter.BufferedWrite(sb, startIndex, length, _charBuffer);
            }
        }

    }

}
