// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultLogWriterPipelineBuilder.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Initializer
{
    using System.Collections.Generic;
    using System.Linq;

    using LogJam.Trace;
    using LogJam.Writer;


    /// <summary>
    /// The default implementation of <see cref="ILogWriterPipelineBuilder"/>. Can be subclassed or replaced when custom pipeline configuration is needed.
    /// </summary>
    public class DefaultLogWriterPipelineBuilder : ILogWriterPipelineBuilder
    {

        /// <summary>
        /// Creates 
        /// </summary>
        /// <param name="logWriterConfig"></param>
        /// <param name="initializers"></param>
        /// <param name="logWriterDependencyDictionary"></param>
        /// <param name="setupTracerFactory"></param>
        /// <returns></returns>
        public virtual ILogWriter CreateLogWriterPipeline(ILogWriterConfig logWriterConfig, List<ILogWriterInitializer> initializers, DependencyDictionary logWriterDependencyDictionary, ITracerFactory setupTracerFactory)
        {
            ILogWriter logWriter = logWriterConfig.CreateLogWriter(setupTracerFactory);
            if (logWriter == null)
            { // Can occur when the logwriter cannot be created; the config object should log errors.
              // TODO: Ensure that the error state is represented
                return null;
            }
            logWriterDependencyDictionary.Add(logWriter.GetType(), logWriter);

            // Build pipeline and support dependency resolution using ILogWriterInitializers
            logWriter = ExtendLogWriterPipeline(logWriter, initializers, logWriterDependencyDictionary, setupTracerFactory);

            // Register the "front" logwriter as the ILogWriter type
            logWriterDependencyDictionary.AddIfNotDefined(typeof(ILogWriter), logWriter);
            logWriterDependencyDictionary.EndInitialization();

            ConnectImportsForLogWriterPipeline(initializers, logWriterDependencyDictionary, setupTracerFactory);

            return logWriter;
        }

        /// <summary>
        /// Creates the <see cref="ILogWriter"/> at the end of the pipeline, using <paramref name="logWriterConfig"/>.
        /// </summary>
        /// <param name="logWriterConfig"></param>
        /// <param name="logWriterDependencyDictionary"></param>
        /// <param name="setupTracerFactory"></param>
        /// <returns></returns>
        protected virtual ILogWriter CreateLogWriter(ILogWriterConfig logWriterConfig,
                                                     DependencyDictionary logWriterDependencyDictionary,
                                                     ITracerFactory setupTracerFactory)
        {
            return logWriterConfig.CreateLogWriter(setupTracerFactory);
        }

        /// <summary>
        /// Adds proxy log writers, building up the front of the pipeline.
        /// </summary>
        /// <param name="logWriter"></param>
        /// <param name="initializers"></param>
        /// <param name="dependencyDictionary"></param>
        /// <param name="setupTracerFactory"></param>
        /// <returns></returns>
        protected virtual ILogWriter ExtendLogWriterPipeline(ILogWriter logWriter, List<ILogWriterInitializer> initializers, DependencyDictionary dependencyDictionary, ITracerFactory setupTracerFactory)
        {
            foreach (var pipelineInitializer in initializers.OfType<IExtendLogWriterPipeline>())
            {
                logWriter = pipelineInitializer.InitializeLogWriter(setupTracerFactory, logWriter, dependencyDictionary);
                dependencyDictionary.AddIfNotDefined(logWriter.GetType(), logWriter);
            }
            return logWriter;
        }

        /// <summary>
        /// Provides dependencies to <see cref="IImportInitializer"/>s.
        /// </summary>
        /// <param name="initializers"></param>
        /// <param name="dependencyDictionary"></param>
        /// <param name="setupTracerFactory"></param>
        protected virtual void ConnectImportsForLogWriterPipeline(List<ILogWriterInitializer> initializers, DependencyDictionary dependencyDictionary, ITracerFactory setupTracerFactory)
        {
            foreach (var importInitializer in initializers.OfType<IImportInitializer>())
            {
                importInitializer.ImportDependencies(setupTracerFactory, dependencyDictionary);
            }
        }

    }

}