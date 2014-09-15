// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerTests.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests
{
	using System.Linq;

	using LogJam.Trace;

	using Xunit;


	/// <summary>
	/// Unit tests for <see cref="LogManager"/>.
	/// </summary>
	public sealed class LogManagerTests
	{
		[Fact]
		public void Default_LogManager_tracks_all_LogJam_operations_to_StatusTraces()
		{
			string testMessage = "test LogJam setup message";
			var traceLevel = TraceLevel.Info;

			using (var logManager = new LogManager())
			{
				// Trace a message
				var internalTracer = logManager.SetupTracerFactory.TracerFor(this);
				internalTracer.Trace(traceLevel, null, testMessage);

				// Verify the message can be found in LogJamTraceEntries
				var traceEntry = logManager.SetupTraces.First(e => e.Message == testMessage);
				Assert.Equal(traceLevel, traceEntry.TraceLevel);
				Assert.Equal(internalTracer.Name, traceEntry.TracerName);
				int countTraces1 = logManager.SetupTraces.Count();

				// Messing with the SetupTracerFactory doesn't force a Start()
				Assert.False(logManager.IsStarted);

				// Start and stop create trace records
				logManager.Start();
				int countTracesAfterStart = logManager.SetupTraces.Count();
				Assert.True(countTracesAfterStart > countTraces1);

				logManager.Stop();
				int countTracesAfterStop = logManager.SetupTraces.Count();
				Assert.True(countTracesAfterStop > countTracesAfterStart);
			}
		}

		[Fact]
		public void LogManager_internal_operations_can_be_directed_to_another_output()
		{
			
		}

	}

}