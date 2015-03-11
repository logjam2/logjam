// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Writer
{

	/// <summary>
	/// An <see cref="ILogWriter{TEntry}"/> that does nothing.
	/// </summary>
	public sealed class NoOpLogWriter<TEntry> : ILogWriter<TEntry> where TEntry : ILogEntry
	{

		public bool Enabled { get { return false; } }

		public bool IsSynchronized { get { return true; } }

		public void Write(ref TEntry entry)
		{}

	}

}
