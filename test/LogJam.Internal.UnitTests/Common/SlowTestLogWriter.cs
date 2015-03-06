// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlowTestLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace LogJam.UnitTests.Common
{
	using System.Threading;


	/// <summary>
	/// Fakes consistent slow logging to verify proper handling of slow log writers by LogJam.
	/// </summary>
	public sealed class SlowTestLogWriter<TEntry> : TestLogWriter<TEntry>
		where TEntry : ILogEntry
	{

		internal SlowTestLogWriter(int msDelay, bool synchronize)
			: base(synchronize)
		{
			WriteEntryDelayMs = msDelay;
			StartDelayMs = msDelay;
			StopDelayMs = msDelay;
			DisposeDelayMs = msDelay;
		}

		internal int WriteEntryDelayMs { get; set; }
		internal int StartDelayMs { get; set; }
		internal int StopDelayMs { get; set; }
		internal int DisposeDelayMs { get; set; }

		public override void Start()
		{
			Thread.Sleep(StartDelayMs);
			base.Start();
		}

		public override void Stop()
		{
			if (IsStarted)
			{
				Thread.Sleep(StopDelayMs);
				base.Stop();
			}
		}

		public override void Write(ref TEntry entry)
		{
			if (IsStarted)
			{
				Thread.Sleep(WriteEntryDelayMs);
				base.Write(ref entry);
			}
		}

		public override void Dispose()
		{
			if (! IsDisposed)
			{
				Thread.Sleep(DisposeDelayMs);
				base.Dispose();
			}
		}

	}

}