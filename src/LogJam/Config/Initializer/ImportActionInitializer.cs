// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportActionInitializer.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;

using LogJam.Shared.Internal;
using LogJam.Trace;

namespace LogJam.Config.Initializer
{

    /// <summary>
    /// An <see cref="IImportInitializer" /> that runs an action during logwriter initialization.
    /// </summary>
    internal sealed class ImportActionInitializer : IImportInitializer
    {

        private readonly Action<DependencyDictionary> _importAction;

        public ImportActionInitializer(Action<DependencyDictionary> importAction)
        {
            Arg.NotNull(importAction, nameof(importAction));

            _importAction = importAction;
        }

        public void ImportDependencies(ITracerFactory setupTracerFactory, DependencyDictionary dependencyDictionary)
        {
            _importAction(dependencyDictionary);
        }

    }

}
