// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportActionInitializer.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Initializer
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Trace;


	/// <summary>
	/// An <see cref="IImportInitializer"/> that runs an action during logwriter initialization.
	/// </summary>
	internal sealed class ImportActionInitializer : IImportInitializer
	{

		private readonly Action<DependencyDictionary> _importAction;

		public ImportActionInitializer(Action<DependencyDictionary> importAction)
		{
			Contract.Requires<ArgumentNullException>(importAction != null);
			_importAction = importAction;
		}

		public void ImportDependencies(ITracerFactory setupTracerFactory, DependencyDictionary dependencyDictionary)
		{
			_importAction(dependencyDictionary);
		}

	}

}