// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceFormattingTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Trace
{
    using System.IO;

    using LogJam.Config;
    using LogJam.Trace;
    using LogJam.Writer.Text;

    using Xunit;


    /// <summary>
    /// Tests formatting for tracing.
    /// </summary>
    public sealed class TraceFormattingTests
    {

        [Fact]
        public void CustomTraceFormatting()
        {
            // Text output is written here
            StringWriter traceOutput = new StringWriter();

            // Can either use a EntryFormatAction, or subclass EntryFormatter<TEntry>.  Here we're using a EntryFormatAction.
            // Note that subclassing EntryFormatter<TEntry> provides a slightly more efficient code-path.
            EntryFormatAction<TraceEntry> format = (traceEntry, formatWriter) =>
            {
                formatWriter.BeginEntry();
                formatWriter.WriteField(traceEntry.TraceLevel.ToString());
                formatWriter.EndEntry();
            };

            using (var traceManager = new TraceManager(new TextWriterLogWriterConfig(traceOutput).Format(format)))
            {
                var tracer = traceManager.TracerFor(this);
                tracer.Info("m");
                tracer.Error("m");
            }

            Assert.Equal("Info\r\nError\r\n", traceOutput.ToString());
        }


    }

}