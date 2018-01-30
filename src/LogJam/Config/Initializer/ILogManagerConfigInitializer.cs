// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogManagerConfigInitializer.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Initializer
{

    /// <summary>
    /// An initializer that can be used to modify the <see cref="LogManagerConfig"/> instance. Can be used eg to
    /// apply formatters to all <see cref="TextLogWriterConfig"/> instances before the configuration is used, but after
    /// log writers configuration is added.
    /// </summary>
    public interface ILogManagerConfigInitializer : IInitializer
    {

        /// <summary>
        /// Initializers can implement this to modify the <see cref="LogManagerConfig"/> before it is used.
        /// </summary>
        /// <param name="logManagerConfig"></param>
        void UpdateLogManagerConfig(LogManagerConfig logManagerConfig);

    }

}
