// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using LogJam.Format;
	using LogJam.Trace;
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Threading;


	/// <summary>
	/// Supports writing log entries to a <see cref="TextWriter"/>.
	/// </summary>
	public class TextWriterLogWriter : TextLogWriter, IBufferingLogWriter
	{

		private readonly TextWriter _textWriter;
		private readonly bool _disposeWriter;
		private readonly string _newLine;

		/// <summary>
		/// Holds the function that is used to determine whether to flush the buffers or not.
		/// </summary>
		private Func<bool> _flushPredicate;

		/// <summary>
		/// Creates a new <see cref="TextWriterLogWriter"/>.
		/// </summary>
		/// <param name="textWriter">The <see cref="TextWriter"/> to write formatted log output to.</param>
		/// <param name="setupTracerFactory">The <see cref="ITracerFactory"/> to use for logging setup operations.</param>
		/// <param name="synchronize">Set to <c>true</c> if all logging operations should be synchronized.</param>
		/// <param name="disposeWriter">Whether to dispose <paramref name="textWriter"/> when the <c>TextWriterLogWriter</c> is disposed.</param>
		/// <param name="flushPredicate">A function that is used to determine whether to flush the buffers or not.  If <c>null</c>,
		/// a predicate is used to cause buffers to be flushed after every write.</param>
		public TextWriterLogWriter(TextWriter textWriter, ITracerFactory setupTracerFactory, bool synchronize = false, bool disposeWriter = true, Func<bool> flushPredicate = null)
			: base(setupTracerFactory, synchronize)
		{
			Contract.Requires<ArgumentNullException>(textWriter != null);

			_textWriter = textWriter;
			_disposeWriter = disposeWriter;
			_newLine = textWriter.NewLine;

			// Default to "always flush" if not specified.
			_flushPredicate = flushPredicate ?? BufferingLogWriter.AlwaysFlush;
		}

		/// @inheritdoc
		public Func<bool> FlushPredicate
		{
			get { return _flushPredicate; }
			set
			{
				if (IsStarted)
				{
					throw new LogJamSetupException("FlushPredicate cannot be set after logwriter is started.", this);
				}
				_flushPredicate = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(true);
			_textWriter.Flush();
			if (_disposeWriter)
			{
				_textWriter.Dispose();
			}
		}

		internal TextWriter InternalTextWriter { get { return _textWriter; } }

		protected override void WriteFormattedEntry(string formattedEntry)
		{
			if (! IsStarted)
			{
				return;
			}

			bool lockTaken = false;
			bool includeNewLine = ! formattedEntry.EndsWith(_newLine);
			if (synchronize)
			{
				Monitor.Enter(this, ref lockTaken);
			}
			try
			{
				if (includeNewLine)
				{
					_textWriter.WriteLine(formattedEntry);
				}
				else
				{
					_textWriter.Write(formattedEntry);
				}
				if (_flushPredicate())
				{
					_textWriter.Flush();
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(this);
				}
			}
		}

	}

}