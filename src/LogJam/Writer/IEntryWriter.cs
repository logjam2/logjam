// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntryWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System.Diagnostics.Contracts;


    /// <summary>
    /// Supports writing strongly-typed log entries to a log target.
    /// </summary>
    /// <typeparam name="TEntry">The base entry type supported by the entry writer.</typeparam>
    [ContractClass(typeof(EntryWriterContract<>))]
    public interface IEntryWriter<TEntry>
        where TEntry : ILogEntry
    {

        /// <summary>
        /// Returns <c>true</c> if this <see cref="ILogWriter" /> can write entries to its target.
        /// </summary>
        /// <value>
        /// If <c>true</c>, this <c>IEntryWriter</c> can write entries.  If <c>false</c>, <see cref="IEntryWriter{TEntry}.Write" />
        /// should not be called.
        /// </value>
        bool IsEnabled { get; }

        /// <summary>
        /// Writes <paramref name="entry" /> to the log target.
        /// </summary>
        /// <param name="entry">The log entry to write.</param>
        void Write(ref TEntry entry);

    }


    [ContractClassFor(typeof(IEntryWriter<>))]
    internal abstract class EntryWriterContract<TEntry> : IEntryWriter<TEntry>
        where TEntry : ILogEntry
    {

        public ILogWriter LogWriter
        {
            get
            {
                Contract.Ensures(Contract.Result<ILogWriter>() != null);

                throw new System.NotImplementedException();
            }
        }

        public bool IsEnabled { get { throw new System.NotImplementedException(); } }

        public void Write(ref TEntry entry)
        {
            throw new System.NotImplementedException();
        }

    }
}
