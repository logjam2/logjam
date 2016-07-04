// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEntry.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal.UnitTests.Examples
{
    using System;

    using LogJam.Writer.Text;


    /// <summary>
    /// A simple, logentry struct.
    /// </summary>
    [DefaultFormatter(typeof(MessageEntryFormatter))]
    public struct MessageEntry : ILogEntry
    {

        public readonly DateTime Timestamp;
        public readonly int? MessageId;
        public readonly string Text;

        public MessageEntry(string text)
        {
            Timestamp = DateTime.UtcNow;
            MessageId = null;
            Text = text;
        }

        public MessageEntry(int messageId, string text)
        {
            Timestamp = DateTime.UtcNow;
            MessageId = messageId;
            Text = text;
        }


        /// <summary>
        /// Default <see cref="EntryFormatter{TEntry}" /> for <see cref="MessageEntry" />.
        /// </summary>
        public class MessageEntryFormatter : EntryFormatter<MessageEntry>
        {

            public override void Format(ref MessageEntry entry, FormatWriter formatWriter)
            {
                formatWriter.BeginEntry(0);
                formatWriter.WriteTimestamp(entry.Timestamp);

                var buf = formatWriter.FieldBuffer;
                buf.Clear();
                buf.Append('[');
                if (entry.MessageId.HasValue)
                {
                    buf.Append(entry.MessageId.Value);
                }
                else
                {
                    buf.Append('?');
                }
                buf.Append(']');
                formatWriter.WriteField(buf);

                formatWriter.WriteField(entry.Text);
                formatWriter.EndEntry();
            }

        }

    }

}
