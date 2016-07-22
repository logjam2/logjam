// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleFormatWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;
    using System.IO;
    using System.Text;

    using LogJam.Trace;
    using LogJam.Util;


    /// <summary>
    /// Writes formatted output to the console.
    /// </summary>
    public sealed class ConsoleFormatWriter : TextWriterFormatWriter, IStartable
    {

        /// <summary>
        /// If more than 5 failures occur while writing to Console.Out, stop writing.
        /// </summary>
        private const short c_maxWriteFailures = 5;

        private short _countWriteFailures;

        // The text ConsoleColor before this ConsoleFormatWriter started operating
        private ConsoleColor _prevTextColor;
        private ConsoleColor _currentTextColor;

        public ConsoleFormatWriter(ITracerFactory setupTracerFactory, bool useColor = true, string fieldDelimiter = DefaultFieldDelimiter, int spacesPerIndentLevel = 4)
            : this(setupTracerFactory, useColor ? new DefaultConsoleColorResolver() : null, fieldDelimiter, spacesPerIndentLevel)
        {}

        public ConsoleFormatWriter(ITracerFactory setupTracerFactory,
                                   IConsoleColorResolver colorResolver,
                                   string fieldDelimiter = DefaultFieldDelimiter,
                                   int spacesPerIndentLevel = 4)
            : base(setupTracerFactory, fieldDelimiter, spacesPerIndentLevel)
        {
            _countWriteFailures = 0;
            ColorResolver = colorResolver;
        }

        protected override void InternalStart()
        {
            // Clear the count of write failures if any
            _countWriteFailures = 0;

            Stream consoleOutStream = Console.OpenStandardOutput(BufferLength);
            if (consoleOutStream == Stream.Null)
            {
                throw new LogJamStartException("Unable to open Console output stream", this);
            }
            else
            {
                var consoleOut = new StreamWriter(consoleOutStream, Console.OutputEncoding, BufferLength);
                SetTextWriter(consoleOut, false);

                (ColorResolver as IStartable).SafeStart(setupTracer);

                _prevTextColor = Console.ForegroundColor;
                _currentTextColor = _prevTextColor;
            }
        }

        protected override void InternalStop()
        {
            CloseTextWriter();

            Console.ForegroundColor = _prevTextColor;

            (ColorResolver as IStartable).SafeStop(setupTracer);
        }

        public IConsoleColorResolver ColorResolver { get; set; }

        public override bool IsColorEnabled { get { return ColorResolver != null; } }

        /// <summary>
        /// Ends an entry.
        /// </summary>
        public override void EndEntry()
        {
            base.EndEntry();

            // Restore initial console color, just in case someone uses Console.Write() concurrently
            SetConsoleColor(_prevTextColor);
        }

        private void SetConsoleColor(ConsoleColor textColor)
        {
            if (textColor != _currentTextColor)
            {
                Flush(); // Required for previously written colored text to be correctly colored.
                Console.ForegroundColor = textColor;
                _currentTextColor = textColor;
            }
        }

        private void OnConsoleWriteException(Exception exception)
        {
            setupTracer.Error(exception, "Exception caught writing to console");
            if (++_countWriteFailures > c_maxWriteFailures)
            {
                setupTracer.Error("Exceeded {0} write failures, halting console logging.");
                Stop();
            }
        }

        protected override void WriteText(string s, ColorCategory colorCategory)
        {
            var colorResolver = ColorResolver;
            if (colorResolver != null && colorCategory != ColorCategory.None)
            {
                SetConsoleColor(colorResolver.ResolveColor(colorCategory));
            }

            try
            {
                base.WriteText(s, colorCategory);
            }
            catch (Exception excp)
            {
                OnConsoleWriteException(excp);
            }
        }

        public override void WriteText(string s, int startIndex, int length, ColorCategory colorCategory)
        {
            var colorResolver = ColorResolver;
            if (colorResolver != null && colorCategory != ColorCategory.None)
            {
                SetConsoleColor(colorResolver.ResolveColor(colorCategory));
            }

            try
            {
                base.WriteText(s, startIndex, length, colorCategory);
            }
            catch (Exception excp)
            {
                OnConsoleWriteException(excp);
            }
        }

        public override void WriteText(StringBuilder sb, int startIndex, int length, ColorCategory colorCategory)
        {
            var colorResolver = ColorResolver;
            if (colorResolver != null && colorCategory != ColorCategory.None)
            {
                SetConsoleColor(colorResolver.ResolveColor(colorCategory));
            }

            try
            {
                base.WriteText(sb, startIndex, length, colorCategory);
            }
            catch (Exception excp)
            {
                OnConsoleWriteException(excp);
            }
        }

    }

}
