// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
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
	/// A log writer writes to one or more log targets.  A log writer supports one or more log entry types via contained
	/// entry writers.
    /// </summary>
    /// <seealso cref="IEntryWriter{TEntry}" />
    /// .
    [ContractClass(typeof(LogWriterContract))]
    public interface ILogWriter
    {

        /// <summary>
		/// Returns <c>true</c> if calls to this object's methods and its <see cref="IEntryWriter{TEntry}"/>s are synchronized
		/// by this log writer, or by a downstream log writer.
        /// </summary>
        /// <value>
        /// If <c>true</c>, calls on this object are threadsafe. If <c>false</c>, thread-safety is not guaranteed.
        /// </value>
		/// <remarks>
		/// In normal operation, the <see cref="ILogWriter"/>s returned to callers from <see cref="LogManager"/> should have
		/// <c>IsSynchronized = true</c>.  However there is often a chain of log writers, and the downstream logwriters may have
		/// <c>IsSynchronized = false</c>.  This is expected, as synchronization is typically provided by upstream proxy providers
		/// such as <see cref="BackgroundMultiLogWriter"/> or <see cref="SynchronizingProxyLogWriter"/>.
		/// </remarks>
        bool IsSynchronized { get; }

        /// <summary>
		/// Returns an <see cref="IEntryWriter{TEntry}"/> if one exists for log entry base type <typeparamref name="TEntry"/>.
        /// </summary>
        /// <typeparam name="TEntry">The log entry type.</typeparam>
        /// <returns></returns>
        bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter)
            where TEntry : ILogEntry;

        /// <summary>
		/// Returns the set of <see cref="IEntryWriter{TEntry}"/>s supported by this <c>ILogWriter</c>; the collection contains
		/// <see cref="KeyValuePair{TKey,TValue}"/>s with the log entry type and <see cref="IEntryWriter"/>.
        /// </summary>
        /// <returns></returns>
		IEnumerable<KeyValuePair<Type, IEntryWriter>> EntryWriters { get; }

    }


    [ContractClassFor(typeof(ILogWriter))]
    internal abstract class LogWriterContract : ILogWriter
    {

        public bool IsSynchronized { get { throw new System.NotImplementedException(); } }

        public bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter) where TEntry : ILogEntry
        {
            throw new System.NotImplementedException();
        }

		public IEnumerable<KeyValuePair<Type, IEntryWriter>> EntryWriters
        {
            get
            {
				Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<Type, IEntryWriter>>>() != null);

                throw new NotImplementedException();
            }
        }

    }

}
