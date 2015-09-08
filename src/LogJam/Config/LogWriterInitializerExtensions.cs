// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterInitializerExtensions.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using LogJam.Config.Initializer;


	/// <summary>
	/// Extension methods for convenient use of <see cref="ILogWriterInitializer"/>s.
	/// </summary>
	public static class LogWriterInitializerExtensions
	{
		/// <summary>
		/// Adds an initialization action to be called during log writer pipeline initialization.
		/// </summary>
		/// <param name="initializerCollection"></param>
		/// <param name="importAction"></param>
		public static void Add(this ICollection<ILogWriterInitializer> initializerCollection, Action<DependencyDictionary> importAction)
		{
			Contract.Requires<ArgumentNullException>(initializerCollection != null);
			Contract.Requires<ArgumentNullException>(importAction != null);

			initializerCollection.Add(new ImportActionInitializer(importAction));
		}

	}

}