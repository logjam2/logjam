// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListLogWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System.Collections;
    using System.Collections.Generic;

    using LogJam.Trace;
    using LogJam.Util;


    /// <summary>
    /// Appends all log entries to a <see cref="List{T}" />.
    /// </summary>
    public sealed class ListLogWriter<TEntry> : SingleEntryTypeLogWriter<TEntry>, IEnumerable<TEntry>, IStartable
        where TEntry : ILogEntry
    {

        private readonly List<TEntry> _entryList;
        private readonly bool _isSynchronized;

        /// <summary>
        /// Creates a new <see cref="ListLogWriter{TEntry}" />.
        /// </summary>
        /// <param name="setupTracerFactory"></param>
        /// <param name="synchronize">
        /// If set to <c>true</c> (the default), writes are synchronized, meaning entries are only added to
        /// the list one thread at a time using a <c>lock</c>.  If <c>false</c>, writes are not synchronized by this class, so
        /// another
        /// mechanism must be used to synchronize writes from multiple threads.
        /// </param>
        public ListLogWriter(ITracerFactory setupTracerFactory, bool synchronize = true)
            : base(setupTracerFactory)
        {
            _entryList = new List<TEntry>();
            _isSynchronized = synchronize;
        }

        #region ILogWriter

        /// <summary>
        /// Returns <c>true</c> if calls to this object's methods and properties are synchronized.
        /// </summary>
        public override bool IsSynchronized { get { return _isSynchronized; } }

        #endregion

        #region IEntryWriter

        /// <summary>
        /// Adds the <paramref name="entry" /> to the <see cref="List{TEntry}" />.
        /// </summary>
        /// <param name="entry">A <typeparamref name="TEntry" />.</param>
        public override void Write(ref TEntry entry)
        {
            if (IsStarted)
            {
                if (! _isSynchronized)
                {
                    _entryList.Add(entry);
                }
                else
                {
                    lock (this)
                    {
                        _entryList.Add(entry);
                    }
                }
            }
        }

        #endregion

        public IEnumerator<TEntry> GetEnumerator()
        {
            return new GrowingListEnumerator<TEntry>(_entryList);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the number of entries logged to this <see cref="ListLogWriter{TEntry}" />.
        /// </summary>
        public int Count { get { return _entryList.Count; } }

        /// <summary>
        /// Removes all entries that have been previously logged.
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                _entryList.Clear();
            }
        }

    }

}
