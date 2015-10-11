// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestFormatter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.Http
{
    using System.IO;

    using LogJam.Format;


    /// <summary>
    /// Formats <see cref="HttpRequestEntry" /> instances to a text stream.
    /// </summary>
    public sealed class HttpRequestFormatter : EntryFormatter<HttpRequestEntry>
    {

        /// <inheritdoc />
        public override void Format(ref HttpRequestEntry entry, TextWriter textWriter)
        {
            textWriter.WriteLine("{0}>\t{1:HH:mm:ss.fff}\t{2}\t{3}", entry.RequestNumber, entry.RequestStarted, entry.Method, entry.Uri);
            FormatterHelper.FormatHeaders(textWriter, entry.RequestHeaders);
            textWriter.WriteLine(); // Extra line break for readability
        }

    }

}
