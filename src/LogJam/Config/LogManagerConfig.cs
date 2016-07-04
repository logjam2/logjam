// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using LogJam.Util;
    using LogJam.Writer;


    /// <summary>
    /// Holds the configuration settings for a <see cref="LogManager" /> instance.
    /// </summary>
    public sealed class LogManagerConfig
    {
        #region Fields

        /// <summary>
        /// Holds the configuration for <see cref="ILogWriter" />s.
        /// </summary>
        private readonly ObservableSet<ILogWriterConfig> _logWriterConfigs;

        #endregion

        /// <summary>
        /// Creates a new <see cref="LogManagerConfig" />.
        /// </summary>
        public LogManagerConfig()
        {
            _logWriterConfigs = new ObservableSet<ILogWriterConfig>();
        }

        /// <summary>
        /// Creates a new <see cref="LogManagerConfig" /> using the specified <paramref name="logWriterConfigs" />.
        /// </summary>
        public LogManagerConfig(params ILogWriterConfig[] logWriterConfigs)
        {
            Contract.Requires<ArgumentNullException>(logWriterConfigs != null);

            _logWriterConfigs = new ObservableSet<ILogWriterConfig>(new HashSet<ILogWriterConfig>(logWriterConfigs));
        }

        /// <summary>
        /// Returns the set of <see cref="ILogWriterConfig" /> instances stored in this <see cref="LogManagerConfig" />.
        /// </summary>
        public ISet<ILogWriterConfig> Writers { get { return _logWriterConfigs; } }

        public void Clear()
        {
            _logWriterConfigs.Clear();
        }

    }

}
