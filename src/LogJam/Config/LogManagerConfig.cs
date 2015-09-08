// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using LogJam.Config.Initializer;
	using LogJam.Writer;


	/// <summary>
	/// Holds the configuration settings for a <see cref="LogManager"/> instance.
	/// </summary>
	public sealed class LogManagerConfig
	{

		#region Fields

		/// <summary>
		/// Holds the configuration for <see cref="ILogWriter"/>s.
		/// </summary>
		private readonly ISet<ILogWriterConfig> _logWriterConfigs;

		/// <summary>
		/// Holds initializers that are applied to all log writers created from this <c>LogManagerConfig</c>.
		/// </summary>
		private readonly List<ILogWriterInitializer> _initializers; 

		#endregion

		public static readonly IEnumerable<ILogWriterInitializer> DefaultInitializers = new ILogWriterInitializer[] { new BackgroundMultiLogWriter.Initializer(), new SynchronizingProxyLogWriter.Initializer() };

		/// <summary>
		/// Creates a new <see cref="LogManagerConfig"/>.
		/// </summary>
		public LogManagerConfig()
		{
			_logWriterConfigs = new HashSet<ILogWriterConfig>();
			_initializers = new List<ILogWriterInitializer>(DefaultInitializers);
		}

		/// <summary>
		/// Creates a new <see cref="LogManagerConfig"/> using the specified <paramref name="logWriterConfigs"/>.
		/// </summary>
		public LogManagerConfig(params ILogWriterConfig[] logWriterConfigs)
		{
			Contract.Requires<ArgumentNullException>(logWriterConfigs != null);

			_logWriterConfigs = new HashSet<ILogWriterConfig>(logWriterConfigs);
			_initializers = new List<ILogWriterInitializer>(DefaultInitializers);
		}

		/// <summary>
		/// Returns the set of <see cref="ILogWriterConfig"/> instances stored in this <see cref="LogManagerConfig"/>.
		/// </summary>
		public ISet<ILogWriterConfig> Writers
		{ get { return _logWriterConfigs; } }

		/// <summary>
		/// Returns a collection of initializers that are applied to all <see cref="ILogWriter"/>s created from this <see cref="LogManagerConfig"/>.
		/// </summary>
		/// <remarks>
		/// These are <c>LogManager</c>-global initializers.  Each <see cref="ILogWriterConfig"/> also has a collection of initializers that are
		/// applied only to logwriters created by the log writer config.
		/// <para>
		/// Initializers are applied in-order.  <see cref="ILogWriterConfig.Initializers"/> are applied before these global initializers.
		/// </para>
		/// </remarks>
		public ICollection<ILogWriterInitializer> Initializers { get { return _initializers; } }

		/// <summary>
		/// Reset the configuration to empty/brand new, so that the containing <see cref="LogManager"/> can be re-used with new configuration.
		/// </summary>
		public void Reset()
		{
			_logWriterConfigs.Clear();
			_initializers.Clear();
			_initializers.AddRange(DefaultInitializers);
		}

	}

}