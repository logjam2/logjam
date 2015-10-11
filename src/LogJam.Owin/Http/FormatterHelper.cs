// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatterHelper.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.Http
{
    using System.Collections.Generic;
    using System.IO;


    /// <summary>
    /// Helper methods for log formatting.
    /// </summary>
    internal static class FormatterHelper
    {

        internal static void FormatHeaders(TextWriter textWriter, KeyValuePair<string, string[]>[] headers)
        {
            foreach (var header in headers)
            {
                foreach (string value in header.Value)
                {
                    textWriter.Write(header.Key);
                    textWriter.Write(": ");
                    textWriter.WriteLine(value);
                }
            }
        }

    }

}
