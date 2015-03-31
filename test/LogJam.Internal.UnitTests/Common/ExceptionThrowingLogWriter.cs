// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionThrowingLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


// ReSharper disable once CheckNamespace
namespace LogJam.UnitTests.Common
{
	using System;

	using LogJam.Trace;
	using LogJam.Writer;


	/// <summary>
	/// A <see cref="IEntryWriter{TEntry}"/> that throws exceptions when writing and disposing.
	/// </summary>
	public class ExceptionThrowingLogWriter<TEntry> : SingleEntryTypeLogWriter<TEntry>, IDisposable where TEntry : ILogEntry
	{

		public int CountExceptionsThrown = 0;

		public ExceptionThrowingLogWriter(ITracerFactory setupTracerFactory)
			: base(setupTracerFactory)
		{}

		public override bool IsSynchronized { get { return true; } }

		public override void Write(ref TEntry entry)
		{
			CountExceptionsThrown++;
			throw new ApplicationException();
		}

		protected override void Dispose(bool isDisposing)
		{
			CountExceptionsThrown++;
			throw new ApplicationException();
		}

	}

}