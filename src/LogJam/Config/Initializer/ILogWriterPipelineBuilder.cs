// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriterPipelineBuilder.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Initializer
{
    using System.Collections.Generic;

    using LogJam.Trace;
    using LogJam.Writer;


    /// <summary>
    /// Interface for a type that builds the logwriter pipeline.
    /// </summary>
    public interface ILogWriterPipelineBuilder : ILogWriterInitializer
    {

        /// <summary>
        /// Creates a logwriter pipeline, starting with the logwriter created by <paramref name="logWriterConfig"/>, and building and connecting proxy logwriters as needed.
        /// </summary>
        /// <param name="logWriterConfig"></param>
        /// <param name="initializers"></param>
        /// <param name="logWriterDependencyDictionary"></param>
        /// <param name="setupTracerFactory"></param>
        /// <returns>The front <see cref="ILogWriter"/> in the pipeline.</returns>
        ILogWriter CreateLogWriterPipeline(ILogWriterConfig logWriterConfig, List<ILogWriterInitializer> initializers, DependencyDictionary logWriterDependencyDictionary, ITracerFactory setupTracerFactory);

    }

}