// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Collections.Generic;

	using LogJam.Config.Initializer;
	using LogJam.Trace;
	using LogJam.Trace.Config;
	using LogJam.Writer;


	/// <summary>
	/// Base class for logwriter configuration.
	/// </summary>
	/// <see cref="TraceWriterConfig.LogWriterConfig"/>
	public abstract class LogWriterConfig : ILogWriterConfig
	{

		private bool _synchronize = true;
		private bool _disposeOnStop = true;
		private readonly ICollection<ILogWriterInitializer> _initializers = new List<ILogWriterInitializer>();

		/// <summary>
		/// Sets or gets whether the <see cref="IEntryWriter{TEntry}"/> returned from <see cref="CreateLogWriter"/> should have its
		/// writes synchronized or not.  Default is <c>true</c>.
		/// </summary>
		public virtual bool Synchronize { get { return _synchronize; } set { _synchronize = value; } }

		/// <summary>
		/// Sets or gets whether log writes should be queued from the logging thread, and written on a single background thread.
		/// Default is <c>false</c>.
		/// </summary>
		public virtual bool BackgroundLogging { get; set; }

		/// <summary>
		/// Sets or gets whether the <see cref="ILogWriter"/> created by <see cref="CreateLogWriter"/> should be disposed
		/// when the <see cref="LogManager"/> is stopped.  Default is <c>true</c>.
		/// </summary>
		public virtual bool DisposeOnStop { get { return _disposeOnStop; } set { _disposeOnStop = value; } }

		/// <inheritdoc />
		public abstract ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory);

		public ICollection<ILogWriterInitializer> Initializers { get { return _initializers; } }

	}

}