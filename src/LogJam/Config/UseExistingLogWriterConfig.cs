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


	/// <summary>
	/// An <see cref="ILogWriterConfig"/> that always returns the <see cref="ILogWriter"/> that is passed in.
	/// Supports creating an <see cref="ILogWriter"/>, then passing that into configuration methods.  Note that once
	/// the <see cref="ILogWriter"/> is disposed, it can't be created or used again.
	/// </summary>
	/// <seealso cref="UseExistingLogWriterConfig{TEntry}">Similar but holds strongly-typed <seealso cref="ILogWriter{TEntry}"/> instances.</seealso>
	public class UseExistingLogWriterConfig : ILogWriterConfig
	{

		private readonly ILogWriter _logWriter;

		/// <summary>
		/// Creates a new <see cref="UseExistingLogWriterConfig{TEntry}"/> instance, which will result in
		/// log entries being written to <paramref name="logWriter"/>.
		/// </summary>
		/// <param name="logWriter"></param>
		public UseExistingLogWriterConfig(ILogWriter logWriter)
		{
			Contract.Requires<ArgumentNullException>(logWriter != null);

			_logWriter = logWriter;
		}

		public bool Synchronized
		{
			get { return _logWriter.IsSynchronized; }
			set { throw new NotImplementedException("Can't set the synchronization of an existing ILogWriter."); }
		}

		public ILogWriter CreateILogWriter()
		{
			return _logWriter;
		}

		public bool Equals(ILogWriterConfig other)
		{
			var otherSameType = other as UseExistingLogWriterConfig;
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