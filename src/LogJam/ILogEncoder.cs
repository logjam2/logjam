// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogFormatter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using System.IO;


	/// <summary>
	/// Encodes log entries for an output byte stream.
	/// </summary>
	public interface ILogEncoder<in TEntry>
	{

		void EncodeEntry(TEntry entry, Stream stream);

	}

}