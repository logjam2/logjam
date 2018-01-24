// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExtendLogWriterPipeline.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Initializer
{
    using LogJam.Trace;
    using LogJam.Writer;


    /// <summary>
    /// An initializer that extends or modifies the logwriter pipeline.
    /// </summary>
    public interface IExtendLogWriterPipeline : ILogWriterInitializer
    {

        /// <summary>
        /// Extends or modifies the logwriter pipeline; may export dependencies to <paramref name="dependencyDictionary"/>.
        /// </summary>
        /// <param name="setupTracerFactory"></param>
        /// <param name="logWriter"></param>
        /// <param name="dependencyDictionary"></param>
        /// <returns>The new log writer; or <paramref name="logWriter"/> if no modification was made to the front of the log writer pipeline.</returns>
        /// <remarks>This method should not assume that dependencies populated by other initializers are in <paramref name="dependencyDictionary"/>, because the order
        /// of initializers may vary.  If an initializer requires dependencies populated by other initializers, it should use <see cref="IImportInitializer"/> instead.</remarks>
        ILogWriter InitializeLogWriter(ITracerFactory setupTracerFactory, ILogWriter logWriter, DependencyDictionary dependencyDictionary);

    }

}