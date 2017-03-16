// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogFileRotatorConfig.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System.Collections.Generic;

    using LogJam.Config.Initializer;
    using LogJam.Writer;
    using LogJam.Writer.Rotator;


    /// <summary>
    /// Base class for configuring <see cref="ILogFileRotator"/>s.
    /// </summary>
    public abstract class LogFileRotatorConfig
    {

        /// <summary>
        /// Returns an <see cref="ILogFileRotator"/>.
        /// </summary>
        /// <returns></returns>
        public abstract ILogFileRotator CreateLogFileRotator();

        /// <summary>
        /// Returns a collection of initializers that are applied to any <see cref="RotatingLogFileWriter" />s that
        /// are associated with this <see cref="LogFileRotatorConfig"/>.
        /// </summary>
        public ICollection<ILogWriterInitializer> Initializers { get; } = new HashSet<ILogWriterInitializer>();

    }

}
