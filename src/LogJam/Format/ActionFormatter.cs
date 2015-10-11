// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionFormatter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Format
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;


    /// <summary>
    /// Supports formatting log entries using a simple delegate.
    /// </summary>
    internal sealed class ActionFormatter<TEntry> : EntryFormatter<TEntry>
        where TEntry : ILogEntry
    {

        private readonly FormatAction<TEntry> _formatAction;

        public ActionFormatter(FormatAction<TEntry> formatAction)
        {
            Contract.Requires<ArgumentNullException>(formatAction != null);

            _formatAction = formatAction;
        }

        public override void Format(ref TEntry entry, TextWriter textWriter)
        {
            _formatAction(entry, textWriter);
        }

    }

}
