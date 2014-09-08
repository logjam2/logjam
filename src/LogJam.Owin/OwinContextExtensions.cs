// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinContextExtensions.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace Microsoft.Owin
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Owin;


	/// <summary>
	/// Extension methods for <see cref="IOwinContext"/>.
	/// </summary>
	public static class OwinContextExtensions
	{

		private const string c_lastLoggedExceptionKey = "LogJam.Owin.LastLoggedException";

		/// <summary>
		/// Returns the request number (ordinal) for the request described by <paramref name="owinContext"/>.  For each
		/// Owin app, this number starts at 1 upon initialization.
		/// </summary>
		/// <param name="owinContext">An <see cref="IOwinContext"/> for the current request.</param>
		/// <returns>The request number for the current OWIN request.</returns>
		public static long GetRequestNumber(this IOwinContext owinContext)
		{
			Contract.Requires<ArgumentNullException>(owinContext != null);

			return owinContext.Get<long>(HttpLoggingMiddleware.RequestNumberKey);
		}

		/// <summary>
		/// Stores <paramref name="exception"/> in the owin context dictionary, to support preventing duplicate
		/// logging of the exception.
		/// </summary>
		/// <param name="owinContext">An <see cref="IOwinContext"/> for the current request.</param>
		/// <param name="exception">An exception to store.  If <c>null</c>, the stored exception is cleared.</param>
		public static void LoggedRequestException(this IOwinContext owinContext, Exception exception)
		{
			Contract.Requires<ArgumentNullException>(owinContext != null);

			owinContext.Environment[c_lastLoggedExceptionKey] = exception;
		}

		/// <summary>
		/// Returns <c>true</c> if <see cref="LoggedRequestException"/> was previously called for the same request,
		/// and same exception.  Reference comparison is used to match the exception.  This method is used to
		/// prevent duplicate logging of the exception.
		/// </summary>
		/// <param name="owinContext">An <see cref="IOwinContext"/> for the current request.</param>
		/// <param name="exception">An exception to compare to a stored exception.  May not be <c>null</c>.</param>
		/// <returns><c>true</c> if <paramref name="exception"/> has already been logged for this request.</returns>
		public static bool HasRequestExceptionBeenLogged(this IOwinContext owinContext, Exception exception)
		{
			Contract.Requires<ArgumentNullException>(owinContext != null);
			Contract.Requires<ArgumentNullException>(exception != null);

			var lastLoggedException = owinContext.Get<Exception>(c_lastLoggedExceptionKey);
			return ReferenceEquals(exception, lastLoggedException);
		}

	}

}
