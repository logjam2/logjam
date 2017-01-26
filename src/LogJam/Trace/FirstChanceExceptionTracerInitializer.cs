// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirstChanceExceptionTracerInitializer.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    using System;
    using System.Runtime.ExceptionServices;

    using LogJam.Trace.Config;
    using LogJam.Trace.Config.Initializer;

    /// <summary>
    /// Implements tracing of first chance exceptions, if enabled via <see cref="TraceManagerConfig.TraceFirstChanceExceptions"/>.
    /// </summary>
    internal sealed class FirstChanceExceptionTracerInitializer : ITraceInitializer
    {

        public void OnStarting(TraceManager traceManager)
        {
            if (traceManager.Config.TraceFirstChanceExceptions)
            {
                var firstChanceTracer = new FirstChanceExceptionTracer(traceManager);
                traceManager.DisposeOnStop(firstChanceTracer);
            }
        }

        /// <summary>
        /// One instance of this should be alive for each started <see cref="TraceManager"/> with
        /// <see cref="TraceManagerConfig.TraceFirstChanceExceptions"/> set to <c>true</c>.
        /// </summary>
        private class FirstChanceExceptionTracer : IDisposable
        {

            private readonly ITracerFactory _tracerFactory;

            private bool _inEventHandler;

            public FirstChanceExceptionTracer(ITracerFactory tracerFactory)
            {
                _tracerFactory = tracerFactory;
                AppDomain.CurrentDomain.FirstChanceException += TraceFirstChanceException;
            }

            private void TraceFirstChanceException(object sender, FirstChanceExceptionEventArgs exceptionEventArgs)
            {
                // Prevent reentrancy
                if (_inEventHandler)
                {
                    return;
                }

                try
                {
                    _inEventHandler = true;
                    _tracerFactory.GetTracer(exceptionEventArgs.Exception.TargetSite.ReflectedType)
                        .Warn(exceptionEventArgs.Exception, "Exception thrown:");
                }
                finally
                {
                    _inEventHandler = false;
                }
            }

            public void Dispose()
            {
                AppDomain.CurrentDomain.FirstChanceException -= TraceFirstChanceException;
            }

        }

    }

}