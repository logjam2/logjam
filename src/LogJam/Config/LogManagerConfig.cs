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

		#endregion

		/// <summary>
		/// Creates a new <see cref="LogManagerConfig"/>.
		/// </summary>
		public LogManagerConfig()
		{
			_logWriterConfigs = new HashSet<ILogWriterConfig>();
		}

		/// <summary>
		/// Creates a new <see cref="LogManagerConfig"/> using the specified <paramref name="logWriterConfigs"/>.
		/// </summary>
		public LogManagerConfig(params ILogWriterConfig[] logWriterConfigs)
		{
			Contract.Requires<ArgumentNullException>(logWriterConfigs != null);

			_logWriterConfigs = new HashSet<ILogWriterConfig>(logWriterConfigs);
		}

		/// <summary>
		/// Returns the set of <see cref="ILogWriterConfig"/> instances stored in this <see cref="LogManagerConfig"/>.
		/// </summary>
		public ISet<ILogWriterConfig> Writers
		{ get { return _logWriterConfigs; } }

	}

}