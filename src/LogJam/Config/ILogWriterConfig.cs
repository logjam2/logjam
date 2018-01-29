// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriterConfig.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
#if CODECONTRACTS
    using System.Diagnostics.Contracts;
#endif
using LogJam.Config.Initializer;
using LogJam.Trace;
using LogJam.Writer;


namespace LogJam.Config
{

    /// <summary>
    /// Base interface for types that configure <see cref="ILogWriter" />s. An <see cref="ILogWriterConfig" />
    /// acts as a factory for an <c>ILogWriter</c>.
    /// </summary>
    /// <remarks>
    /// <c>ILogWriterConfig</c> objects should generally not override <see cref="object.GetHashCode" /> or
    /// <see cref="object.Equals(object)" />,
    /// because they are identified by reference. It should be valid to have two <c>ILogWriterConfig</c> objects with the same
    /// values stored
    /// in a set or dictionary.
    /// </remarks>
#if CODECONTRACTS
    [ContractClass(typeof(LogWriterConfigContract))]
#endif
    public interface ILogWriterConfig
    {

        /// <summary>
        /// Sets or gets whether the <see cref="ILogWriter" /> returned from <see cref="CreateLogWriter" /> should have its
        /// writes synchronized or not.
        /// </summary>
        // TODO: Remove, move to LogManagerConfig as a global setting?
        bool Synchronize { get; set; }

        /// <summary>
        /// Sets or gets whether log writes should be queued from the logging thread, and written on a single background thread.
        /// </summary>
        bool BackgroundLogging { get; set; }

        /// <summary>
        /// Sets or gets whether the <see cref="ILogWriter" /> created by <see cref="CreateLogWriter" /> should be disposed
        /// when the <see cref="LogManager" /> is stopped.
        /// </summary>
        bool DisposeOnStop { get; set; }

        /// <summary>
        /// Creates and returns a new <see cref="ILogWriter" /> using the configured settings.
        /// </summary>
        /// <param name="setupTracerFactory">An <see cref="ITracerFactory" /> for tracing information about logging setup.</param>
        /// <returns>A new <see cref="ILogWriter" /> using the configured settings.</returns>
        ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory);

        /// <summary>
        /// Returns a collection of initializers that are applied to any <see cref="ILogWriter" />s returned from
        /// <see cref="CreateLogWriter" />.
        /// </summary>
        /// <remarks>
        /// After these initializers are applied to a logwriter, <c>LogManager</c>-global initializers from
        /// <see cref="LogManagerConfig.Initializers" /> are applied.
        /// </remarks>
        ICollection<ILogWriterInitializer> Initializers { get; }

    }

#if CODECONTRACTS
    [ContractClassFor(typeof(ILogWriterConfig))]
    internal abstract class LogWriterConfigContract : ILogWriterConfig
    {

        public bool Synchronize { get; set; }
        public bool BackgroundLogging { get; set; }
        public bool DisposeOnStop { get; set; }

        public ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
        {
            Contract.Requires<ArgumentNullException>(setupTracerFactory != null);

            throw new NotImplementedException();
        }

        public ICollection<ILogWriterInitializer> Initializers
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<ILogWriterInitializer>>() != null);

                throw new NotImplementedException();
            }
        }

    }
#endif

}
