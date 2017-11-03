// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TracerFactoryExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    using System;

    using LogJam.Shared.Internal;
    using LogJam.Util;


    /// <summary>
    /// Extension methods for <see cref="ITracerFactory" />.
    /// </summary>
    public static class TracerFactoryExtensions
    {

        /// <summary>
        /// Returns a <see cref="Tracer"/> for <paramref name="type"/>.
        /// </summary>
        /// <param name="tracerFactory"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Obsolete("Use ITracerFactory.GetTracer(Type) instead.")]
        public static Tracer GetTracer(this ITracerFactory tracerFactory, Type type)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));
            Arg.NotNull(type, nameof(type));

            return tracerFactory.GetTracer(type);
        }

        /// <summary>
        /// Returns a <see cref="Tracer"/> for object <paramref name="traceSource"/>.
        /// </summary>
        /// <param name="tracerFactory"></param>
        /// <param name="traceSource"></param>
        /// <returns></returns>
        public static Tracer TracerFor(this ITracerFactory tracerFactory, object traceSource)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));
            Arg.NotNull(traceSource, nameof(traceSource));

            // Handle the case where Type is passed in, when an object was expected
            if (traceSource is Type traceSourceType)
            {
                return tracerFactory.GetTracer(traceSourceType);
            }

            return tracerFactory.GetTracer(traceSource.GetType());
        }

        /// <summary>
        /// Returns a <see cref="Tracer"/> for type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tracerFactory"></param>
        /// <returns></returns>
        public static Tracer TracerFor<T>(this ITracerFactory tracerFactory)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));

            return tracerFactory.GetTracer(typeof(T));
        }

    }

}
