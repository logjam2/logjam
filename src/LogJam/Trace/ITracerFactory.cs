// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITracerFactory.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    using System;
    using System.Diagnostics.Contracts;

    using LogJam.Trace.Config;


    /// <summary>
    /// Controls configuration and creation/caching of <see cref="Tracer" />s.
    /// </summary>
    [ContractClass(typeof(TracerFactoryContract))]
    public interface ITracerFactory : IDisposable
    {

        /// <summary>
        /// Returns a <see cref="Tracer" /> for tracing messages associated with <paramref name="name" />.
        /// </summary>
        /// <param name="name">
        /// A name for the tracer, often the class name or namespace containing the logic being traced. Can
        /// also be a category.
        /// </param>
        /// <returns>A <see cref="Tracer" />.</returns>
        Tracer GetTracer(string name);

        /// <summary>
        /// Returns a <see cref="Tracer" /> for tracing messages associated with <paramref name="type" />.
        /// </summary>
        /// <param name="type">
        /// Used to determine the <see cref="Tracer.Name"/>, using <see cref="TraceManagerConfig.TypeNameFunc"/>
        /// </param>
        /// <returns>A <see cref="Tracer" />.</returns>
        Tracer GetTracer(Type type);

    }

    [ContractClassFor(typeof(ITracerFactory))]
    internal abstract class TracerFactoryContract : ITracerFactory
    {

        #region Implementation of ITracerFactory

        public Tracer GetTracer(string name)
        {
            Contract.Ensures(Contract.Result<Tracer>() != null);

            throw new NotImplementedException();
        }

        public Tracer GetTracer(Type type)
        {
            Contract.Ensures(Contract.Result<Tracer>() != null);

            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IDisposable

        public abstract void Dispose();

        #endregion

    }

}
