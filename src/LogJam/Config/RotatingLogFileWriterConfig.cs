// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotatingLogFileWriterConfig.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Config.Initializer;
	using LogJam.Trace;
	using LogJam.Writer;
	using LogJam.Writer.Rotator;


	/// <summary>
	/// Configures a rotating log file writer.
	/// </summary>
	public sealed class RotatingLogFileWriterConfig : LogWriterConfig, ILogWriterPipelineInitializer, IImportInitializer
	{

		public RotatingLogFileWriterConfig(LogFileRotatorConfig logFileRotatorConfig, ILogFileWriterConfig logFileWriterConfig)
		{
			Contract.Requires<ArgumentNullException>(logFileRotatorConfig != null);
			Contract.Requires<ArgumentNullException>(logFileWriterConfig != null);

			LogFileRotator = logFileRotatorConfig;
			LogFileWriter = logFileWriterConfig;

			Initializers.Add(this);
		}

		/// <summary>
		/// Configures the writer for individual files.
		/// </summary>
		public ILogFileWriterConfig LogFileWriter { get; set; }

		/// <summary>
		/// Configures the <see cref="ILogFileRotator"/> used to implement file rotation logic.
		/// </summary>
		public LogFileRotatorConfig LogFileRotator { get; set; }

		/// <summary>
		/// Event that is fired after a log file is rotated.
		/// </summary>
		public event EventHandler<AfterRotateLogFileEventArgs> AfterRotate;

		/// <summary>
		/// This property can only be <c>true</c>; file rotation is dependent on synchronization.
		/// </summary>
		public override bool Synchronize
		{
			get { return true; }
			set
			{
				if (value != true)
				{
					throw new LogJamSetupException("RotatingLogFileWriterConfig.Synchronize cannot be set to false.", this);
				}
			}
		}

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
			                                                      rotatorConfig.CreateLogFileRotator());
			rotatingLogFileWriter.AfterRotate += AfterRotate;
			return rotatingLogFileWriter;
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
			ISynchronizingLogWriter synchronizingLogWriter;
			if (! dependencyDictionary.TryGet(out synchronizingLogWriter))
			{
				throw new LogJamSetupException("Cannot create RotatingLogFileWriter because ISynchronizingLogWriter reference is not available.", this);
			}
			rotatingLogFileWriter.SetSynchronizingLogWriter(synchronizingLogWriter);
		}

	}

}