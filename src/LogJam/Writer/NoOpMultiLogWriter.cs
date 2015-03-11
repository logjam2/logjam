// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoOpMultiLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	using LogJam.Trace;


	/// <summary>
	/// A <see cref="IMultiLogWriter"/> that returns log writers that do nothing.
	/// </summary>
	public sealed class NoOpMultiLogWriter : MultiLogWriter
	{

		public NoOpMultiLogWriter(bool isSychronized, ITracerFactory setupTracerFactory)
			: base(isSychronized, setupTracerFactory)
		{}

	}

}