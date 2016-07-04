// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableSet.cs">
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
    using System.Linq;


    /// <summary>
    /// An <see cref="ISet{T}"/> that raises events when the set contents are changed.
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
            var addingItem = AddingItem;
            if (addingItem != null)
            {
                addingItem(item);
            }
        }

        private void OnRemovingItem(T item)
        {
            var removingItem = RemovingItem;
            if (removingItem != null)
            {
                removingItem(item);
            }
        }

        #region ISet<T>

        public IEnumerator<T> GetEnumerator() =>_innerSet.GetEnumerator();

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
            throw new NotImplementedException();
            // _innerSet.IntersectWith(other);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other.ToList())
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

        public bool Overlaps(IEnumerable<T> other) =>_innerSet.Overlaps(other);

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
            var innerList = _innerSet.ToList();
            _innerSet.Clear();
            foreach (var item in innerList)
            {
                OnRemovingItem(item);
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