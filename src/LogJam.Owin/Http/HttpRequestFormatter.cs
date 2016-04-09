// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestFormatter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.Http
{
    using System.Text;

    using LogJam.Writer.Text;


    /// <summary>
    /// Formats <see cref="HttpRequestEntry" /> instances to a text stream.
    /// </summary>
    public sealed class HttpRequestFormatter : EntryFormatter<HttpRequestEntry>
    {

        /// <inheritdoc />
        public override void Format(ref HttpRequestEntry entry, FormatWriter formatWriter)
        {
            StringBuilder buf = formatWriter.FieldBuffer;

            formatWriter.BeginEntry(0);

            // RequestNumber
            buf.Clear();
            buf.Append(entry.RequestNumber);
            buf.Append('>');
            formatWriter.WriteField(buf, ColorCategory.Markup, 3);

            formatWriter.WriteTimestamp(entry.RequestStarted, ColorCategory.Detail);

            formatWriter.WriteField(entry.Method, ColorCategory.Info, 3);
            formatWriter.WriteField(entry.Uri, ColorCategory.Important);

            FormatterHelper.FormatHeaders(formatWriter, entry.RequestHeaders);

            formatWriter.WriteLine(); // Extra line break for readability
            formatWriter.EndEntry();

            formatWriter.IndentLevel++;
        }

    }

}
