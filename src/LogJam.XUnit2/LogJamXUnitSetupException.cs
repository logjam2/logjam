// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamXUnitSetupException.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
	using System;


	/// <summary>
	/// Exception thrown when invalid setup is detected.
	/// </summary>
	public sealed class LogJamXUnitSetupException : LogJamSetupException
	{

		internal LogJamXUnitSetupException(string message, object source)
			: base(message, source)
		{}

		internal LogJamXUnitSetupException(string message, Exception innerException, object source)
			: base(message, innerException, source)
		{}

	}

}