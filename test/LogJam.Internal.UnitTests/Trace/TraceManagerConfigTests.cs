// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfigTests.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal.UnitTests.Trace
{
	using LogJam.Trace;
	using LogJam.Trace.Format;
	using LogJam.Writer;

	using Xunit;


	/// <summary>
	/// Tests <see cref="TraceManager.Config"/>, requiring internal access.
	/// </summary>
	public sealed class TraceManagerConfigTests
	{

		/// <summary>
		/// By default, info and greater messages are written to a <see cref="DebuggerLogWriter"/>.
		/// </summary>
		[Fact]
		public void VerifyDefaultTraceManagerConfig()
		{
			using (var traceManager = new TraceManager())
			{
				var tracer = traceManager.TracerFor(this);
				Assert.True(tracer.IsInfoEnabled());
				Assert.False(tracer.IsVerboseEnabled());
				tracer.Info("Info message to debugger");

				AssertEquivalentToDefaultTraceManagerConfig(traceManager);
			}
		}

		public static void AssertEquivalentToDefaultTraceManagerConfig(TraceManager traceManager)
		{
			var tracer = traceManager.GetTracer("");
			Assert.True(tracer.IsInfoEnabled());
			Assert.False(tracer.IsVerboseEnabled());

			// Walk the Tracer object to ensure everything is as expected for default configuration
			Assert.IsType<TraceWriter>(tracer.Writer);
			var traceWriter = (TraceWriter) tracer.Writer;
			Assert.IsType<TextWriterLogWriter<TraceEntry>>(traceWriter.InnerLogWriter);
			var logWriter = (TextWriterLogWriter<TraceEntry>) traceWriter.InnerLogWriter;
			Assert.IsType<DebuggerTraceFormatter>(logWriter.Formatter);
			Assert.IsType<DebuggerTextWriter>(logWriter.TextWriter);
		}

	}

}