// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogFormatter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam
{
	using System;
	using System.IO;

	using LogJam.Trace;


	/// <summary>
	/// ILogFormatter objects format a text representation of <typeparamref name="TEntry"/> objects.
	/// </summary>
	public abstract class LogFormatter<TEntry>
	{
		#region Public Methods and Operators

		public abstract void Format(ref TEntry entry, TextWriter textWriter);

		public virtual string Format(ref TEntry entry)
		{
			StringWriter sw = new StringWriter();
			Format(ref entry, sw);
			return sw.ToString();
		}

		#endregion
	}
}
