// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestEntry.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal.UnitTests.Examples
{
	using System.Diagnostics;
	using System.Threading;


	/// <summary>
	/// A test <see cref="ILogEntry"/> struct used for testing and perf testing.
	/// </summary>
	public struct TestEntry : ILogEntry
	{

		public TestEntry(ref int counter)
		{
			entryCreateTimestamp = Stopwatch.GetTimestamp();
			index = Interlocked.Increment(ref counter);
			logWriteTimestamp = 0;
		}

		/// <summary>
		/// The <see cref="Stopwatch"/> timestamp that the <see cref="TestEntry"/> was created;
		/// </summary>
		public readonly long entryCreateTimestamp;
		/// <summary>
		/// The index of this test entry.
		/// </summary>
		public readonly int index;
		/// <summary>
		/// When the <see cref="TestEntry"/> is written to a test log.
		/// </summary>
		public long logWriteTimestamp;

		/// <summary>
		/// Returns "how long" it took to log this entry.
		/// </summary>
		public long TicksToLog
		{
			get
			{
				if (logWriteTimestamp == 0)
				{
					return 0;
				}
				else
				{
					return logWriteTimestamp - entryCreateTimestamp;
				}
			}
		}

	}

}