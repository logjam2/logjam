// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Writer
{

	/// <summary>
	/// Supports writing strongly-typed log entries to a log target.
	/// </summary>
	/// <typeparam name="TEntry">The base entry type supported by the log writer.</typeparam>
	public interface ILogWriter<TEntry> : ILogWriter where TEntry : ILogEntry
	{

		/// <summary>
		/// Writes <paramref name="entry"/> to the log target.
		/// </summary>
		/// <param name="entry">The log entry to write.</param>
		void Write(ref TEntry entry);

	}

}
