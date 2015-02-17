// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionThrowingLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Common
{
	using System;

	using LogJam.Writer;


	/// <summary>
	/// A <see cref="ILogWriter{TEntry}"/> that throws exceptions when writing and disposing.
	/// </summary>
	internal class ExceptionThrowingLogWriter<TEntry> : ILogWriter<TEntry>, IDisposable where TEntry : ILogEntry
	{

		public int CountExceptionsThrown = 0;

		public bool Enabled { get { return true; } }
		public bool IsSynchronized { get { return true; } }

		public void Write(ref TEntry entry)
		{
			CountExceptionsThrown++;
			throw new ApplicationException();
		}

		public void Dispose()
		{
			CountExceptionsThrown++;
			throw new ApplicationException();
		}

	}

}