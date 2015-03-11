// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterMultiLogWriterConfig.cs">
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
	/// Configures use of a <see cref="TextWriterMultiLogWriter"/>.
	/// </summary>
	public class TextWriterMultiLogWriterConfig : ILogWriterConfig
	{

		private readonly Func<TextWriter> _createTextWriter;
		// Configured formatters are stored as a list of logentry Type, logentry formatter, configure action
		private readonly List<Tuple<Type, object, Action<TextWriterMultiLogWriter>>> _formatters;

		/// <summary>
		/// Initializes a new config object that will create <see cref="TextWriterMultiLogWriter"/> instances
		/// that write to a <see cref="TextWriter"/> returned from <paramref name="createTextWriterFunc"/>.
		/// </summary>
		/// <param name="createTextWriterFunc">A function that returns a <see cref="TextWriter"/>.  This function is
		/// called each time the parent <see cref="LogManager"/> is <c>Start()</c>ed.</param>
		public TextWriterMultiLogWriterConfig(Func<TextWriter> createTextWriterFunc)
		{
			Contract.Requires<ArgumentNullException>(createTextWriterFunc != null);

			_createTextWriter = createTextWriterFunc;
			_formatters = new List<Tuple<Type, object, Action<TextWriterMultiLogWriter>>>();
			DisposeTextWriter = true;
		}

		/// <summary>
		/// Initializes a new config object that will create <see cref="TextWriterMultiLogWriter"/> instances
		/// that write to <paramref name="textWriter"/>.
		/// </summary>
		/// <param name="textWriter"></param>
		/// <remarks>
		/// Note that <paramref name="textWriter"/> will not be automatically <c>Dispose()</c>ed each time
		/// the <see cref="LogManager"/> is stopped.  To cause automatic <c>Dispose()</c>, set
		/// <see cref="DisposeTextWriter"/> to <c>true</c>.
		/// </remarks>
		public TextWriterMultiLogWriterConfig(TextWriter textWriter)
			: this(() => textWriter)
		{
			Contract.Requires<ArgumentNullException>(textWriter != null);

			// Default to not Disposing the TextWriter
			DisposeTextWriter = false;
		}

		/// <summary>
		/// Adds formatting for entry types <see cref="TEntry"/> using <paramref name="formatter"/>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="formatter"></param>
		/// <returns></returns>
		public TextWriterMultiLogWriterConfig Format<TEntry>(LogFormatter<TEntry> formatter)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(formatter != null);

			Action<TextWriterMultiLogWriter> configureAction = (mw) => mw.AddFormat(formatter);

			_formatters.Add(new Tuple<Type, object, Action<TextWriterMultiLogWriter>>(typeof(TEntry), formatter, configureAction));
			return this;
		}

		/// <summary>
		/// Adds formatting for entry types <see cref="TEntry"/> using <paramref name="formatAction"/>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="formatAction"></param>
		/// <returns></returns>
		public TextWriterMultiLogWriterConfig Format<TEntry>(FormatAction<TEntry> formatAction)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(formatAction != null);

			return Format((LogFormatter<TEntry>) formatAction);
		}

		/// <summary>
		/// Set to <see cref="true"/> to call <see cref="IDisposable.Dispose"/> on the created <see cref="TextWriter"/>.
		/// </summary>
		public bool DisposeTextWriter { get; set; }

		#region ILogWriterConfig

		public bool Equals(ILogWriterConfig other)
		{
			TextWriterMultiLogWriterConfig typedOther = other as TextWriterMultiLogWriterConfig;
			if (typedOther == null)
			{
				return false;
			}

			return _createTextWriter == typedOther._createTextWriter
			       && Synchronized == typedOther.Synchronized
			       && EqualityUtil.AreEquivalent(_formatters, typedOther._formatters);
		}

		public bool Synchronized { get; set; }

		public ILogWriter CreateILogWriter(ITracerFactory setupTracerFactory)
		{
			var writer = new TextWriterMultiLogWriter(_createTextWriter(), setupTracerFactory, Synchronized, DisposeTextWriter);
			foreach (var formatterTuple in _formatters)
			{
				var configureFormatterAction = formatterTuple.Item3;
				configureFormatterAction(writer);
			}
			return writer;
		}

		#endregion

	}

}