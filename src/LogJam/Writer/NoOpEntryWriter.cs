// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpEntryWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Writer
{
	using System;
	using System.Diagnostics.Contracts;


	/// <summary>
	/// An <see cref="IEntryWriter{TEntry}"/> that does nothing.
	/// </summary>
	public sealed class NoOpEntryWriter<TEntry> : IEntryWriter<TEntry> where TEntry : ILogEntry
	{

		public NoOpEntryWriter()
		{}

		public bool IsEnabled { get { return false; } }

		public Type LogEntryType { get { return typeof(TEntry); } }

		public void Write(ref TEntry entry)
		{}

	}

}
