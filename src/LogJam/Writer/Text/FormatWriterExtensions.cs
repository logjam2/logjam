// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatWriterExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;

    using LogJam.Util.Text;


    /// <summary>
    /// Extension methods for <see cref="FormatWriter" />.
    /// </summary>
    public static class FormatWriterExtensions
    {

        /// <summary>
        /// Formats an int field, with zero padding and space padding if specified.
        /// </summary>
        /// <param name="formatWriter">The <see cref="FormatWriter" /> being written to.</param>
        /// <param name="number">The number to format.</param>
        /// <param name="colorCategory">The color category for the output.</param>
        /// <param name="zeroPaddedWidth">
        /// The minimum digit width to write. If <paramref name="number" /> has fewer digits the
        /// output is left-padded with zeros.
        /// </param>
        /// <param name="spacePaddedWidth">
        /// The minimum character width to write. If the written characters are less than this width, the output is right-padded
        /// with spaces.
        /// </param>
        public static void WriteIntField(this FormatWriter formatWriter,
                                         int number,
                                         ColorCategory colorCategory = ColorCategory.Detail,
                                         int zeroPaddedWidth = 0,
                                         int spacePaddedWidth = 0)
        {
            Contract.Requires<ArgumentNullException>(formatWriter != null);

            StringBuilder fieldBuffer = formatWriter.FieldBuffer;
            fieldBuffer.Clear();
            if (zeroPaddedWidth > 0)
            {
                fieldBuffer.AppendPadZeroes(number, zeroPaddedWidth);
            }

            int spacesPadding = spacePaddedWidth - fieldBuffer.Length;
            if (spacesPadding > 0)
            {
                fieldBuffer.Append(' ', spacesPadding);
            }

            formatWriter.WriteField(fieldBuffer, colorCategory);
        }

        /// <summary>
        /// Formats a time offset field
        /// </summary>
        /// <param name="timeOffset"></param>
        /// <param name="colorCategory"></param>
        public static void WriteTimeOffset(this FormatWriter formatWriter, TimeSpan timeOffset, ColorCategory colorCategory, int leftPaddedWidth = 9)
        {
            var buf = formatWriter.FieldBuffer;
            buf.Clear();

            // Handle negative timespans
            if (timeOffset.Ticks < 0)
            {
                buf.Append('-');
                timeOffset = timeOffset.Negate();
            }

            int hours = (int) Math.Floor(timeOffset.TotalHours);
            if (hours > 0)
            {
                buf.Append(hours);
                buf.Append(':');
            }
            int minutes = timeOffset.Minutes;
            if (hours > 0)
            {
                buf.AppendPadZeroes(minutes, 2);
            }
            else
            {
                buf.Append(minutes);
            }
            buf.Append(':');
            buf.AppendPadZeroes(timeOffset.Seconds, 2);
            buf.Append('.');
            buf.AppendPadZeroes(timeOffset.Milliseconds, 3);

            int leftPadSpaces = leftPaddedWidth - buf.Length;
            formatWriter.WriteSpaces(leftPadSpaces);
            formatWriter.WriteField(buf, colorCategory);
        }

        public static void WriteAbbreviatedTypeName(this FormatWriter formatWriter, string typeName, ColorCategory colorCategory = ColorCategory.Detail, int padWidth = 0)
        {
            Contract.Requires<ArgumentNullException>(formatWriter != null);
            Contract.Requires<ArgumentNullException>(typeName != null);

            // Count the dots
            int countDots = 0;
            for (int i = typeName.IndexOf('.'); i >= 0; i = typeName.IndexOf('.', i + 1))
            {
                countDots++;
            }
            if (countDots == 0)
            {
                formatWriter.WriteField(typeName, colorCategory, padWidth);
                return;
            }

            // Walk the string, abbreviating until just over half the segments are abbreviated
            StringBuilder fieldBuffer = formatWriter.FieldBuffer;
            int segmentsToAbbreviate = (countDots >> 1) + 1;
            fieldBuffer.Clear();
            int len = typeName.Length;
            fieldBuffer.Append(typeName[0]); // Always include the first char
            for (int i = 1, segmentsAbbreviated = 0; i < len; ++i)
            {
                char ch = typeName[i];
                if (ch == '.')
                {
                    fieldBuffer.Append(ch);
                    i++;
                    if (++segmentsAbbreviated >= segmentsToAbbreviate)
                    { // No more abbreviating - take the rest straight
                        fieldBuffer.Append(typeName, i, len - i);
                        break;
                    }
                    else
                    { // Keep abbreviating - always include the first char after a '.'
                        if (i < len)
                        {
                            fieldBuffer.Append(typeName[i]);
                        }
                    }
                }
                else if (! ch.AsciiIsLower())
                {
                    fieldBuffer.Append(ch);
                }
                // Else ch is lower case - omit it 
            }

            // Add padding if needed
            int spacesPadding = padWidth - fieldBuffer.Length;
            if (spacesPadding > 0)
            {
                fieldBuffer.Append(' ', spacesPadding);
            }

            formatWriter.WriteField(fieldBuffer, colorCategory);
        }

    }

}
