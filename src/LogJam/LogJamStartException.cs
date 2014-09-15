// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamStartException.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using System;


	/// <summary>
	/// Thrown when LogJam could not be successfully started.
	/// </summary>
	public sealed class LogJamStartException : Exception
	{

		internal LogJamStartException(string message, Exception innerException)
			: base(message, innerException)
		{}

	}

}