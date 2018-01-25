// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterInitializerExtensions.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;

using LogJam.Config.Initializer;
using LogJam.Shared.Internal;

namespace LogJam.Config
{

    /// <summary>
    /// Extension methods for convenient use of <see cref="ILogWriterInitializer" />s.
    /// </summary>
    public static class LogWriterInitializerExtensions
    {

        /// <summary>
        /// Adds an initialization action to be called during log writer pipeline initialization.
        /// </summary>
        /// <param name="initializerCollection"></param>
        /// <param name="importAction"></param>
        public static void Add(this ICollection<ILogWriterInitializer> initializerCollection, Action<DependencyDictionary> importAction)
        {
            Arg.NotNull(initializerCollection, nameof(initializerCollection));
            Arg.NotNull(importAction, nameof(importAction));

            initializerCollection.Add(new ImportActionInitializer(importAction));
        }

    }

}
