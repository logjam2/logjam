// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseExistingLogWriterConfig.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Trace;
	using LogJam.Writer;


	/// <summary>
	/// An <see cref="ILogWriterConfig"/> that always returns the <see cref="ILogWriter"/> that is passed in.
	/// Supports creating an <see cref="ILogWriter"/>, then passing that into configuration methods.
	/// </summary>
	public class UseExistingLogWriterConfig : LogWriterConfig
	{

		private readonly ILogWriter _logWriter;


		/// <summary>
		/// Creates a new <see cref="UseExistingLogWriterConfig"/> instance, which will result in
		/// log entries being written to <paramref name="logWriter"/>.
		/// </summary>
		/// <param name="logWriter"></param>
		/// <param name="disposeOnStop">Set to <c>true</c> to dispose <paramref name="logWriter"/> when the 
		/// <see cref="LogManager"/> is stopped.  By default <c>disposeOnStop</c> is false.</param>
		public UseExistingLogWriterConfig(ILogWriter logWriter, bool disposeOnStop = false)
		{
			Contract.Requires<ArgumentNullException>(logWriter != null);

			_logWriter = logWriter;
			DisposeOnStop = disposeOnStop;
		}

		internal ILogWriter LogWriter { get { return _logWriter; } }

		public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
		{
			return _logWriter;
		}

	}

}