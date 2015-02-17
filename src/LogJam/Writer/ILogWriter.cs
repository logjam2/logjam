// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{

	/// <summary>
	/// Untyped log writer interface.
	/// </summary>
	/// <seealso cref="ILogWriter{TEntry}"/>.
	public interface ILogWriter
	{
		/// <summary>
		/// Returns <c>true</c> if this <see cref="ILogWriter"/> can write entries to its target.
		/// </summary>
		/// <value>
		/// If <c>true</c>, this <c>ILogWriter</c> can write entries.  If <c>false</c>, <see cref="ILogWriter{TEntry}.Write"/> should not be called.
		/// </value>
		bool Enabled { get; }

		/// <summary>
		/// Returns <c>true</c> if calls to this object's methods and properties are synchronized.
		/// </summary>
		/// <value>
		/// If <c>true</c>, calls on this object are threadsafe.  If <c>false</c>, thread-safety is not guaranteed.
		/// </value>
		bool IsSynchronized { get; }

	}

}