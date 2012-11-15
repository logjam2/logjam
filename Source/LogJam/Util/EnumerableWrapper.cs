// -----------------------------------------------------------------------
// <copyright file="EnumerableWrapper.cs" company="PrecisionDemand">
// Copyright (c) 2012 PrecisionDemand.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LogJam.Trace.Util
{
	/// <summary>
	/// Wraps an <see cref="IEnumerable{T}"/>.  Used to prevent access to the underlying collection via casting.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class EnumerableWrapper<T> : IEnumerable<T>
	{
		private readonly IEnumerable<T> _enumerable;

		internal EnumerableWrapper(IEnumerable<T> enumerable)
		{
			Contract.Requires<ArgumentNullException>(enumerable != null);

			_enumerable = enumerable;
		}

		#region Implementation of IEnumerable

		public IEnumerator<T> GetEnumerator()
		{
			return _enumerable.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _enumerable.GetEnumerator();
		}

		#endregion
	}
}