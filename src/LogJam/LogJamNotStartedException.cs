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
	using System.Diagnostics.Contracts;


	/// <summary>
	/// Thrown when a LogJam manager has not been started.
	/// </summary>
	public sealed class LogJamNotStartedException : Exception
	{

		private readonly object _notStartedInstance;

		internal LogJamNotStartedException(object notStartedInstance)
		{
			Contract.Requires<ArgumentNullException>(notStartedInstance != null);

			_notStartedInstance = notStartedInstance;
		}

		/// <summary>
		/// The LogJam object that was not started.
		/// </summary>
		public object NotStartedInstance
		{
			get { return _notStartedInstance; }
		}

		public override string Message { get { return NotStartedInstance + " must be Start()ed."; } }

	}

}