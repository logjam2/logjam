// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerfTests.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.UnitTests
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using Microsoft.Owin.Testing;

	using Xunit.Extensions;


	/// <summary>
	/// Runs perf test on standard <see cref="LogJam.Owin"/> setup.
	/// </summary>
	public sealed class PerfTests : BaseOwinTest
	{

		[Theory]
		[InlineData(5, 100, 3)]
		[InlineData(50, 100, 30)]
		public void ParallelTraceTest(int threads, int requestsPerThread, int tracesPerRequest)
		{
			// Test logging to TextWriter.Null - which should have no perf overhead
			using (TestServer testServer = CreateTestServer(TextWriter.Null))
			{

				Action testCalls = () =>
				                   {
					                   var task = testServer.CreateRequest("/trace?traceCount=" + tracesPerRequest).GetAsync();
					                   var response = task.Result; // Wait for the call to complete
				                   };

				Parallel.Invoke(Enumerable.Repeat(testCalls, threads).ToArray());
			}
		}

	}

}