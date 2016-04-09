// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using LogJam.Config;

    using Microsoft.Owin;

    using Owin;


    /// <summary>
    /// Extension methods for <see cref="HttpRequestMessage" />.
    /// </summary>
    public static class RequestExtensions
    {

        private const string c_lastLoggedExceptionKey = "LogJam.WebApi.LastLoggedException";

        /// <summary>
        /// Returns the request number (ordinal) for the request described by <paramref name="webApiRequest" />.  This
        /// method will return monotonically increasing request numbers if OWIN request logging is enabled
        /// via <see cref="AppBuilderExtensions.LogHttpRequests(IAppBuilder, IEnumerable{ILogWriterConfig})" />.
        /// </summary>
        /// <param name="webApiRequest">An <see cref="HttpRequestMessage" /> for the current request.</param>
        /// <returns>The request number for the current OWIN request.</returns>
        public static long GetRequestNumber(this HttpRequestMessage webApiRequest)
        {
            Contract.Requires<ArgumentNullException>(webApiRequest != null);

            IOwinContext owinContext = webApiRequest.GetOwinContext();
            if (owinContext == null)
            {
                return 0;
            }
            else
            {
                return owinContext.GetRequestNumber();
            }
        }

        /// <summary>
        /// Stores <paramref name="exception" /> in the owin context dictionary, to support preventing duplicate
        /// logging of the exception.
        /// </summary>
        /// <param name="webApiRequest">An <see cref="HttpRequestMessage" /> for the current request.</param>
        /// <param name="exception">An exception to store.  If <c>null</c>, the stored exception is cleared.</param>
        public static void LoggedRequestException(this HttpRequestMessage webApiRequest, Exception exception)
        {
            Contract.Requires<ArgumentNullException>(webApiRequest != null);

            IOwinContext owinContext = webApiRequest.GetOwinContext();
            if (owinContext != null)
            {
                owinContext.LoggedRequestException(exception);
            }
            else
            {
                // If no OWIN, use the Web API request dictionary
                webApiRequest.Properties[c_lastLoggedExceptionKey] = exception;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if <see cref="LoggedRequestException" /> was previously called for the same request,
        /// and same exception.  Reference comparison is used to match the exception.  This method is used to
        /// prevent duplicate logging of the exception.
        /// </summary>
        /// <param name="webApiRequest">An <see cref="HttpRequestMessage" /> for the current request.</param>
        /// <param name="exception">An exception to compare to a stored exception.  May not be <c>null</c>.</param>
        /// <returns><c>true</c> if <paramref name="exception" /> has already been logged for this request.</returns>
        public static bool HasRequestExceptionBeenLogged(this HttpRequestMessage webApiRequest, Exception exception)
        {
            Contract.Requires<ArgumentNullException>(webApiRequest != null);
            Contract.Requires<ArgumentNullException>(exception != null);

            IOwinContext owinContext = webApiRequest.GetOwinContext();
            if (owinContext != null)
            {
                return owinContext.HasRequestExceptionBeenLogged(exception);
            }
            else
            {
                // If no OWIN, use the Web API request dictionary
                object previousException;
                webApiRequest.Properties.TryGetValue(c_lastLoggedExceptionKey, out previousException);
                return ReferenceEquals(exception, previousException as Exception);
            }
        }

    }

}
