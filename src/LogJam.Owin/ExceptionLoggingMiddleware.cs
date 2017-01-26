// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionLoggingMiddleware.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;

    using LogJam.Trace;

    using Microsoft.Owin;


    /// <summary>
    /// Middleware that logs exceptions that are passed up the Owin stack.
    /// </summary>
    internal sealed class ExceptionLoggingMiddleware : OwinMiddleware
    {

        private readonly Tracer _tracer;
        private readonly Func<IOwinContext, string> _messageFormatter;

        private static readonly Boolean s_isMono = Type.GetType("Mono.Runtime") != null;

        public ExceptionLoggingMiddleware(OwinMiddleware next,
                                          Tracer tracer,
                                          Func<IOwinContext, string> messageFormatter = null)
            : base(next)
        {
            Contract.Requires<ArgumentNullException>(tracer != null);

            _tracer = tracer;
            _messageFormatter = messageFormatter ?? DefaultMessageFormatter;
        }

        public string DefaultMessageFormatter(IOwinContext context)
        {
            var request = context.Request;
            return string.Format("Unhandled exception for {0} {1} User={2} RemoteIP={3}", request.Method, request.Uri, request.User, request.RemoteIpAddress);
        }

        public override async Task Invoke(IOwinContext owinContext)
        {
            try
            {
                await Next.Invoke(owinContext);
            }
            catch (Exception ex)
            {
               TraceUnhandledException(owinContext, ex);
               throw;
            }
        }

        private void TraceUnhandledException(IOwinContext owinContext, Exception exception)
        {
            if (_tracer.IsErrorEnabled())
            {
                _tracer.Error(exception, _messageFormatter(owinContext));
            }
        }

    }

}
