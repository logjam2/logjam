// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpResponseFormatter.cs">
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
    /// Formats <see cref="HttpResponseEntry" /> instances to a text stream.
    /// </summary>
    public sealed class HttpResponseFormatter : EntryFormatter<HttpResponseEntry>
    {

        /// <inheritdoc />
        public override void Format(ref HttpResponseEntry entry, TextWriter textWriter)
        {
            textWriter.WriteLine("{0}<\tResponse:   \t{1}\t{2}\t   {3:ss\\.fff}s", entry.RequestNumber, entry.Method, entry.Uri, entry.Ttfb);
            textWriter.WriteLine("HTTP/1.1 {0}\t{1}", entry.HttpStatusCode, entry.HttpReasonPhrase);
            FormatterHelper.FormatHeaders(textWriter, entry.ResponseHeaders);
            textWriter.WriteLine(); // Extra line break for readability
        }

    }

}
