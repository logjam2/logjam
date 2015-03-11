// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;

	using LogJam.Trace;
	using LogJam.Writer;


	/// <summary>
	/// The non-generic interface that all <see cref="LogWriterConfig{TEntry}"/> instances implement.
	/// </summary>
	public interface ILogWriterConfig : IEquatable<ILogWriterConfig>
	{
		/// <summary>
		/// Sets or gets whether the <see cref="ILogWriter{TEntry}"/> returned from <see cref="CreateILogWriter"/> should have its
		/// writes synchronized or not.
		/// </summary>
		// TODO: Remove, move to LogManagerConfig?
		bool Synchronized { get; set; }

		/// <summary>
		/// Creates and returns a new <see cref="ILogWriter"/> using the configured settings.
		/// </summary>
		/// <param name="setupTracerFactory">An <see cref="ITracerFactory"/> for tracing information about logging setup.</param>
		/// <returns>A new <see cref="ILogWriter"/> using the configured settings.</returns>
		ILogWriter CreateILogWriter(ITracerFactory setupTracerFactory);
	}

}