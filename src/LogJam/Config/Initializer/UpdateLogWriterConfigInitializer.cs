// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateLogWriterConfigInitializer.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Linq;

namespace LogJam.Config.Initializer
{

    /// <summary>
    /// An initializer that updates all <see cref="LogManagerConfig.Writers"/> that are of type <typeparamref name="TLogWriterConfig"/>
    /// or its subclasses.
    /// </summary>
    public sealed class UpdateLogWriterConfigInitializer<TLogWriterConfig> : ILogManagerConfigInitializer
        where TLogWriterConfig : ILogWriterConfig
    {

        private readonly Action<TLogWriterConfig> _updateAction;

        /// <summary>
        /// Creates a new <see cref="UpdateLogWriterConfigInitializer{TLogWriterConfig}"/> instance, which calls
        /// <paramref name="updateAction"/> for all applicable log writer configs.
        /// </summary>
        /// <param name="updateAction">A delegate to update the <typeparamref name="TLogWriterConfig"/>.</param>
        public UpdateLogWriterConfigInitializer(Action<TLogWriterConfig> updateAction)
        {
            _updateAction = updateAction ?? throw new ArgumentNullException(nameof(updateAction));
        }

        /// <inheritdoc />
        public void UpdateLogManagerConfig(LogManagerConfig logManagerConfig)
        {
            foreach (var writerConfig in logManagerConfig.Writers.Flatten())
            {
                if (writerConfig is TLogWriterConfig logWriterConfig)
                {
                    _updateAction(logWriterConfig);
                }
            }
        }

    }

}
