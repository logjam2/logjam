// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMultiLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using LogJam.Writers;


	/// <summary>
	/// Supports multiplexing multiple strongly-typed log writers to a single log target.
	/// </summary>
	public interface IMultiLogWriter : ILogWriter
	{

		/// <summary>
		/// Returns an <see cref="ILogWriter{TEntry}"/> that writes <see cref="TEntry"/> instances 
		/// to the log target.
		/// </summary>
		/// <typeparam name="TEntry">A log entry type</typeparam>
		/// <paramref name="logWriter">An <see cref="ILogWriter{TEntry}"/> that writes entries to this, or a
		/// <see cref="NoOpLogWriter{TEntry}"/> if no functioning log writer can be returned.</paramref>
		/// <returns><c>true</c> if a functioning log writer is returned in <paramref name="logWriter"/>;
		/// <c>false</c> if a functioning log writer could not be obtained.</returns>
		bool GetLogWriter<TEntry>(out ILogWriter<TEntry> logWriter) 
			where TEntry : ILogEntry;

	}

}