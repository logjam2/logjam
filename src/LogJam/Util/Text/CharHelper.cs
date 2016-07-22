// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharHelper.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util.Text
{

    /// <summary>
    /// Helper functions for dealing with characters.
    /// </summary>
    internal static class CharHelper
    {

        internal static bool AsciiIsLower(this char ch)
        {
            return (ch >= 97) && (ch <= 122);
        }

        internal static char AsciiToLower(this char ch)
        {
            if (65 <= ch && ch <= 90)
            {
                ch |= ' ';
            }
            return ch;
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="ch" /> matches any of <paramref name="chars" />.
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        internal static bool MatchesAny(this char ch, char[] chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (ch == chars[i])
                {
                    return true;
                }
            }

            return false;
        }

    }

}
