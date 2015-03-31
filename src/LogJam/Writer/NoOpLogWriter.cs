﻿// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{

	using LogJam.Trace;


	/// <summary>
	/// An <see cref="ILogWriter"/> that returns entry writers that do nothing.
	/// </summary>
	public sealed class NoOpLogWriter : BaseLogWriter
	{

		public NoOpLogWriter(ITracerFactory setupTracerFactory)
			: base(setupTracerFactory)
		{}

		public override bool IsSynchronized { get { return true; } }

		public override bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter)
		{
			if (! base.TryGetEntryWriter(out entryWriter))
			{
				entryWriter = new NoOpEntryWriter<TEntry>();
				base.AddEntryWriter(entryWriter);
			}
			return true;
		}

	}

}