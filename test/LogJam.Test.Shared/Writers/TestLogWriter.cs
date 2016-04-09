// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestLogWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Test.Shared.Writers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using LogJam.Trace;
    using LogJam.Writer;


    /// <summary>
    /// A test <see cref="ILogWriter" /> and <see cref="IEntryWriter{TEntry}" />, similar to
    /// <see cref="ListLogWriter{TEntry}" />, but with additional
    /// features for unit testing.
    /// </summary>
    public class TestLogWriter<TEntry> : SingleEntryTypeLogWriter<TEntry>, IEnumerable<TEntry>, IStartable, IDisposable
        where TEntry : ILogEntry
    {

        private readonly IList<TEntry> _entryList;
        private readonly bool _isSynchronized = false;

        public TestLogWriter(ITracerFactory setupTracerFactory, bool synchronize)
            : base(setupTracerFactory)
        {
            _entryList = new List<TEntry>();
            _isSynchronized = synchronize;
        }

        public new bool IsDisposed { get { return base.IsDisposed; } }

        #region IEntryWriter

        /// <summary>
        /// Returns <c>true</c> if calls to this object's methods and properties are synchronized.
        /// </summary>
        public override bool IsSynchronized { get { return _isSynchronized; } }

        /// <summary>
        /// Adds the <paramref name="entry" /> to the <see cref="List{TEntry}" />.
        /// </summary>
        /// <param name="entry">A <typeparamref name="TEntry" />.</param>
        public override void Write(ref TEntry entry)
        {
            if (! _isSynchronized)
            {
                if (IsStarted)
                {
                    _entryList.Add(entry);
                }
            }
            else
            {
                lock (this)
                {
                    if (IsStarted)
                    {
                        _entryList.Add(entry);
                    }
                }
            }
        }

        #endregion

        public IEnumerator<TEntry> GetEnumerator()
        {
            IEnumerable<TEntry> enumerable;
            if (_isSynchronized)
            {
                lock (this)
                {
                    enumerable = _entryList.ToArray();
                }
            }
            else
            {
                enumerable = _entryList.ToArray();
            }
            return enumerable.GetEnumerator();
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
            _entryList.Clear();
        }

    }

}
