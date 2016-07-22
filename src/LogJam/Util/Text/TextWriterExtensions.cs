// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util.Text
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;


    /// <summary>
    /// Extension methods for <see cref="TextWriter" />.
    /// </summary>
    internal static class TextWriterExtensions
    {
        #region Constants

        private const char LineFeed = '\n';

        #endregion

        public static void BufferedWrite(this TextWriter textWriter, string s, char[] buffer)
        {
            Contract.Requires<ArgumentNullException>(textWriter != null);
            Contract.Requires<ArgumentNullException>(s != null);

            int macIndex = s.Length;
            int bufLen = buffer.Length;
            for (int i = 0; i < macIndex; i += bufLen)
            {
                int lenCopy = Math.Min(macIndex - i, bufLen);
                s.CopyTo(i, buffer, 0, lenCopy);
                textWriter.Write(buffer, 0, lenCopy);
            }
        }

        public static void BufferedWrite(this TextWriter textWriter, string s, int startIndex, int length, char[] buffer)
        {
            Contract.Requires<ArgumentNullException>(textWriter != null);
            Contract.Requires<ArgumentNullException>(s != null);
            Contract.Requires<ArgumentOutOfRangeException>(s.Length >= startIndex + length);

            int macIndex = startIndex + length;
            int bufLen = buffer.Length;
            for (int i = startIndex; i < macIndex; i += bufLen)
            {
                int lenCopy = Math.Min(macIndex - i, bufLen);
                s.CopyTo(i, buffer, 0, lenCopy);

                textWriter.Write(buffer, 0, lenCopy);
            }
        }

        public static void BufferedWrite(this TextWriter textWriter, StringBuilder sb, int startIndex, int length, char[] buffer)
        {
            Contract.Requires<ArgumentNullException>(textWriter != null);
            Contract.Requires<ArgumentNullException>(sb != null);

            int macIndex = Math.Min(sb.Length, startIndex + length);
            int bufLen = buffer.Length;
            for (int i = startIndex; i < macIndex; i += bufLen)
            {
                int lenCopy = Math.Min(macIndex - i, bufLen);
                sb.CopyTo(i, buffer, 0, lenCopy);
                textWriter.Write(buffer, 0, lenCopy);
            }
        }

        public static void Repeat(this TextWriter textWriter, char ch, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                textWriter.Write(ch);
            }
        }

        public static void WriteIndentedLines(this TextWriter textWriter, string s, int indentSpaces, string linePrefix = null, bool endWithNewline = true)
        {
            Contract.Requires<ArgumentOutOfRangeException>(indentSpaces >= 0);

            throw new NotImplementedException();
            /*
            var buffer = new StringBuilder();
            if (((indentSpaces == 0) && (linePrefix == null)) || string.IsNullOrEmpty(s))
            {
                // Extra logic unnecessary
                buffer.Append(s);
                return;
            }

            if ((buffer.Length == 0) || buffer.EndsWith(LineFeed))
            {
                if (linePrefix != null)
                {
                    buffer.Append(linePrefix);
                }

                buffer.Append(' ', indentSpaces);
            }

            // Track line position within s to append to buffer - append one line at a time, then prefix each line with indentation
            int ichAppend = 0;
            int ichLineFeed = s.IndexOf(LineFeed);
            while (ichLineFeed >= 0)
            {
                // Append the line including the LineFeed
                buffer.Append(s, ichAppend, ichLineFeed - ichAppend + 1);
                ichAppend = ichLineFeed + 1;

                // If the LineFeed is the last char, don't indent the next line
                if (ichLineFeed < s.Length - 1)
                {
                    // Indent following the LineFeed
                    if (linePrefix != null)
                    {
                        buffer.Append(linePrefix);
                    }

                    buffer.Append(' ', indentSpaces);
                }

                // Look for next LineFeed
                ichLineFeed = s.IndexOf(LineFeed, ichAppend);
            }

            // Append whatever remains.
            buffer.Append(s, ichAppend, s.Length - ichAppend);
            */
        }

    }

}
