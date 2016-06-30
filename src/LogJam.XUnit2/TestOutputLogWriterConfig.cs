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
	using Writer;

	/// <summary>
	/// Creates a <see cref="TestOutputFormatWriter" /> using the specified xunit2 <see cref="ITestOutputHelper" />.
	/// </summary>
	public sealed class TestOutputLogWriterConfig : TextLogWriterConfig
	{

		private readonly ITestOutputLogWriterConfig _innerTextLogWriterConfig;

        public TestOutputLogWriterConfig()
        {
            // By default, tests are logged with an offset from start time, and no clock timestamp
            IncludeTimeOffset = true;
            IncludeTime = false;
			_innerTextLogWriterConfig = new InnerTestOutputLogConfig(this);
        }

        public TestOutputLogWriterConfig(ITestOutputHelper testOutput)
            : this()
        {
            Contract.Requires<ArgumentNullException>(testOutput != null);

            TestOutput = testOutput;
        }

        /// <summary>
        /// The <see cref="ITestOutputHelper" /> to use to send log output to.  Must be set before logging begins.
        /// </summary>
        public ITestOutputHelper TestOutput { get; private set; }

        /// <summary>
        /// <c>true</c> to include the time offset (since <see cref="StartTimeUtc" />)  when formatting timestamps.
        /// </summary>
        public bool IncludeTimeOffset { get; set; }

		/// <inheritdoc />
		public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
		{
			return new TestOuputLogWriter(setupTracerFactory, _innerTextLogWriterConfig);
		}

		protected override FormatWriter CreateFormatWriter(ITracerFactory setupTracerFactory)
		{
			var testOutputHelper = TestOutput;
			if (testOutputHelper == null)
			{
				throw new LogJamXUnitSetupException("TestOutputLogWriterConfig.TestOutput must be set before logging.", this);
			}

			return new TestOutputFormatWriter(testOutputHelper, setupTracerFactory)
			{
				IncludeTimeOffset = IncludeTimeOffset
			};
		}

		private ILogWriter CreateInnerLogWriter(ITracerFactory setuTracerFactory)
		{
			return base.CreateLogWriter(setuTracerFactory);
		}

		private class InnerTestOutputLogConfig : ITestOutputLogWriterConfig
		{

			private readonly TestOutputLogWriterConfig _parent;
			public InnerTestOutputLogConfig(TestOutputLogWriterConfig parent)
			{
				_parent = parent;
			}

			public ITestOutputHelper TestOutput
			{
				get { return _parent.TestOutput; }
				set { _parent.TestOutput = value; }
			}

			public ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
			{
				return _parent.CreateInnerLogWriter(setupTracerFactory);
			}

			public bool BackgroundLogging
			{
				get { return _parent.BackgroundLogging; }
				set { throw new NotSupportedException(); }
			}

			public bool DisposeOnStop
			{
				get { return _parent.DisposeOnStop; }
				set { throw new NotSupportedException(); }
			}

			public bool Synchronized
			{
				get { return _parent.Synchronized; }
				set { throw new NotSupportedException(); }
			}
		}
    }

}
