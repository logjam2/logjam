// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;


    /// <summary>
    /// A log writer writes to one or more log targets.
    /// </summary>
    /// <seealso cref="IEntryWriter{TEntry}" />
    /// .
    [ContractClass(typeof(LogWriterContract))]
    public interface ILogWriter
    {

        /// <summary>
        /// Returns <c>true</c> if calls to this object's methods and its <see cref="IEntryWriter{TEntry}" />s are synchronized.
        /// </summary>
        /// <value>
        /// If <c>true</c>, calls on this object are threadsafe.  If <c>false</c>, thread-safety is not guaranteed.
        /// </value>
        bool IsSynchronized { get; }

        /// <summary>
        /// Returns an <see cref="IEntryWriter{TEntry}" /> if one exists for base entry type <typeparamref name="TEntry" />.
        /// </summary>
        /// <typeparam name="TEntry">The log entry type.</typeparam>
        /// <returns></returns>
        bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter)
            where TEntry : ILogEntry;

        /// <summary>
        /// Returns the set of <see cref="IEntryWriter{TEntry}" />s available for this <c>ILogWriter</c>.
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<Type, object>> EntryWriters { get; }

    }


    [ContractClassFor(typeof(ILogWriter))]
    internal abstract class LogWriterContract : ILogWriter
    {

        public bool IsSynchronized { get { throw new System.NotImplementedException(); } }

        public bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter) where TEntry : ILogEntry
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<KeyValuePair<Type, object>> EntryWriters
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<Type, object>>>() != null);

                throw new NotImplementedException();
            }
        }

    }

}
