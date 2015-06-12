// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterLogWriterConfig.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;

	using LogJam.Format;
	using LogJam.Trace;
	using LogJam.Util;
	using LogJam.Writer;


	/// <summary>
	/// Configures use of a <see cref="TextWriterLogWriter"/>.
	/// </summary>
	public class TextWriterLogWriterConfig : TextLogWriterConfig
	{

		/// <summary>
		/// Initializes a new config object that will create <see cref="TextWriterLogWriter"/> instances
		/// that write to a <see cref="TextWriter"/> returned from <paramref name="createTextWriterFunc"/>.
		/// </summary>
		/// <param name="createTextWriterFunc">A function that returns a <see cref="TextWriter"/>.  This function is
		/// called each time the parent <see cref="LogManager"/> is <c>Start()</c>ed.</param>
		public TextWriterLogWriterConfig(Func<TextWriter> createTextWriterFunc)
		{
			Contract.Requires<ArgumentNullException>(createTextWriterFunc != null);

			CreateTextWriter = createTextWriterFunc;
			DisposeTextWriter = true;
		}

		/// <summary>
		/// Initializes a new config object that will create <see cref="TextWriterLogWriter"/> instances
		/// that write to <paramref name="textWriter"/>.
		/// </summary>
		/// <param name="textWriter"></param>
		/// <remarks>
		/// Note that <paramref name="textWriter"/> will not be automatically <c>Dispose()</c>ed each time
		/// the <see cref="LogManager"/> is stopped.  To cause automatic <c>Dispose()</c>, set
		/// <see cref="DisposeTextWriter"/> to <c>true</c>.
		/// </remarks>
		public TextWriterLogWriterConfig(TextWriter textWriter)
			: this(() => textWriter)
		{
			Contract.Requires<ArgumentNullException>(textWriter != null);

			// Default to not Disposing the TextWriter
			DisposeTextWriter = false;
		}

		/// <summary>
		/// Initializes a new <see cref="TextWriterLogWriterConfig"/>.  <see cref="CreateTextWriter"/> must be set before 
		/// the config object can be used.
		/// </summary>
		public TextWriterLogWriterConfig()
		{
			DisposeTextWriter = true;
		}

		public Func<TextWriter> CreateTextWriter { get; set; } 

		/// <summary>
		/// Set to <c>true</c> to call <see cref="IDisposable.Dispose"/> on the created <see cref="TextWriter"/>.
		/// </summary>
		public bool DisposeTextWriter { get; set; }

		#region ILogWriterConfig

		public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
		{
			if (CreateTextWriter == null)
			{
				throw new LogJamSetupException("CreateTextWriter delegate must be set before " + GetType() + " can create a LogWriter.", this);
			}

			var writer = new TextWriterLogWriter(CreateTextWriter(), setupTracerFactory, Synchronized, DisposeTextWriter);
			ApplyConfiguredFormatters(writer);
			return writer;
		}

		#endregion

	}

}