// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="EqualityUtil.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{
	using System.Collections.Generic;


	/// <summary>
	/// Reusable methods for equality comparison and hash code computation.
	/// </summary>
	internal static class EqualityUtil
	{
		/// <summary>
		/// Hashes a variable using its <see cref="object.GetHashCode"/> implementation, but tolerates nulls.  Also left-shifts by <paramref name="leftShift"/>.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="leftShift"></param>
		/// <returns></returns>
		public static int Hash(this object o, int leftShift = 0)
		{
			return o == null ? 0 : (o.GetHashCode() << leftShift);
		}

		/// <summary>
		/// Combines 2 hash codes into 1.
		/// </summary>
		/// <param name="h1"></param>
		/// <param name="h2"></param>
		/// <returns></returns>
		internal static int CombineHashCodes(int h1, int h2)
		{
			return (((h1 << 5) + h1) ^ h2);
		}

	}

}