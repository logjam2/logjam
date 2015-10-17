// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfigTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal.UnitTests.Trace
{
    using LogJam.Trace;
    using LogJam.Trace.Format;
    using LogJam.Writer;
    using LogJam.Writer.Text;

    using Xunit;


    /// <summary>
    /// Tests <see cref="TraceManager.Config" />, requiring internal access.
    /// </summary>
    public sealed class TraceManagerConfigTests
    {

        // Some tests return different values when running in a debugger.
        private static readonly bool s_inDebugger = System.Diagnostics.Debugger.IsAttached;

        /// <summary>
        /// By default, info and greater messages are written to a <see cref="DebuggerFormatWriter" />.
        /// </summary>
        [Fact]
        public void VerifyDefaultTraceManagerConfig()
        {
            using (var traceManager = new TraceManager())
            {
                var tracer = traceManager.TracerFor(this);
                Assert.Equal(s_inDebugger, tracer.IsInfoEnabled());
                Assert.False(tracer.IsVerboseEnabled());
                tracer.Info("Info message to debugger");

                AssertEquivalentToDefaultTraceManagerConfig(traceManager);
            }
        }

        public static void AssertEquivalentToDefaultTraceManagerConfig(TraceManager traceManager)
        {
            var tracer = traceManager.GetTracer("");
            Assert.Equal(s_inDebugger, tracer.IsInfoEnabled());
            Assert.False(tracer.IsVerboseEnabled());

            // Walk the Tracer object to ensure everything is as expected for default configuration
            Assert.IsType<TraceWriter>(tracer.Writer);
            var traceWriter = (TraceWriter) tracer.Writer;
            Assert.IsType<TextLogWriter.InnerEntryWriter<TraceEntry>>(traceWriter.InnerEntryWriter);
            var entryWriter = (TextLogWriter.InnerEntryWriter<TraceEntry>) traceWriter.InnerEntryWriter;
            Assert.IsType<DefaultTraceFormatter>(entryWriter.Formatter);
            Assert.IsType<DebuggerFormatWriter>(entryWriter.Parent);
        }

    }

}
