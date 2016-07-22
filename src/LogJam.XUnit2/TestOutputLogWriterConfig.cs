// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputLogWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
    using System;
    using System.Diagnostics.Contracts;

    using LogJam.Config;
    using LogJam.Trace;
    using LogJam.Writer.Text;

    using Xunit.Abstractions;


    /// <summary>
    /// Creates a <see cref="TextLogWriter" /> that writes to a <see cref="TestOutputFormatWriter"/> using the specified xunit2 <see cref="ITestOutputHelper" />.
    /// </summary>
    public sealed class TestOutputLogWriterConfig : TextLogWriterConfig, ITestOutputAccessor
    {

        private readonly ProxyTestOutputAccessor _proxyTestOutputAccessor;

        /// <summary>
        /// Initializes a new <see cref="TestOutputLogWriterConfig"/>.
        /// </summary>
        public TestOutputLogWriterConfig()
        {
            _proxyTestOutputAccessor = new ProxyTestOutputAccessor();
            _proxyTestOutputAccessor.AddTarget(this);

            // By default, tests are logged with an offset from start time, and no clock timestamp
            IncludeTimeOffset = true;
            IncludeTime = false;
        }

        /// <summary>
        /// Creates a new <see cref="TestOutputLogWriterConfig"/> that is configured to use <paramref name="testOutput"/>.
        /// </summary>
        /// <param name="testOutput"></param>
        public TestOutputLogWriterConfig(ITestOutputHelper testOutput)
            : this()
        {
            Contract.Requires<ArgumentNullException>(testOutput != null);

            TestOutput = testOutput;
        }

        /// <summary>
        /// The <see cref="ITestOutputHelper" /> to use to send log output to.
        /// </summary>
        public ITestOutputHelper TestOutput { get; set; }

        /// <summary>
        /// A <see cref="ITestOutputAccessor" /> that can be used to change the test output target after logging is started.
        /// </summary>
        public ITestOutputAccessor TestOutputAccessor { get { return _proxyTestOutputAccessor; } }

        /// <summary>
        /// <c>true</c> to include the time offset (since <see cref="StartTimeUtc" />)  when formatting timestamps.
        /// </summary>
        public bool IncludeTimeOffset { get; set; }

        protected override FormatWriter CreateFormatWriter(ITracerFactory setupTracerFactory)
        {
            var testOutputHelper = TestOutput;

            var formatWriter = new TestOutputFormatWriter(testOutputHelper, setupTracerFactory)
                               {
                                   IncludeTimeOffset = IncludeTimeOffset
                               };
            _proxyTestOutputAccessor.AddTarget(formatWriter);
            return formatWriter;
        }

    }

}
