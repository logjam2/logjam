// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerTests.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Trace
{
	using LogJam.Trace;

	using Xunit;


	/// <summary>
	/// Unit tests for <see cref="TraceManager"/>
	/// </summary>
	public sealed class TraceManagerTests
	{

		[Fact]
		public void Each_TraceManager_has_a_LogManager()
		{
			using (var traceManager = new TraceManager())
			{
				var logManager = traceManager.LogManager;
				Assert.NotNull(logManager);
				Assert.Equal(traceManager.IsStarted, logManager.IsStarted);

				traceManager.Start();
				Assert.True(logManager.IsStarted);

				traceManager.Stop();
				Assert.False(logManager.IsStarted);
			}
		}

		[Fact(Skip = "nyi")]
		public void LogManager_can_be_passed_to_TraceManager_ctor()
		{
			
		}

	}

}