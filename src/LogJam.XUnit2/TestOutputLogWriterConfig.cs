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
	using System.Diagnostics.Contracts;

	using LogJam.Config;
	using LogJam.Trace;
	using LogJam.Writer.Text;

	using Xunit.Abstractions;


	/// <summary>
	/// Creates a <see cref="TestOutputFormatWriter" /> using the specified xunit2 <see cref="ITestOutputHelper" />.
	/// </summary>
	public sealed class TestOutputLogWriterConfig : TextLogWriterConfig
	{

		public TestOutputLogWriterConfig()
		{
			// By default, tests are logged with an offset from start time, and no clock timestamp
			IncludeTimeOffset = true;
			IncludeTime = false;
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
		public ITestOutputHelper TestOutput { get; set; }

		/// <summary>
		/// <c>true</c> to include the time offset (since <see cref="StartTimeUtc" />)  when formatting timestamps.
		/// </summary>
		public bool IncludeTimeOffset { get; set; }

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

	}

}
