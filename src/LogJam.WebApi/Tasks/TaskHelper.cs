// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.WebApi.Tasks
{
	using System.Threading.Tasks;


	/// <summary>
	/// Task/TPL helper functions.
	/// </summary>
	internal static class TaskHelper
	{

		private static readonly Task s_completed = Task.FromResult(true);

		public static Task Completed { get { return s_completed; } }

	}

}
