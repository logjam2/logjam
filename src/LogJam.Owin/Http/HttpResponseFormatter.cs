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
    using System.Text;

    using LogJam.Util.Text;
    using LogJam.Writer.Text;


    /// <summary>
    /// Formats <see cref="HttpResponseEntry" /> instances to a text stream.
    /// </summary>
    public sealed class HttpResponseFormatter : EntryFormatter<HttpResponseEntry>
    {

        /// <inheritdoc />
        public override void Format(ref HttpResponseEntry entry, FormatWriter formatWriter)
        {
            StringBuilder buf = formatWriter.FieldBuffer;

            formatWriter.IndentLevel--;
            formatWriter.BeginEntry(0);

            // RequestNumber
            buf.Clear();
            buf.Append(entry.RequestNumber);
            buf.Append('<');
            formatWriter.WriteField(buf, ColorCategory.Markup, 5);

            formatWriter.WriteField("Response:", ColorCategory.Detail);

            // Ttfb
            buf.Clear();
            buf.AppendPadZeroes(entry.Ttfb.Seconds, 2);
            buf.Append('.');
            buf.AppendPadZeroes(entry.Ttfb.Milliseconds, 3);
            buf.Append('s');
            formatWriter.WriteField(buf, ColorCategory.Info);

            // Determine request color
            ColorCategory requestColorCategory = ColorCategory.None;
            if (formatWriter.IsColorEnabled)
            {
                var statusCode = entry.HttpStatusCode;
                if ((statusCode >= 200) && (statusCode < 200))
                {
                    requestColorCategory = ColorCategory.Success;
                }
                else if ((statusCode >= 300) && (statusCode < 400))
                {
                    requestColorCategory = ColorCategory.Important;
                }
                else if (statusCode >= 500)
                {
                    requestColorCategory = ColorCategory.Error;
                }
                else
                {
                    requestColorCategory = ColorCategory.Warning;
                }
            }

            formatWriter.WriteField(entry.Method, requestColorCategory, 6);
            formatWriter.WriteField(entry.Uri, requestColorCategory);

            // HTTP status line
            buf.Clear();
            buf.Append("HTTP/1.1 ");
            buf.Append(entry.HttpStatusCode);
            buf.Append(" ");
            buf.Append(entry.HttpReasonPhrase);
            formatWriter.WriteLine(buf, requestColorCategory);

            FormatterHelper.FormatHeaders(formatWriter, entry.ResponseHeaders);

            formatWriter.EndEntry();
            formatWriter.WriteLine(); // Extra line break for readability
        }

    }

}
