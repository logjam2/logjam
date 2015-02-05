// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogDecoder.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using System.IO;


	/// <summary>
	/// Parses log entries from an input byte stream.
	/// </summary>
	public interface ILogDecoder<out TEntry>
	{
		/// <summary>
		/// Parses a <typeparam name="TEntry"/> from <paramref name="stream"/> and returns it.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns>The next <typeparam name="TEntry"/> that can be read from <paramref name="stream"/></returns>
		TEntry DecodeEntry(Stream stream);
	}

}