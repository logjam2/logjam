// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterActionPriority.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{

	/// <summary>
	/// Specifies the priority of actions passed to <see cref="ISynchronizingLogWriter.QueueSynchronized"/>
	/// </summary>
	public enum LogWriterActionPriority : byte
	{
		/// <summary>
		/// Priority not set.
		/// </summary>
		Uninitialized = 0,
		/// <summary>
		/// Delaying completion is preferred - eg after currently queued actions are completed.
		/// </summary>
		Delay = 1,
		/// <summary>
		/// Run the action in log entry queue order.
		/// </summary>
		Normal = 2,
		/// <summary>
		/// Run the action as soon as possible.
		/// </summary>
		High = 3
	}

}
