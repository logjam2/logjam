// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamSetupException.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using System;


	/// <summary>
	/// Signifies an error in a setup operation.
	/// </summary>
	public class LogJamSetupException : LogJamException
	{

		internal LogJamSetupException(string message, object source)
			: base(message, source)
		{}

		internal LogJamSetupException(string message, Exception innerException, object source)
			: base(message, innerException, source)
		{}

	}

}