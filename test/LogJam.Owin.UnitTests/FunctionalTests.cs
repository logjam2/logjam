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

	using global::Owin;

	using LogJam.Config;
	using LogJam.Owin.Http;
	using LogJam.Trace;
	using LogJam.Trace.Config;
	using LogJam.Trace.Format;
	using LogJam.Trace.Switches;

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
		public void SingleRequestWithTracingWith3LogTargets()
		{
			var setupLog = new SetupLog();
			var stringWriter = new StringWriter();
			using (TestServer testServer = new OwinTestWith3LogTargets().CreateTestServer(stringWriter, setupLog, true))
			{
				IssueTraceRequest(testServer, 2);
			}
			Console.WriteLine("Logging complete.");

			Console.WriteLine();
			Console.WriteLine("StringWriter contents:");
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

		/// <summary>
		/// Sets up a TestServer with more complex logging, same handlers as in BaseOwinTest
		/// </summary>
		private class OwinTestWith3LogTargets : BaseOwinTest 
		{

			protected override void ConfigureLogging(IAppBuilder appBuilder, TextWriter logTarget, bool backgroundThreadLogging)
			{
				var logManagerConfig = appBuilder.GetLogManagerConfig();
				var traceManagerConfig = appBuilder.GetTraceManagerConfig();
				var traceSwitches = new SwitchSet()
			       {
				       { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Verbose) }
			       };

				// Log to debugger
				var debuggerConfig = logManagerConfig.UseDebugger()
													 .Format(new DebuggerTraceFormatter())
													 .Format(new HttpRequestFormatter())
													 .Format(new HttpResponseFormatter());
				debuggerConfig.BackgroundLogging = backgroundThreadLogging;
				traceManagerConfig.TraceTo(debuggerConfig, traceSwitches);

				// Log to console
				var consoleConfig = new ConsoleLogWriterConfig()
													 .Format(new DebuggerTraceFormatter())
													 .Format(new HttpRequestFormatter())
													 .Format(new HttpResponseFormatter());
				consoleConfig.BackgroundLogging = backgroundThreadLogging;
				logManagerConfig.Writers.Add(consoleConfig);
				traceManagerConfig.TraceTo(consoleConfig, traceSwitches);

				// Log to TextWriter
				var textLogConfig = logManagerConfig.UseTextWriter(logTarget)
							  .Format(new DebuggerTraceFormatter() { IncludeTimestamp = true })
							  .Format(new HttpRequestFormatter())
							  .Format(new HttpResponseFormatter());
				textLogConfig.BackgroundLogging = backgroundThreadLogging;
				traceManagerConfig.TraceTo(textLogConfig, traceSwitches);

				appBuilder.LogHttpRequests(debuggerConfig, consoleConfig, textLogConfig);
				appBuilder.UseOwinTracerLogging();			
			}

		}

	}

}