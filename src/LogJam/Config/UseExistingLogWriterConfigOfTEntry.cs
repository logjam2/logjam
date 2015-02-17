// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseExistingLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Writer;


	/// <summary>
	/// A <see cref="LogWriterConfig{TEntry}"/> that always returns the <see cref="ILogWriter{TEntry}"/> that is passed in.
	/// Supports creating a <see cref="ILogWriter{TEntry}"/>, then passing that into configuration methods.
	/// </summary>
	public sealed class UseExistingLogWriterConfig<TEntry> : LogWriterConfig<TEntry> where TEntry : ILogEntry
	{

		private readonly ILogWriter<TEntry> _logWriter;

		/// <summary>
		/// Creates a new <see cref="UseExistingLogWriterConfig{TEntry}"/> instance, which will result in
		/// log entries being written to <paramref name="logWriter"/>.
		/// </summary>
		/// <param name="logWriter"></param>
		public UseExistingLogWriterConfig(ILogWriter<TEntry> logWriter)
		{
			Contract.Requires<ArgumentNullException>(logWriter != null);

			_logWriter = logWriter;
		}

		public override ILogWriter<TEntry> CreateLogWriter()
		{	
			return _logWriter;
		}

		public override bool Equals(ILogWriterConfig other)
		{
			var otherSameType = other as UseExistingLogWriterConfig<TEntry>;
			if (otherSameType == null)
			{
				return false;
			}

			return ReferenceEquals(_logWriter, otherSameType._logWriter);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ _logWriter.GetHashCode();
		}

	}

}