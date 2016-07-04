// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamManagerExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
	using System;
	using System.Collections.Generic;

	using LogJam.Config;
	using LogJam.Writer;
	using LogJam.XUnit2.Util;

	using Xunit.Abstractions;

	/// <summary>
	/// Extension methods for <see cref="LogManager"/>
	/// </summary>
	public static class LogJamManagerExtensions
	{
		/// <summary>
		/// Sets the test output for a given <see cref="ILogWriterConfig"/>
		/// </summary>
		/// <param name="logManager"></param>
		/// <param name="logWriterConfig"></param>
		/// <param name="testOutput"></param>
		/// <exception cref="KeyNotFoundException">Thrown when the <see cref="LogManager"/> does not contain the <see cref="ILogWriterConfig"/></exception>
		/// <exception cref="InvalidOperationException">Thrown when the <see cref="ILogWriter"/> corresponding to <see cref="ILogWriterConfig"/> does not support setting testOutput (eg. the corresponding writer is not of type TestOutputLogWriter</exception>
		public static void SetTestOutputForWriterConfig(this LogManager logManager,
										 ILogWriterConfig logWriterConfig,
										 ITestOutputHelper testOutput)
		{
			ILogWriter logWriter;
			if (! logManager.TryGetLogWriter(logWriterConfig, out logWriter))
			{
				throw new KeyNotFoundException("LogManager does not contain logWriterConfig: " + logWriterConfig);
			}

			var testOutputLogWriter = (logWriter is ProxyLogWriter
										   ? ((ProxyLogWriter) logWriter).InnerLogWriter
										   : logWriter) as TestOuputLogWriter;
			if (testOutputLogWriter == null)
			{
				throw new InvalidOperationException($"The logWrterConfig of type {logWriter.GetType().GetCSharpName()} does not support writing to testOutput");
			}

			testOutputLogWriter.SetTestOutput(testOutput);
		}

	}
}
