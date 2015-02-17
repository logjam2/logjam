// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseExistingTextWriterConfig.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;

	using LogJam.Format;
	using LogJam.Writer;


	/// <summary>
	/// A <see cref="TextWriterLogWriterConfig{TEntry}"/> that uses a passed in <see cref="TextWriter"/> and <see cref="LogFormatter{TEntry}"/>,
	/// and creates a <see cref="TextWriterLogWriter{TEntry}"/> that logs using both.
	/// </summary>
	/// <remarks>
	/// When the <see cref="TextWriterLogWriter{TEntry}"/> is disposed, the pass
	/// One weakness of this option is that, after the <see cref="TextWriterLogWriter{TEntry}"/> that contains the is stopped/disposed, the TextWriter cannot be re-opened.
	/// </remarks>
	public sealed class UseExistingTextWriterConfig<TEntry> : TextWriterLogWriterConfig<TEntry>
		where TEntry : ILogEntry
	{

		private TextWriter _textWriter;
		private bool _textWriterHasBeenUsed;

		public UseExistingTextWriterConfig(TextWriter textWriter, LogFormatter<TEntry> logFormatter = null)
		{
			Contract.Requires<ArgumentNullException>(textWriter != null);

			TextWriter = textWriter;
			Formatter = logFormatter;
		}

		public UseExistingTextWriterConfig(TextWriter textWriter, FormatAction<TEntry> formatAction)
			: this(textWriter, (LogFormatter<TEntry>) formatAction)
		{
			Contract.Requires<ArgumentNullException>(textWriter != null);
			Contract.Requires<ArgumentNullException>(formatAction != null);
		}

		public UseExistingTextWriterConfig()
		{}

		/// <summary>
		/// Gets or sets the <see cref="TextWriter"/> that is used when logging is started.
		/// </summary>
		public TextWriter TextWriter
		{
			get { return _textWriter; }
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);

				_textWriter = value;
				_textWriterHasBeenUsed = false;				
			}
		}

		protected override TextWriter CreateTextWriter()
		{
			if (_textWriterHasBeenUsed)
			{
				throw new LogJamStartException("UseExistingTextWriterConfig cannot reuse the TextWriter - it has probably been disposed.", _textWriter);
			}

			_textWriterHasBeenUsed = true;
			return _textWriter;
		}

	}

}