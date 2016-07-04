// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrowingListEnumerator.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;


    /// <summary>
    /// Supports enumeration of a growing list, and doesn't throw exceptions when the list is modified mid-enumeration.
    /// </summary>
    /// <remarks>
    /// This enumerator expects to be read from a single thread, but the underlying list may be written from other threads.
    /// </remarks>
    internal sealed class GrowingListEnumerator<T> : IEnumerator<T>
    {

        private readonly IList<T> _list;
        private int _currentIndex;
        private int _lastKnownCount;

        public GrowingListEnumerator(IList<T> list)
        {
            Contract.Requires<ArgumentNullException>(list != null);
            _list = list;
            _currentIndex = -1;
            UpdateLastKnownCount();
        }

        public void Dispose()
        {}

        public bool MoveNext()
        {
            _currentIndex++;
            if (_currentIndex < _lastKnownCount)
            {
                return true;
            }
            UpdateLastKnownCount();
            return (_currentIndex < _lastKnownCount);
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        public T Current
        {
            get
            {
                if ((_currentIndex < 0) ||
                    (_currentIndex >= _lastKnownCount))
                {
                    return default(T);
                }

                try
                {
                    return _list[_currentIndex];
                }
                catch (ArgumentOutOfRangeException)
                {
                    // This could be caused by clearing or truncating the list.
                    UpdateLastKnownCount();
                    return default(T);
                }
            }
        }

        object IEnumerator.Current { get { return Current; } }

        private void UpdateLastKnownCount()
        {
            lock (_list)
            {
                _lastKnownCount = _list.Count;
            }
        }

    }

}
