// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writers
{
	using System;
	using System.IO;

	using LogJam.Config;
	using LogJam.Format;
	using LogJam.Util;


	/// <summary>
	/// Log writer configuration that creates a <see cref="TextWriterLogWriter{TEntry}"/> using the specified <see cref="LogFormatter{TEntry}"/>.
	/// </summary>
	public abstract class TextWriterLogWriterConfig<TEntry> : LogWriterConfig<TEntry> where TEntry : ILogEntry
	{

		/// <summary>
		/// Creates a new <see cref="TextWriterLogWriterConfig{TEntry}"/>.  The <see cref="Formatter"/> property must be set.
		/// </summary>
		protected TextWriterLogWriterConfig()
		{}

		/// <summary>
		/// Creates a new <see cref="TextWriterLogWriterConfig{TEntry}"/> using the specified <paramref name="formatter"/>.
		/// </summary>
		/// <param name="formatter"></param>
		protected TextWriterLogWriterConfig(LogFormatter<TEntry> formatter)
		{
			Formatter = formatter;
		}

		/// <summary>
		/// Gets or sets the <see cref="LogFormatter{TEntry}"/> used to format <typeparamref name="TEntry"/> instances.
		/// </summary>
		public LogFormatter<TEntry> Formatter { get; set; }

		/// <summary>
		/// Creates or otherwise returns the <see cref="TextWriter"/> that formatted log output will be written to.
		/// </summary>
		/// <returns></returns>
		protected abstract TextWriter CreateTextWriter();

		public override ILogWriter<TEntry> CreateLogWriter()
		{
			if (Formatter == null)
			{
				throw new InvalidOperationException("LogFormatter<TEntry> must be set");
			}

			return new TextWriterLogWriter<TEntry>(CreateTextWriter(), Formatter);
		}

		public override bool Equals(ILogWriterConfig other)
		{
			if (! base.Equals(other))
			{
				return false;
			}
			var otherSameType = other as TextWriterLogWriterConfig<TEntry>;
			return Equals(Formatter, otherSameType.Formatter);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ Formatter.Hash(3);
		}

	}

}