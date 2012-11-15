// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="NoopDisposable.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.Trace.Util
{
	using System;

	/// <summary>
	/// An <see cref="IDisposable"/> that does nothing on <see cref="Dispose"/>.
	/// </summary>
	public sealed class NoopDisposable : IDisposable
	{
		#region Public Methods and Operators

		/// <summary>
		/// The dispose.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion
	}
}