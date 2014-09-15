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


	/// <summary>
	/// The non-generic interface that all <see cref="LogWriterConfig{TEntry}"/> instances implement.
	/// </summary>
	public interface ILogWriterConfig : IEquatable<ILogWriterConfig>
	{
		/// <summary>
		/// Sets or gets whether the <see cref="ILogWriter{TEntry}"/> returned from <see cref="CreateILogWriter"/> should have its
		/// writes synchronized or not.
		/// </summary>
		bool Synchronized { get; set; }

		/// <summary>
		/// Returns the <see cref="ILogWriter{TEntry}"/> <c>TEntry</c> type for the logwriter returned from <see cref="CreateILogWriter"/>.
		/// </summary>
		Type BaseEntryType { get; }

		/// <summary>
		/// Creates and returns a new <see cref="ILogWriter"/> using the configured settings.
		/// </summary>
		/// <returns>A new <see cref="ILogWriter"/> using the configured settings.</returns>
		ILogWriter CreateILogWriter();
	}

}