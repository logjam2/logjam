// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace LogJam.UnitTests.Common
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	using LogJam.Util;
	using LogJam.Writer;


	/// <summary>
	/// A test <see cref="ILogWriter{TEntry}"/>, similar to <see cref="ListLogWriter{TEntry}"/>, but with additional 
	/// features for unit testing.
	/// </summary>
	public class TestLogWriter<TEntry> :  ILogWriter<TEntry>, IEnumerable<TEntry>, IEnumerable, IStartable, IDisposable
		where TEntry : ILogEntry
	{

		private readonly IList<TEntry> _entryList;
		private readonly bool _isSynchronized = false;
		private bool _isStarted = false;
		private bool _isDisposed = false;

		public TestLogWriter(bool synchronize)
		{
			_entryList = new List<TEntry>();
			_isSynchronized = synchronize;
		}

		internal bool IsDisposed
		{ get { return _isDisposed; } }

		public virtual void Dispose()
		{
			lock (this)
			{
				Stop();

				_isDisposed = true;
			}
		}

		protected void EnsureNotDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().GetCSharpName());
			}
		}

		#region ILogWriter

		/// <summary>
		/// Returns <c>true</c> until the logwriter is disposed.
		/// </summary>
		public bool Enabled { get { return _isStarted; } }

		/// <summary>
		/// Returns <c>true</c> if calls to this object's methods and properties are synchronized.
		/// </summary>
		public bool IsSynchronized { get { return _isSynchronized; } }

		/// <summary>
		/// Adds the <paramref name="entry"/> to the <see cref="List{TEntry}"/>.
		/// </summary>
		/// <param name="entry">A <typeparamref name="TEntry"/>.</param>
		public virtual void Write(ref TEntry entry)
		{
			if (! _isSynchronized)
			{
				if (_isStarted)
				{
					_entryList.Add(entry);
				}
			}
			else
			{
				lock (this)
				{
					if (_isStarted)
					{
						_entryList.Add(entry);
					}
				}
			}
		}

		#endregion
		#region IStartable

		public virtual void Start()
		{
			lock (this)
			{
				EnsureNotDisposed();

				_isStarted = true;
			}
		}

		public virtual void Stop()
		{
			lock (this)
			{
				_isStarted = false;
			}
		}

		public bool IsStarted
		{
			get { return _isStarted; }
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
		/// Returns the number of entries logged to this <see cref="ListLogWriter{TEntry}"/>.
		/// </summary>
		public int Count
		{ get { return _entryList.Count; } }

		/// <summary>
		/// Removes all entries that have been previously logged.
		/// </summary>
		public void Clear()
		{
			_entryList.Clear();
		}

	}

}