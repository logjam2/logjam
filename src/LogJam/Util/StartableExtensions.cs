// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartableExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{
    using System;
    using System.Collections;
    using System.Diagnostics.Contracts;

    using LogJam.Trace;


    /// <summary>
    /// Helper methods for <see cref="IStartable" />.
    /// </summary>
    internal static class StartableExtensions
    {

        internal static void SafeStart(this IStartable startable, Tracer tracer)
        {
            Contract.Requires<ArgumentNullException>(tracer != null);

            if (startable == null)
            {
                return;
            }

            if (startable.IsStarted)
            {
                tracer.Info("{0} already started, not restarting it.", startable);
                return;
            }

            try
            {
                tracer.Verbose("Starting {0} ...", startable);
                startable.Start();
                tracer.Info("Successfully started {0}.", startable);
            }
            catch (Exception excp)
            {
                tracer.Severe(excp, "Exception Start()ing {0}", startable);
            }
        }

        internal static void SafeStart(this IStartable startable, ITracerFactory tracerFactory)
        {
            Contract.Requires<ArgumentNullException>(tracerFactory != null);

            if (startable == null)
            {
                return;
            }

            Tracer tracer = tracerFactory.TracerFor(startable);
            SafeStart(startable, tracer);
        }

        internal static void SafeStart(this IEnumerable collection, ITracerFactory tracerFactory)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(tracerFactory != null);

            foreach (object o in collection)
            {
                SafeStart(o as IStartable, tracerFactory);
            }
        }

        internal static void SafeStop(this IStartable startable, Tracer tracer)
        {
            Contract.Requires<ArgumentNullException>(tracer != null);

            if (startable == null)
            {
                return;
            }

            if (startable.IsStarted)
            {
                try
                {
                    tracer.Verbose("Stopping {0} ...", startable);
                    startable.Stop();
                    tracer.Info("Successfully stopped {0}.", startable);
                }
                catch (Exception excp)
                {
                    tracer.Severe(excp, "Exception Stop()ing {0}", startable);
                }
            }
        }

        internal static void SafeStop(this IStartable startable, ITracerFactory tracerFactory)
        {
            Contract.Requires<ArgumentNullException>(tracerFactory != null);

            if (startable == null)
            {
                return;
            }

            Tracer tracer = tracerFactory.TracerFor(startable);
            SafeStop(startable, tracer);
        }

        internal static void SafeStop(this IEnumerable collection, ITracerFactory tracerFactory)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(tracerFactory != null);

            foreach (object o in collection)
            {
                SafeStop(o as IStartable, tracerFactory);
            }
        }

        internal static void SafeDispose(this IDisposable disposable, Tracer tracer)
        {
            Contract.Requires<ArgumentNullException>(tracer != null);

            if (disposable == null)
            {
                return;
            }

            try
            {
                tracer.Debug("Disposing {0} ...", disposable);
                disposable.Dispose();
                tracer.Verbose("Disposed {0}.", disposable);
            }
            catch (Exception excp)
            {
                tracer.Severe(excp, "Exception Dispose()ing {0}", disposable);
            }
        }

        internal static void SafeDispose(this IDisposable disposable, ITracerFactory tracerFactory)
        {
            Contract.Requires<ArgumentNullException>(tracerFactory != null);

            if (disposable == null)
            {
                return;
            }

            Tracer tracer = tracerFactory.TracerFor(disposable);
            disposable.SafeDispose(tracer);
        }

        internal static void SafeDispose(this IEnumerable collection, ITracerFactory tracerFactory)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Requires<ArgumentNullException>(tracerFactory != null);

            foreach (object o in collection)
            {
                SafeDispose(o as IDisposable, tracerFactory);
            }
        }

    }

}
