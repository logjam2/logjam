// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartableState.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{

	/// <summary>
	/// State values for <see cref="IStartable"/> instances.
	/// </summary>
	public enum StartableState : byte
	{

		Unstarted = 0,
		Starting = 1,
		Started = 2,
		Stopping = 3,
		Stopped = 4,
		FailedToStart = 5,
		FailedToStop = 6

	}

}