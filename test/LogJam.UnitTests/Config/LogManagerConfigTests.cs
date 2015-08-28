// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerConfigTests.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Config
{
	using LogJam.Config;

	using Xunit;


	/// <summary>
	/// Exercises <see cref="LogManagerConfig"/>.
	/// </summary>
	public sealed class LogManagerConfigTests
	{

		[Fact]
		public void DefaultLogManagerHasEmptyConfig()
		{
			using (var logManager = new LogManager())
			{
				Assert.Empty(logManager.Config.Writers);
				Assert.False(logManager.IsStarted);
				Assert.True(logManager.IsHealthy);
			}
		}

	}

}