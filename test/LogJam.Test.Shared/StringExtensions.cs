// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Test.Shared
{

    /// <summary>
    /// Extension methods on <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Returns the number of occurrence of <paramref name="ch"/> in the string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="ch">A <see cref="char"/>.</param>
        /// <returns></returns>
        public static int CountOf(this string s, char ch)
        {
            int count = 0;
            int index = 0;
            while (true)
            {
                index = s.IndexOf(ch, index);
                if (index == -1)
                {
                    break;
                }

                count++;
                index++;
                if (index >= s.Length)
                {
                    break;
                }
            }

            return count;
        }

    }

}
