// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBufferingLogWriter.cs">
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
    /// Adds support for "smart" flushing logic to log writers that buffer output, and need to know
    /// when it makes sense to flush their buffers.
    /// </summary>
    [ContractClass(typeof(BufferingLogWriterContract))]
    public interface IBufferingLogWriter : ILogWriter
    {

        /// <summary>
        /// Gets and sets a function that the logwriter can use to determine whether it should flush
        /// its buffer or not.
        /// </summary>
        Func<bool> FlushPredicate { get; set; }

    }


    [ContractClassFor(typeof(IBufferingLogWriter))]
    internal abstract class BufferingLogWriterContract : IBufferingLogWriter
    {

        public bool IsSynchronized { get { throw new NotImplementedException(); } }

        public Func<bool> FlushPredicate
        {
            get
            {
                Contract.Ensures(Contract.Result<Func<bool>>() != null);
                throw new NotImplementedException();
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                throw new NotImplementedException();
            }
        }

        #region Not in contract

        public bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter) where TEntry : ILogEntry
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<Type, object>> EntryWriters { get { throw new NotImplementedException(); } }

        #endregion
    }

}
