// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpResponseFormatter.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System.Text;

using LogJam.Util.Text;
using LogJam.Writer.Text;

namespace LogJam.Owin.Http
{

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
            formatWriter.WriteField(buf, ColorCategory.Markup, 3);

            formatWriter.WriteTimestamp(entry.RequestCompleted, ColorCategory.Detail);

            // Ttfb
            buf.Clear();
            buf.AppendPadZeroes(entry.Ttfb.Seconds, 2);
            buf.Append('.');
            buf.AppendPadZeroes(entry.Ttfb.Milliseconds, 3);
            buf.Append('s');
            formatWriter.WriteField(buf, ColorCategory.Info);

            // Determine response color from HTTP status code
            ColorCategory responseColorCategory = ColorCategory.None;
            if (formatWriter.IsColorEnabled)
            {
                var statusCode = entry.HttpStatusCode;
                if ((statusCode >= 200) && (statusCode < 300))
                {
                    responseColorCategory = ColorCategory.Success;
                }
                else if ((statusCode >= 300) && (statusCode < 400))
                {
                    responseColorCategory = ColorCategory.Important;
                }
                else if (statusCode >= 500)
                {
                    responseColorCategory = ColorCategory.Error;
                }
                else
                {
                    responseColorCategory = ColorCategory.Warning;
                }
            }

            formatWriter.WriteField(entry.Method, ColorCategory.Info, 3);
            formatWriter.WriteField(entry.Uri, responseColorCategory);
            formatWriter.WriteLine();

            // HTTP status line
            formatWriter.WriteLinePrefix(formatWriter.IndentLevel + 1);
            formatWriter.WriteText("  HTTP/1.1 ", ColorCategory.Detail);
            buf.Clear();
            buf.Append(entry.HttpStatusCode);
            formatWriter.WriteText(buf, 0, buf.Length, responseColorCategory);
            formatWriter.WriteSpaces(1);
            formatWriter.WriteText(entry.HttpReasonPhrase, ColorCategory.Detail);
            formatWriter.WriteLine();

            FormatterHelper.FormatHeaders(formatWriter, entry.ResponseHeaders);

            formatWriter.WriteLine(); // Extra line break for readability
            formatWriter.EndEntry();
        }

    }

}
