// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util.Text
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;


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
        /// The append indent lines.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="indentSpaces">
        /// The indent spaces.
        /// </param>
        /// <param name="linePrefix">
        /// The line prefix.
        /// </param>
        public static void AppendIndentLines(this StringBuilder sb, string s, int indentSpaces, string linePrefix = null)
        {
            Contract.Requires<ArgumentOutOfRangeException>(indentSpaces >= 0);

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

            // Track line position within s to append to sb - append one line at a time, then prefix each line with indentation
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
                    // s > sb, since it's longer
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
        /// The sb.
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
        /// The sb.
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

            // sb ends with suffix
            return true;
        }

        /// <summary>
        /// The ensure ends with.
        /// </summary>
        /// <param name="sb">
        /// The sb.
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

        #endregion
    }
}
