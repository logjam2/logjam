// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinLogger.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Diagnostics;

using LogJam.Shared.Internal;
using LogJam.Trace;

using Microsoft.Owin.Logging;

using TraceLevel = LogJam.Trace.TraceLevel;

namespace LogJam.Owin
{

    /// <summary>
    /// Provides an <see cref="ILogger" /> implementation using <see cref="LogJam.Trace.Tracer" />.
    /// </summary>
    internal class OwinLogger : ILogger
    {

        private readonly Tracer _tracer;

        public OwinLogger(Tracer tracer)
        {
            _tracer = tracer;
        }

        public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            TraceLevel traceLevel = TraceEventTypeToTraceLevel(eventType);
            if (! _tracer.IsTraceEnabled(traceLevel))
            {
                return false;
            }

            string message = state as string;
            if ((message != null) || (exception != null))
            {
                _tracer.Trace(traceLevel, exception, message);
            }

            return true;
        }

        private static TraceLevel TraceEventTypeToTraceLevel(TraceEventType tet)
        {
            switch (tet)
            {
                case TraceEventType.Critical:
                    return TraceLevel.Severe;
                case TraceEventType.Error:
                    return TraceLevel.Error;
                case TraceEventType.Warning:
                    return TraceLevel.Warn;
                case TraceEventType.Information:
                    return TraceLevel.Info;
                case TraceEventType.Verbose:
                    return TraceLevel.Verbose;
                default:
                    return TraceLevel.Off;
            }
        }

    }


    internal class OwinLoggerFactory : ILoggerFactory
    {

        private readonly ITracerFactory _tracerFactory;

        public OwinLoggerFactory(ITracerFactory tracerFactory)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));

            _tracerFactory = tracerFactory;
        }

        public ILogger Create(string name)
        {
            return new OwinLogger(_tracerFactory.GetTracer(name));
        }

    }

}
