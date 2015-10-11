// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputLogWriterConfig.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using LogJam.Config;
    using LogJam.Format;
    using LogJam.Trace;
    using LogJam.Writer;

    using Xunit.Abstractions;


    /// <summary>
    /// Creates a <see cref="TestOutputLogWriter" /> using the specified xunit2 <see cref="ITestOutputHelper" />.
    /// </summary>
    public sealed class TestOutputLogWriterConfig : LogWriterConfig
    {

        // Configured formatters are stored as a list of logentry Type, logentry formatter, configure action
        private readonly List<Tuple<Type, object, Action<TestOutputLogWriter>>> _formatters = new List<Tuple<Type, object, Action<TestOutputLogWriter>>>();

        public TestOutputLogWriterConfig()
        {}

        public TestOutputLogWriterConfig(ITestOutputHelper testOutput)
        {
            Contract.Requires<ArgumentNullException>(testOutput != null);

            TestOutput = testOutput;
        }

        /// <summary>
        /// The <see cref="ITestOutputHelper" /> to use to send log output to.  Must be set before logging begins.
        /// </summary>
        public ITestOutputHelper TestOutput { get; set; }

        /// <summary>
        /// Configures use of <see cref="TestOutputTraceFormatter" />, which includes formatting showing the time since
        /// the test started, for each test.
        /// </summary>
        /// <returns></returns>
        public TestOutputLogWriterConfig UseTestTraceFormat()
        {
            Action<TestOutputLogWriter> configureAction = (mw) => mw.AddFormat(new TestOutputTraceFormatter());
            _formatters.Add(new Tuple<Type, object, Action<TestOutputLogWriter>>(typeof(TraceEntry), null, configureAction));
            return this;
        }

        /// <summary>
        /// Adds formatting for entry types <typeparamref name="TEntry" /> using <paramref name="formatter" />.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public TestOutputLogWriterConfig Format<TEntry>(LogFormatter<TEntry> formatter)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(formatter != null);

            Action<TestOutputLogWriter> configureAction = (mw) => mw.AddFormat(formatter);

            _formatters.Add(new Tuple<Type, object, Action<TestOutputLogWriter>>(typeof(TEntry), formatter, configureAction));
            return this;
        }

        /// <summary>
        /// Adds formatting for entry types <typeparamref name="TEntry" /> using <paramref name="formatAction" />.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="formatAction"></param>
        /// <returns></returns>
        public TestOutputLogWriterConfig Format<TEntry>(FormatAction<TEntry> formatAction)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(formatAction != null);

            return Format((LogFormatter<TEntry>) formatAction);
        }

        public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
        {
            var testOutputHelper = TestOutput;
            if (testOutputHelper == null)
            {
                throw new LogJamXUnitSetupException("TestOutputLogWriterConfig.TestOutput must be set before logging.", this);
            }

            var logWriter = new TestOutputLogWriter(testOutputHelper, setupTracerFactory);
            ApplyConfiguredFormatters(logWriter);
            return logWriter;
        }

        /// <summary>
        /// Applies all configured formatters to <paramref name="writer" />.
        /// </summary>
        /// <param name="writer"></param>
        private void ApplyConfiguredFormatters(TestOutputLogWriter writer)
        {
            foreach (var formatterTuple in _formatters)
            {
                var configureFormatterAction = formatterTuple.Item3;
                configureFormatterAction(writer);
            }
        }

    }

}
