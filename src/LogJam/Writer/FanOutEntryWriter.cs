// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FanOutEntryWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LogJam.Shared.Internal;


    /// <summary>
    /// Base class for LogWriters that write each entry to multiple downstream <see cref="IEntryWriter{TEntry}" /> instances.
    /// </summary>
    /// <seealso cref="ProxyEntryWriter{TEntry}" />
    /// for a similar logwriter that writes each entry to a single
    /// <see cref="IEntryWriter{TEntry}" />
    /// instance.
    public class FanOutEntryWriter<TEntry> : IEntryWriter<TEntry>, IDisposable
        where TEntry : ILogEntry
    {

        private readonly IEntryWriter<TEntry>[] _innerEntryWriters;
        private bool _disposed = false;

        /// <summary>
        /// Creates a new <see cref="FanOutEntryWriter{TEntry}" />.
        /// </summary>
        /// <param name="innerEntryWriters">The inner <see cref="IEntryWriter{TEntry}" />s to delegate to. May not be <c>null</c>.</param>
        public FanOutEntryWriter(params IEntryWriter<TEntry>[] innerEntryWriters)
        {
            Arg.NoneNull(innerEntryWriters, nameof(innerEntryWriters));

            _innerEntryWriters = innerEntryWriters;
        }

        /// <summary>
        /// Creates a new <see cref="FanOutEntryWriter{TEntry}" />.
        /// </summary>
        /// <param name="innerLogWriters">The inner <see cref="IEntryWriter{TEntry}" />s to delegate to. May not be <c>null</c>.</param>
        public FanOutEntryWriter(IEnumerable<IEntryWriter<TEntry>> innerLogWriters)
        {
            Arg.NoneNull(innerLogWriters, nameof(innerLogWriters));

            _innerEntryWriters = innerLogWriters.ToArray();
        }

        public virtual void Dispose()
        {
            if (! _disposed)
            {
                foreach (var writer in _innerEntryWriters)
                {
                    if (writer is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Returns the inner <see cref="IEntryWriter{TEntry}" />s that this <c>FanOutEntryWriter</c>
        /// forwards to.
        /// </summary>
        public IEnumerable<IEntryWriter<TEntry>> InnerEntryWriters { get { return _innerEntryWriters; } }

        /// <inheritdoc />
        public virtual void Write(ref TEntry entry)
        {
            foreach (var writer in _innerEntryWriters)
            {
                writer.Write(ref entry);
            }
        }

        /// <inheritdoc />
        public bool IsEnabled
        {
            get
            {
                // TODO: Perf test this - is it called for normal log operations? should we cache?
                return _innerEntryWriters.Any(writer => writer.IsEnabled);
            }
        }

    }

}
