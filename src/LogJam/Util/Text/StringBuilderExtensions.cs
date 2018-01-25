// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util.Text
{
    using System;
    using System.Text;

    using LogJam.Shared.Internal;


    /// <summary>
    /// Provides extension methods for <see cref="StringBuilder" /> objects.
    /// </summary>
    public static class StringBuilderExtensions
    {
        #region Constants

        private const char LineFeed = '\n';

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Appends <paramref name="number"/> to <paramref name="sb"/>, left-padding with zeroes to fill <paramref name="width" /> characters.
        /// </summary>
        /// <param name="sb">A <see cref="StringBuilder" />.</param>
        /// <param name="number">A number to write.</param>
        /// <param name="width">The minimum number of characters to fill.</param>
        public static void AppendPadZeroes(this StringBuilder sb, int number, int width)
        {
            Arg.InRange(width, 0, 10, nameof(width));

            if (number < 0)
            {
                sb.Append('-');
                number *= -1;
                width--;
            }

            int threshold = 1, i = 0;
            for (; i <= width && threshold <= number; ++i, threshold *= 10)
            {}
            int countZeroes = width - i;

            if (countZeroes > 0)
            {
                sb.Append('0', countZeroes);
                if (number == 0)
                { // To avoid writing an extra 0
                    return;
                }
            }

            sb.Append(number);
        }

        /// <summary>
        /// Appends indented lines to <paramref name="sb" />.
        /// </summary>
        /// <param name="sb">A <see cref="StringBuilder" />.</param>
        /// <param name="s">The string to append.</param>
        /// <param name="indentSpaces">Number of spaces to indent.</param>
        /// <param name="linePrefix">The prefix for each line (before indentation).</param>
        public static void AppendIndentLines(this StringBuilder sb, string s, int indentSpaces, string linePrefix = null)
        {
            Arg.InRange(indentSpaces, 0, int.MaxValue, nameof(indentSpaces));

            if (((indentSpaces == 0) && (linePrefix == null)) || string.IsNullOrEmpty(s))
            {
                // Extra logic unnecessary
                sb.Append(s);
                return;
            }

            if ((sb.Length == 0) || sb.EndsWith(LineFeed))
            {
                if (linePrefix != null)
                {
                    sb.Append(linePrefix);
                }

                sb.Append(' ', indentSpaces);
            }

            // Track line position within s to append to buffer - append one line at a time, then prefix each line with indentation
            int ichAppend = 0;
            int ichLineFeed = s.IndexOf(LineFeed);
            while (ichLineFeed >= 0)
            {
                // Append the line including the LineFeed
                sb.Append(s, ichAppend, ichLineFeed - ichAppend + 1);
                ichAppend = ichLineFeed + 1;

                // If the LineFeed is the last char, don't indent the next line
                if (ichLineFeed < s.Length - 1)
                {
                    // Indent following the LineFeed
                    if (linePrefix != null)
                    {
                        sb.Append(linePrefix);
                    }

                    sb.Append(' ', indentSpaces);
                }

                // Look for next LineFeed
                ichLineFeed = s.IndexOf(LineFeed, ichAppend);
            }

            // Append whatever remains.
            sb.Append(s, ichAppend, s.Length - ichAppend);
        }

        /// <summary>
        /// Compares <paramref name="s" /> to a substring in <paramref name="sb" />.
        /// </summary>
        /// <param name="sb">
        /// A <see cref="StringBuilder" />.
        /// </param>
        /// <param name="index">
        /// Start position within <paramref name="sb" />.
        /// </param>
        /// <param name="s">
        /// String to compare to a segment of <paramref name="sb" />.
        /// </param>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        public static int CompareSubString(this StringBuilder sb, int index, string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                int sbIndex = index + i;
                if (sbIndex >= sb.Length)
                {
                    // s > buffer, since it's longer
                    return 1;
                }
                else
                {
                    int cmp = s[i] - sb[sbIndex];
                    if (cmp != 0)
                    {
                        return cmp;
                    }
                }
            }

            // The substring is equal
            return 0;
        }

        /// <summary>
        /// The ends with.
        /// </summary>
        /// <param name="sb">
        /// The buffer.
        /// </param>
        /// <param name="suffix">
        /// The suffix.
        /// </param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool EndsWith(this StringBuilder sb, char suffix)
        {
            int len = sb.Length;
            if (len < 1)
            {
                return false;
            }

            return sb[len - 1] == suffix;
        }

        /// <summary>
        /// The ends with.
        /// </summary>
        /// <param name="sb">
        /// The buffer.
        /// </param>
        /// <param name="suffix">
        /// The suffix.
        /// </param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public static bool EndsWith(this StringBuilder sb, string suffix)
        {
            int i = sb.Length - suffix.Length;
            if (i < 0)
            {
                return false;
            }

            for (int j = 0; i < sb.Length; ++i, ++j)
            {
                if (sb[i] != suffix[j])
                {
                    return false;
                }
            }

            // buffer ends with suffix
            return true;
        }

        /// <summary>
        /// The ensure ends with.
        /// </summary>
        /// <param name="sb">
        /// The buffer.
        /// </param>
        /// <param name="suffix">
        /// The suffix.
        /// </param>
        public static void EnsureEndsWith(this StringBuilder sb, string suffix)
        {
            if (! sb.EndsWith(suffix))
            {
                // Append suffix, since it's not currently there.
                sb.Append(suffix);
            }
        }

        /// <summary>
        /// Returns the first index of any of <paramref name="matchChars" /> in <paramref name="sb" />, starting
        /// at index <paramref name="startIndex" /> and ending just before <paramref name="macIndex" />.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="matchChars"></param>
        /// <param name="startIndex"></param>
        /// <param name="macIndex"></param>
        /// <returns></returns>
        public static int IndexOfAny(this StringBuilder sb, char[] matchChars, int startIndex = 0, int macIndex = -1)
        {
            if (macIndex < 0)
            {
                macIndex = sb.Length;
            }

            for (int i = startIndex; i < macIndex; ++i)
            {
                char ch = sb[i];
                for (int j = 0; j < matchChars.Length; ++j)
                {
                    if (ch == matchChars[j])
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Remove all instances of characters in the buffer that match <paramref name="charsToRemove" />.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="charsToRemove"></param>
        public static void RemoveAll(this StringBuilder sb, char[] charsToRemove)
        {
            int upperEndOfRemoveStretch = -1;
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                char ch = sb[i];
                if (ch.MatchesAny(charsToRemove))
                {
                    if (upperEndOfRemoveStretch == -1)
                    {
                        upperEndOfRemoveStretch = i;
                    }
                }
                else if (upperEndOfRemoveStretch > i)
                {
                    sb.Remove(i + 1, upperEndOfRemoveStretch - i);
                    upperEndOfRemoveStretch = -1;
                }
            }

            if (upperEndOfRemoveStretch > 0)
            {
                sb.Remove(0, upperEndOfRemoveStretch + 1);
            }
        }

        /// <summary>
        /// Trims leading and trailing space characters - does not trim all whitespace characters, only " ".
        /// </summary>
        /// <param name="sb"></param>
        public static void TrimSpaces(this StringBuilder sb)
        {
            int len = sb.Length;
            for (int i = len - 1; i >= 0; i--)
            {
                char ch = sb[i];
                if (ch != ' ')
                {
                    if (i < len - 1)
                    {
                        sb.Length = i + 1;
                        len = sb.Length;
                    }
                    break;
                }
            }

            for (int i = 0; i < len; ++i)
            {
                char ch = sb[i];
                if (ch != ' ')
                {
                    if (i > 0)
                    {
                        sb.Remove(0, i);
                    }
                    break;
                }
            }
        }

        public static void BufferedAppend(this StringBuilder sb, StringBuilder source, int startIndex, int length, char[] buffer)
        {
            Arg.NotNull(sb, nameof(sb));
            Arg.NotNull(source, nameof(source));
            if (source.Length < startIndex + length)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"Cannot read past end of source - startIndex: {startIndex} + length: {length} > source.Length: {source.Length}");
            }

            int macIndex = startIndex + length;
            int bufLen = buffer.Length;
            for (int i = startIndex; i < macIndex; i += bufLen)
            {
                int lenCopy = Math.Min(macIndex - i, bufLen);
                source.CopyTo(i, buffer, 0, lenCopy);
                sb.Append(buffer, 0, lenCopy);
            }
        }

        #endregion
    }
}
