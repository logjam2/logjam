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

    using LogJam.Shared.Internal;
    using LogJam.Trace;


    /// <summary>
    /// Helper methods for <see cref="IStartable" />.
    /// </summary>
    internal static class StartableExtensions
    {

        /// <summary>
        /// Starts <paramref name="startable"/>, reporting any errors to <paramref cref="tracer"/>.
        /// </summary>
        /// <param name="startable"></param>
        /// <param name="tracer"></param>
        /// <returns><c>true</c> if <paramref name="startable"/> did not fail starting.</returns>
        internal static bool SafeStart(this IStartable startable, Tracer tracer)
        {
            Arg.DebugNotNull(tracer, nameof(tracer));

            if (startable == null)
            {
                return true;
            }

            var state = startable.State;

            if (state == StartableState.Started)
            {
                return true;
            }
            if (state == StartableState.Starting)
            {
                tracer.Debug("{0} already starting, not restarting it.", startable);
                // Return to avoid re-entrant start tracking
                return true;
            }

            try
            {
                tracer.Verbose("Starting {0} ...", startable);
                startable.Start();
                if (startable.State == StartableState.Started)
                {
                    tracer.Info("Successfully started {0}.", startable);
                }
                else if (startable.State == StartableState.Starting)
                {
                    tracer.Info("Start in progress for {0}.", startable);
                }
                else
                {
                    tracer.Warn("{0} not started, but no exception thrown.", startable);
                }
                return true;
            }
            catch (Exception excp)
            {
                tracer.Severe(excp, "Exception Start()ing {0}", startable);
                return false;
            }
        }

        /// <summary>
        /// Starts <paramref name="startable"/>, reporting any errors to <paramref cref="tracerFactory"/>.
        /// </summary>
        /// <param name="startable"></param>
        /// <param name="tracerFactory"></param>
        /// <returns><c>true</c> if <paramref name="startable"/> did not fail starting.</returns>
        internal static bool SafeStart(this IStartable startable, ITracerFactory tracerFactory)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));

            if (startable == null)
            {
                return true;
            }

            Tracer tracer = tracerFactory.TracerFor(startable);
            return SafeStart(startable, tracer);
        }

        /// <summary>
        /// Starts all objects in <paramref name="collection"/>, reporting any errors to <paramref cref="tracerFactory"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="tracerFactory"></param>
        /// <returns><c>true</c> if no elements in <see cref="collection"/> failed starting.</returns>
        internal static bool SafeStart(this IEnumerable collection, ITracerFactory tracerFactory)
        {
            Arg.NotNull(collection, nameof(collection));
            Arg.NotNull(tracerFactory, nameof(tracerFactory));

            bool allStarted = true;
            foreach (object o in collection)
            {
                allStarted &= SafeStart(o as IStartable, tracerFactory);
            }
            return allStarted;
        }

        internal static void SafeStop(this IStartable startable, Tracer tracer)
        {
            Arg.NotNull(tracer, nameof(tracer));

            if (startable == null)
            {
                return;
            }

            if (startable.State == StartableState.Started)
            {
                try
                {
                    tracer.Verbose("Stopping {0} ...", startable);
                    startable.Stop();
                    if (startable.State == StartableState.Stopped)
                    {
                        tracer.Info("Successfully stopped {0}.", startable);
                    }
                    else if (startable.State == StartableState.Stopping)
                    {
                        tracer.Info("Stop still in progress for {0}.", startable);
                    }
                    else
                    {
                        tracer.Warn("{0} not stopped, but no exception thrown.", startable);
                    }
                }
                catch (Exception excp)
                {
                    tracer.Severe(excp, "Exception Stop()ing {0}", startable);
                }
            }
        }

        internal static void SafeStop(this IStartable startable, ITracerFactory tracerFactory)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));

            if (startable == null)
            {
                return;
            }

            Tracer tracer = tracerFactory.TracerFor(startable);
            SafeStop(startable, tracer);
        }

        internal static void SafeStop(this IEnumerable collection, ITracerFactory tracerFactory)
        {
            Arg.NotNull(collection, nameof(collection));
            Arg.NotNull(tracerFactory, nameof(tracerFactory));

            foreach (object o in collection)
            {
                SafeStop(o as IStartable, tracerFactory);
            }
        }

        internal static void SafeDispose(this IDisposable disposable, Tracer tracer)
        {
            Arg.NotNull(tracer, nameof(tracer));

            if (disposable == null)
            {
                return;
            }

            try
            {
                tracer.Debug("Disposing {0} ...", disposable);
                disposable.Dispose();
                tracer.Debug("Disposed {0}.", disposable);
            }
            catch (Exception excp)
            {
                tracer.Severe(excp, "Exception Dispose()ing {0}", disposable);
            }
        }

        internal static void SafeDispose(this IDisposable disposable, ITracerFactory tracerFactory)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));

            if (disposable == null)
            {
                return;
            }

            Tracer tracer = tracerFactory.TracerFor(disposable);
            disposable.SafeDispose(tracer);
        }

        internal static void SafeDispose(this IEnumerable collection, ITracerFactory tracerFactory)
        {
            Arg.NotNull(collection, nameof(collection));
            Arg.NotNull(tracerFactory, nameof(tracerFactory));

            foreach (object o in collection)
            {
                SafeDispose(o as IDisposable, tracerFactory);
            }
        }

    }

}
