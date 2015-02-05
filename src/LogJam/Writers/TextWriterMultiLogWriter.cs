// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterMultiLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writers
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;


	/// <summary>
	/// Supports writing multiple log entry types to a <see cref="TextWriter"/>.
	/// </summary>
	public class TextWriterMultiLogWriter : IMultiLogWriter, IStartable
	{

		private bool _disposed;
		private readonly TextWriter _writer;
		private readonly bool _isSynchronized;

		/// <summary>
		/// Creates a new <see cref="TextWriterMultiLogWriter"/>.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/> to write formatted log output to.</param>
		/// <param name="synchronize">Set to <c>true</c> if all logging operations should be synchronized.</param>
		public TextWriterMultiLogWriter(TextWriter writer, bool synchronize)
		{
			Contract.Requires<ArgumentNullException>(writer != null);

			_disposed = false;
			_writer = writer;
			_isSynchronized = synchronize;
		}

		#region IMultiLogWriter

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public bool Enabled { get { throw new NotImplementedException(); } }

		public bool IsSynchronized { get { throw new NotImplementedException(); } }

		public bool GetLogWriter<TEntry>(out ILogWriter<TEntry> logWriter) where TEntry : ILogEntry
		{
			throw new NotImplementedException();
		}

		#endregion
		#region IStartable

		public void Start()
		{
			throw new NotImplementedException();
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}

		public bool IsStarted { get { throw new NotImplementedException(); } }

		#endregion
	}

}