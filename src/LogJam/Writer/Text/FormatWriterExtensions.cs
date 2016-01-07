// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatWriterExtensions.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
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
    /// Extension methods for <see cref="FormatWriter"/>.
    /// </summary>
    public static class FormatWriterExtensions
    {

        #region Format fields

        public static void WriteIntField(this FormatWriter formatWriter, int number, ColorCategory colorCategory = ColorCategory.Detail, int zeroPaddedWidth = 0, int spacePaddedWidth = 0)
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

        #endregion

    }

}