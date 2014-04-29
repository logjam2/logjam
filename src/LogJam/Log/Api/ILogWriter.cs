// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace.Log.Api
{
	/// <summary>
	/// The LogWriter interface.
	/// </summary>
	/// <typeparam name="TEntry">
	/// </typeparam>
	public interface ILogWriter<in TEntry>
	{
		#region Public Methods and Operators

		/// <summary>
		/// The log.
		/// </summary>
		/// <param name="entry">
		/// The entry.
		/// </param>
		void Write(TEntry entry);

		#endregion
	}
}
