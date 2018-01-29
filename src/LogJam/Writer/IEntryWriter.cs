// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntryWriter.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
#if CODECONTRACTS
using System.Diagnostics.Contracts;
#endif

namespace LogJam.Writer
{


/// <summary>
/// Non-generic interface for entry writers.  Entry writers normally implement <see cref="IEntryWriter{TEntry}" /> directly, and implement
/// <c>IEntryWriter</c> because <see cref="IEntryWriter{TEntry}" /> requires it.  An <see cref="ILogWriter" /> contains one or more
/// <see cref="IEntryWriter{TEntry}" /> instances, each are uniquely identified by their <c>TEntry</c> type.
/// </summary>
/// <seealso cref="IEntryWriter{TEntry}" />
/// .
#if CODECONTRACTS
    [ContractClass(typeof(EntryWriterContract))]
#endif
public interface IEntryWriter
    {

        /// <summary>
        /// Returns <c>true</c> if this <see cref="IEntryWriter" /> can write entries to its target.
        /// </summary>
        /// <value>
        /// If <c>true</c>, this <c>IEntryWriter</c> can write entries.  If <c>false</c>, <see cref="IEntryWriter{TEntry}.Write" /> should not be called.
        /// </value>
        bool IsEnabled { get; }

        /// <summary>
        /// Returns the log entry type supported by this entry writer.  The log entry type must implement <seealso cref="ILogEntry" />.
        /// </summary>
        Type LogEntryType { get; }

    }


#if CODECONTRACTS
    [ContractClassFor(typeof(IEntryWriter))]
    internal abstract class EntryWriterContract : IEntryWriter
    {

        public bool IsEnabled { get { throw new NotImplementedException(); } }

        public Type LogEntryType
        {
            get
            {
                Contract.Ensures(Contract.Result<Type>() != null);

                throw new NotImplementedException();
            }
        }

    }
#endif

}
