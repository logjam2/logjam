// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableSet.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LogJam.Util.Collections
{

    /// <summary>
    /// An <see cref="ISet{T}" /> that raises events when the set contents are changed.
    /// </summary>
    internal class ObservableSet<T> : ISet<T>
    {

        private readonly ISet<T> _innerSet;

        public ObservableSet(IEqualityComparer<T> equalityComparer = null)
        {
            _innerSet = new HashSet<T>(equalityComparer);
        }

        public ObservableSet(ISet<T> innerSet)
        {
            _innerSet = innerSet;
        }

        /// <summary>
        /// Event that is raised when an item is added to the set.
        /// </summary>
        public event Action<T> AddingItem;

        /// <summary>
        /// Event that is raised when an item is removed from the set.
        /// </summary>
        public event Action<T> RemovingItem;

        private void OnAddingItem(T item)
        {
            AddingItem?.Invoke(item);
        }

        private void OnRemovingItem(T item)
        {
            RemovingItem?.Invoke(item);
        }

        #region ISet<T>

        public IEnumerator<T> GetEnumerator() => _innerSet.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _innerSet).GetEnumerator();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (! Contains(item))
                {
                    OnAddingItem(item);
                }
            }

            _innerSet.UnionWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            // exceptSet: The set of items in this, that are not in other
            var exceptSet = new HashSet<T>(_innerSet);
            foreach (var t in other)
            {
                exceptSet.Remove(t);
            }
            foreach (var item in exceptSet)
            {
                Remove(item);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other.ToArray())
            {
                if (Contains(item))
                {
                    Remove(item);
                }
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
            // _innerSet.SymmetricExceptWith(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other) => _innerSet.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => _innerSet.IsSupersetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => _innerSet.IsProperSupersetOf(other);

        public bool IsProperSubsetOf(IEnumerable<T> other) => _innerSet.IsProperSubsetOf(other);

        public bool Overlaps(IEnumerable<T> other) => _innerSet.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => _innerSet.SetEquals(other);

        public bool Add(T item)
        {
            if (_innerSet.Add(item))
            {
                OnAddingItem(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            if (_innerSet.Count > 0)
            {
                var previousElements = _innerSet.ToArray();
                _innerSet.Clear();
                foreach (var item in previousElements)
                {
                    OnRemovingItem(item);
                }
            }
        }

        public bool Contains(T item) => _innerSet.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _innerSet.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            if (_innerSet.Remove(item))
            {
                OnRemovingItem(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public int Count => _innerSet.Count;

        public bool IsReadOnly => _innerSet.IsReadOnly;

        #endregion

    }

}
