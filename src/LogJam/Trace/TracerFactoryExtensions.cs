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

        public static Tracer GetTracer(this ITracerFactory tracerFactory, Type type)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));
            Arg.NotNull(type, nameof(type));

            // ? Convert generic types to their generic type definition - so the same
            // Tracer is used for ArrayList<T> regardless of the type parameter T.
            //if (type.IsGenericType)
            //{
            //	type = type.GetGenericTypeDefinition();
            //}

            return tracerFactory.GetTracer(type.GetCSharpName());
        }

        public static Tracer TracerFor(this ITracerFactory tracerFactory, object traceSource)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));
            Arg.NotNull(traceSource, nameof(traceSource));

            // Handle the case where Type is passed in, when an object was expected
            Type traceSourceType = traceSource as Type;
            if (traceSourceType != null)
            {
                return GetTracer(tracerFactory, traceSourceType);
            }

            return tracerFactory.GetTracer(traceSource.GetType());
        }

        public static Tracer TracerFor<T>(this ITracerFactory tracerFactory)
        {
            Arg.NotNull(tracerFactory, nameof(tracerFactory));

            return tracerFactory.GetTracer(typeof(T));
        }

    }

}
