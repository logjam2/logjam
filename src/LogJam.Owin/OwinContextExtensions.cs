﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinContextExtensions.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Owin
{
	using LogJam.Owin;


	/// <summary>
	/// Extension methods for <see cref="IOwinContext"/>.
	/// </summary>
	public static class OwinContextExtensions
	{

		public static long GetRequestNumber(this IOwinContext owinContext)
		{
			return owinContext.Get<long>(HttpLoggingMiddleware.RequestNumberKey);
		}

	}

}
