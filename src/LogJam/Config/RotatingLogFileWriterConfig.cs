// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotatingLogFileWriterConfig.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using LogJam.Config.Initializer;
using LogJam.Shared.Internal;
using LogJam.Trace;
using LogJam.Writer;
using LogJam.Writer.Rotator;

namespace LogJam.Config
{

    /// <summary>
    /// Configures a rotating log file writer.
    /// </summary>
    public sealed class RotatingLogFileWriterConfig : LogWriterConfig,
                                                      IExtendLogWriterPipeline,
                                                      IImportInitializer,
                                                      IEnumerable<ILogWriterConfig>
    {

        private LogFileRotatorConfig _logFileRotator;

        private ILogFileWriterConfig _logFileWriter;

        public RotatingLogFileWriterConfig(LogFileRotatorConfig logFileRotatorConfig, ILogFileWriterConfig logFileWriterConfig)
        {
            Arg.NotNull(logFileRotatorConfig, nameof(logFileRotatorConfig));
            Arg.NotNull(logFileWriterConfig, nameof(logFileWriterConfig));

            LogFileRotator = logFileRotatorConfig;
            LogFileWriter = logFileWriterConfig;

            // Use the initializers provided by this type
            Initializers.Add(this);
        }

        /// <summary>
        /// Configures the writer for individual files.
        /// </summary>
        public ILogFileWriterConfig LogFileWriter
        {
            get { return _logFileWriter; }
            set
            {
                Arg.NotNull(value, nameof(LogFileWriter));

                _logFileWriter = value;
            }
        }

        /// <summary>
        /// Configures the <see cref="ILogFileRotator" /> used to implement file rotation logic.
        /// </summary>
        public LogFileRotatorConfig LogFileRotator
        {
            get => _logFileRotator;
            set
            {
                Arg.NotNull(value, nameof(LogFileRotator));

                _logFileRotator = value;
            }
        }

        /// <summary>
        /// This property can only be <c>true</c>; file rotation is dependent on synchronization.
        /// </summary>
        public override bool Synchronize
        {
            get => true;
            set
            {
                if (value != true)
                {
                    throw new LogJamSetupException("RotatingLogFileWriterConfig.Synchronize cannot be set to false.", this);
                }
            }
        }

        /// <summary>
        /// Returns combined <see cref="ILogWriterInitializer" />s from this <see cref="RotatingLogFileWriterConfig" />, and the
        /// <see cref="LogFileRotatorConfig" />.
        /// </summary>
        public override ICollection<ILogWriterInitializer> Initializers
        {
            get
            {
                var rotatorInitializers = LogFileRotator.Initializers;
                if (rotatorInitializers.Count == 0)
                {
                    return base.Initializers;
                }
                else
                {
                    var combinedInitializers = new HashSet<ILogWriterInitializer>(base.Initializers);
                    combinedInitializers.UnionWith(rotatorInitializers);
                    return combinedInitializers;
                }
            }
        }

        public ILogWriter InitializeLogWriter(ITracerFactory setupTracerFactory, ILogWriter logWriter, DependencyDictionary dependencyDictionary)
        {
            // export the LogFileRotator
            var rotatingLogFileWriter = dependencyDictionary.Get<RotatingLogFileWriter>();
            dependencyDictionary.Add(typeof(ILogFileRotator), rotatingLogFileWriter.LogFileRotator);
            dependencyDictionary.Add(rotatingLogFileWriter.LogFileRotator.GetType(), rotatingLogFileWriter.LogFileRotator);

            return logWriter;
        }

        public void ImportDependencies(ITracerFactory setupTracerFactory, DependencyDictionary dependencyDictionary)
        {
            // Connect the RotatingLogFileWriter to the ISynchronizingLogWriter
            var rotatingLogFileWriter = dependencyDictionary.Get<RotatingLogFileWriter>();
            if (! dependencyDictionary.TryGet(out ISynchronizingLogWriter synchronizingLogWriter))
            {
                throw new LogJamSetupException("Cannot create RotatingLogFileWriter because ISynchronizingLogWriter reference is not available.", this);
            }

            rotatingLogFileWriter.SetSynchronizingLogWriter(synchronizingLogWriter);
        }

        /// <summary>
        /// Event that is fired after a log file is rotated.
        /// </summary>
        public event EventHandler<AfterRotateLogFileEventArgs> AfterRotate;

        public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
        {
            ILogFileWriterConfig logFileWriterConfig = LogFileWriter;
            LogFileRotatorConfig rotatorConfig = LogFileRotator;
            Tracer tracer = setupTracerFactory.TracerFor(this);

            if (logFileWriterConfig == null)
            {
                tracer.Error("LogFileWriter (config object) must be set to create a RotatingLogFileWriter.");
            }

            if (rotatorConfig == null)
            {
                tracer.Error("LogFileRotator (config object) must be set to create a RotatingLogFileWriter.");
            }

            if ((logFileWriterConfig == null) || (rotatorConfig == null))
            {
                return null;
            }

            var rotatingLogFileWriter = new RotatingLogFileWriter(setupTracerFactory,
                                                                  logFileWriterConfig,
                                                                  rotatorConfig.CreateLogFileRotator(logFileWriterConfig));
            rotatingLogFileWriter.AfterRotate += AfterRotate;
            return rotatingLogFileWriter;
        }

        #region IEnumerable<ILogWriterConfig>

        public IEnumerator<ILogWriterConfig> GetEnumerator()
        {
            if (_logFileWriter != null)
            {
                return new List<ILogWriterConfig>(1) { _logFileWriter }.GetEnumerator();
            }
            else
            {
                return Enumerable.Empty<ILogWriterConfig>().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

}
