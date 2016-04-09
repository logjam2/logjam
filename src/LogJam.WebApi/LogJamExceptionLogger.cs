// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamExceptionLogger.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.WebApi
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.ExceptionHandling;

    using LogJam.Trace;
    using LogJam.WebApi.Tasks;


    /// <summary>
    /// Logs Web API exceptions to a LogJam <see cref="Tracer" />.
    /// </summary>
    public sealed class LogJamExceptionLogger : IExceptionLogger
    {

        private readonly Tracer _tracer;

        /// <summary>
        /// Creates a new <see cref="LogJamExceptionLogger" />, which logs Web API exceptions to a <see cref="Tracer" /> obtained
        /// from
        /// <paramref name="tracerFactory" />.
        /// </summary>
        /// <param name="tracerFactory">A <see cref="ITracerFactory" />.</param>
        public LogJamExceptionLogger(ITracerFactory tracerFactory)
        {
            Contract.Requires<ArgumentNullException>(tracerFactory != null);

            _tracer = tracerFactory.TracerFor(this);
        }

        /// <summary>
        /// Logs an unhandled Web API exception.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous exception logging operation.
        /// </returns>
        /// <param name="context">The exception logger context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var exception = context.Exception;

            if (request.HasRequestExceptionBeenLogged(exception))
            {
                // Don't repeatedly log the same exception
                _tracer.Error("Exception thrown handling Web API request: {0} {1}\n    (exception already logged)",
                              request.Method,
                              request.RequestUri.OriginalString);
            }
            else
            {
                // exception hasn't previously been logged
                _tracer.Error(exception,
                              "Exception thrown handling Web API request: {0} {1}",
                              request.Method,
                              request.RequestUri.OriginalString);
                request.LoggedRequestException(exception);
            }

            return TaskHelper.Completed;
        }

    }

}
