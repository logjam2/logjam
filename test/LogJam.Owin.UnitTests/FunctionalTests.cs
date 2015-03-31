// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionalTests.cs">
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

	using LogJam.Trace;

	using Microsoft.Owin.Testing;

	using Xunit;


	/// <summary>
	/// Functional tests for <see cref="LogJam.Owin"/>.
	/// </summary>
	public sealed class FunctionalTests : BaseOwinTest
	{
		[Fact]
		public void SingleRequestWithTracing()
		{
			var setupLog = new SetupLog();
			var stringWriter = new StringWriter();
			using (TestServer testServer = CreateTestServer(stringWriter, setupLog))
			{
				IssueTraceRequest(testServer, 2);
			}

			Console.WriteLine(stringWriter);

			Assert.NotEmpty(setupLog);
			Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
		}

		[Fact]
		public void ExceptionTracing()
		{
			var setupLog = new SetupLog();
			var stringWriter = new StringWriter();
			using (TestServer testServer = CreateTestServer(stringWriter, setupLog))
			{
				var task = testServer.CreateRequest("/exception").GetAsync();
				var response = task.Result; // Wait for the call to complete
			}

			Console.WriteLine(stringWriter);

			Assert.NotEmpty(setupLog);
			Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
		}

	}

}