// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferingLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Util;


	/// <summary>
	/// Shared functionality for log writers that write to buffers.
	/// </summary>
	public static class BufferingLogWriter
	{

		/// <summary>
		/// Predicate that causes an <see cref="IBufferingLogWriter"/> to flush after every log entry is written.
		/// </summary>
		public static readonly Func<bool> AlwaysFlush = () => true;

		/// <summary>
		/// Predicate that causes an <see cref="IBufferingLogWriter"/> to never automatically flush.  In most cases this means
		/// that flushing only occurs when the buffer is filled.
		/// </summary>
		public static readonly Func<bool> NeverFlush = () => false;


	}

}