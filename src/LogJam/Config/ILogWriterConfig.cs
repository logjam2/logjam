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
	/// Base interface for types that configure <see cref="ILogWriter"/>s.  An <see cref="ILogWriterConfig"/>
	/// acts as a factory for an <c>ILogWriter</c>.
	/// </summary>
	/// <remarks>
	/// <c>ILogWriterConfig</c> objects should generally not override <see cref="object.GetHashCode"/> or <see cref="object.Equals(object)"/>, 
	/// because they are identified by reference.  It should be valid to have two <c>ILogWriterConfig</c> objects with the same values stored
	/// in a set or dictionary.
	/// </remarks>
	public interface ILogWriterConfig
	{
		/// <summary>
		/// Sets or gets whether the <see cref="ILogWriter"/> returned from <see cref="CreateLogWriter"/> should have its
		/// writes synchronized or not.
		/// </summary>
		// TODO: Remove, move to LogManagerConfig as a global setting?
		bool Synchronized { get; set; }

		/// <summary>
		/// Sets or gets whether log writes should be queued from the logging thread, and written on a single background thread.
		/// </summary>
		bool BackgroundLogging { get; set; }

		/// <summary>
		/// Sets or gets whether the <see cref="ILogWriter"/> created by <see cref="CreateLogWriter"/> should be disposed
		/// when the <see cref="LogManager"/> is stopped.
		/// </summary>
		bool DisposeOnStop { get; set; }

		/// <summary>
		/// Creates and returns a new <see cref="ILogWriter"/> using the configured settings.
		/// </summary>
		/// <param name="setupTracerFactory">An <see cref="ITracerFactory"/> for tracing information about logging setup.</param>
		/// <returns>A new <see cref="ILogWriter"/> using the configured settings.</returns>
		ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory);
	}

}