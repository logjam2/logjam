// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntryActionFormatter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;

    using LogJam.Shared.Internal;


    /// <summary>
    /// Supports formatting log entries using a <see cref="EntryFormatAction{TEntry}" /> delegate.
    /// </summary>
    internal sealed class EntryActionFormatter<TEntry> : EntryFormatter<TEntry>
        where TEntry : ILogEntry
    {

        private readonly EntryFormatAction<TEntry> _formatAction;

        public EntryActionFormatter(EntryFormatAction<TEntry> formatAction)
        {
            Arg.NotNull(formatAction, nameof(formatAction));

            _formatAction = formatAction;
        }

        public override void Format(ref TEntry entry, FormatWriter formatWriter)
        {
            _formatAction(entry, formatWriter);
        }

    }

}
