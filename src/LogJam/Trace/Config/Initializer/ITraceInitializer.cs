// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITraceInitializer.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config.Initializer
{

    /// <summary>
    /// Base interface for trace initializers, which allows separation of specialized startup logic.
    /// </summary>
    /// <see cref="TraceManagerConfig.Initializers"/>
    public interface ITraceInitializer
    {

        /// <summary>
        /// Called when <paramref name="traceManager"/> is starting.
        /// </summary>
        /// <param name="traceManager"></param>
        void OnStarting(TraceManager traceManager);

    }

}