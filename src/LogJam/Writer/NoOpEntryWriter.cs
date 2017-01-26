// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpEntryWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System;


    /// <summary>
    /// An <see cref="IEntryWriter{TEntry}" /> that does nothing.
    /// </summary>
    public sealed class NoOpEntryWriter<TEntry> : IEntryWriter<TEntry>
        where TEntry : ILogEntry
    {

        public NoOpEntryWriter()
        {}

        public bool IsEnabled => false;

        public Type LogEntryType => typeof(TEntry);

        public void Write(ref TEntry entry)
        {}

    }

}
