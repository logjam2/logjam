// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITracerFactory.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    using System;


    /// <summary>
    /// Controls configuration and creation/caching of <see cref="Tracer" />s.
    /// </summary>
    public interface ITracerFactory : IDisposable
    {

        /// <summary>
        /// Returns a <see cref="Tracer" /> for tracing messages associated with <paramref name="name" />.
        /// </summary>
        /// <param name="name">
        /// A name for the tracer, often the class name or namespace containing the logic being traced.  Can
        /// also be a category.
        /// </param>
        /// <returns>A <see cref="Tracer" />.</returns>
        Tracer GetTracer(string name);

    }

}
