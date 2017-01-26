// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISynchronizingLogWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using System;
	using System.Diagnostics.Contracts;


	/// <summary>
	/// A <see cref="ILogWriter"/> that additionally supports synchronizing arbitrary operations (eg file rotation, flushing) so 
	/// the operation doesn't overlap with other logging operations (eg writing, flushing).
	/// </summary>
	[ContractClass(typeof(ISynchronizingLogWriterContract))]
	public interface ISynchronizingLogWriter
	{

		/// <summary>
		/// Queues an action to execute as soon as possible, but after currently running or queued operations complete, and synchronized with other 
		/// operations on the same <see cref="ILogWriter"/>.  An <see cref="Action"/> queued with this method must be synchronized so that it
		/// does not run concurrently with other queued actions or other log writer operations.
		/// </summary>
		/// <param name="action">The <see cref="Action"/> to run in a synchronized context.</param>
		/// <param name="priority">The priority setting for running <paramref name="action"/>.</param>
		void QueueSynchronized(Action action, LogWriterActionPriority priority);

	}

	[ContractClassFor(typeof(ISynchronizingLogWriter))]
	internal abstract class ISynchronizingLogWriterContract : ISynchronizingLogWriter
	{

		public void QueueSynchronized(Action action, LogWriterActionPriority priority)
		{
			Contract.Requires<ArgumentNullException>(action != null);

			throw new NotImplementedException();
		}

	}

}