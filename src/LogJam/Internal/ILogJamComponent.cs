// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogJamComponent.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal
{
	using LogJam.Trace;


	/// <summary>
	/// Internal LogJam component accessors to simplify code.
	/// </summary>
	internal interface ILogJamComponent
	{

		ITracerFactory SetupTracerFactory { get; }

	}

}