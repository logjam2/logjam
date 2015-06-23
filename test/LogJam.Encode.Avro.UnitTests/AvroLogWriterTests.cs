// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvroLogWriterTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode.Avro.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using LogJam.Trace;
    using LogJam.Writer;

    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Container;

    using Xunit;
    using Xunit.Abstractions;

    using TraceLevel = LogJam.Trace.TraceLevel;


    /// <summary>
    /// Playing around with what Avro can do for logging.
    /// </summary>
    public sealed class AvroLogWriterTests : BaseTest
    {

        public AvroLogWriterTests(ITestOutputHelper testOutput)
            : base(testOutput)
        { }

        [Fact]
        public void RoundtripLog()
        {
            string tracerName = typeof(AvroLogWriterTests).FullName;

            // Log these 4 entries
            var applicationEntry = new ApplicationEntry(typeof(AvroLogWriterTests).Assembly);
            var requestHeaderEntry = new LogRequestHeader(applicationEntry);
            var traceEntry1 = new TraceEntry(tracerName, TraceLevel.Info, "Info message");
            var traceEntry2 = new TraceEntry(tracerName, TraceLevel.Verbose, "Verbose message");

            var setupLog = new SetupLog();
            using (var stream = new MemoryStream())
            {
                using (var avroLogWriter = new AvroLogWriter(setupLog, stream, false))
                {
                    var requestHeaderWriter = avroLogWriter.AddEntryType<LogRequestHeader>();
                    var appEntryWriter = avroLogWriter.AddEntryType<ApplicationEntry>();
                    var traceEntryWriter = avroLogWriter.AddEntryType<TraceEntry>();
                    testOutput.WriteLine("Avro schema: {0}", avroLogWriter.GetAvroSchema());
                    avroLogWriter.Start();
                    
                    // Log the 4 entries
                    requestHeaderWriter.Write(ref requestHeaderEntry);
                    appEntryWriter.Write(ref applicationEntry);
                    traceEntryWriter.Write(ref traceEntry1);
                    traceEntryWriter.Write(ref traceEntry2);

                    Assert.False(setupLog.HasAnyExceeding(TraceLevel.Info));
                }

                tracer.Info("Completed write to stream, bytes: {0}", stream.Position);

                stream.Seek(0, SeekOrigin.Begin);

                using (var avroReader = AvroContainer.CreateGenericReader(stream))
                using (var seqReader = new SequentialReader<object>(avroReader))
                {
                    var schema = avroReader.Schema;
                    AvroRecord[] logEntriesRead = seqReader.Objects.Cast<AvroRecord>().ToArray();
                    Assert.Equal(4, logEntriesRead.Length);
                    Assert.Equal("LogRequestHeader", logEntriesRead[0].Schema.Name);
                    Assert.Equal("ApplicationEntry", logEntriesRead[1].Schema.Name);
                    Assert.True("Verbose" == ((dynamic) logEntriesRead[3]).TraceLevel.Value);
                }
            }
        }

    }

}