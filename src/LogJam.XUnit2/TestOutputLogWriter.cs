// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputLogWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
    using System;
    using System.Diagnostics.Contracts;

    using LogJam.Format;
    using LogJam.Trace;
    using LogJam.Writer;

    using Xunit.Abstractions;


    /// <summary>
    /// Formats and writes log entries to an <see cref="ITestOutputHelper" />. This enables
    /// capturing trace and other log output and writing it to a test's xunit 2.0 test output.
    /// </summary>
    public sealed class TestOutputLogWriter : TextLogWriter
    {

        private readonly ITestOutputHelper _testOutput;

        /// <summary>
        /// Creates a new <see cref="TestOutputLogWriter" />.
        /// </summary>
        /// <param name="testOutput">The xunit <see cref="ITestOutputHelper" /> to write formatted log output to.</param>
        /// <param name="setupTracerFactory">The <see cref="ITracerFactory" /> used to trace setup operations.</param>
        public TestOutputLogWriter(ITestOutputHelper testOutput, ITracerFactory setupTracerFactory)
            : base(setupTracerFactory)
        {
            Contract.Requires<ArgumentNullException>(testOutput != null);

            _testOutput = testOutput;
        }

        protected override void WriteFormattedEntry(string formattedEntry)
        {
            _testOutput.WriteLine(formattedEntry);
        }

    }

}
