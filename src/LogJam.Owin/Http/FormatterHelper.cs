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
    using System.Text;

    using LogJam.Writer.Text;


    /// <summary>
    /// Helper methods for log formatting.
    /// </summary>
    internal static class FormatterHelper
    {

        internal static void FormatHeaders(FormatWriter formatWriter, KeyValuePair<string, string[]>[] headers)
        {
            StringBuilder buf = formatWriter.FieldBuffer;
            foreach (var header in headers)
            {
                foreach (string value in header.Value)
                {
                    buf.Clear();
                    buf.Append(header.Key);
                    buf.Append(": ");
                    buf.Append(value);
                    formatWriter.WriteLine(buf, ColorCategory.Debug);
                }
            }
        }

    }

}
