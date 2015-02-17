// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueueLogWriterOfTEntry.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{

	/// <summary>
	/// Abstract <see cref="ILogWriter{TEntry}"/> that writes to the tail of a queue, and supports reading
	/// from the head of the queue.
	/// </summary>
	internal interface IQueueLogWriter<TEntry> : ILogWriter<TEntry>, IStartable
		where TEntry : ILogEntry
	{
		/// <summary>
		/// Returns <c>true</c> if there are no queued entries held.
		/// Returns <c>false</c> if there are 1 or more entries held in the queue.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Tries to remove and return the log entry at the beginning of the queue.
		/// </summary>
		/// <param name="logEntry">The entry that is read.</param>
		/// <returns><c>true</c> if an entry was removed and returned from the beginning of the queue; <c>false</c> if no
		/// entry could be retrieved from the queue.</returns>
		bool TryDequeue(out TEntry logEntry);

	}

}