// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImportInitializer.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Initializer
{
	using LogJam.Trace;


	/// <summary>
	/// An initializer that can be implemented to resolve dependencies after the log writer pipeline has been built.
	/// </summary>
	public interface IImportInitializer : ILogWriterInitializer
	{

		/// <summary>
		/// An initializer can implement this method to import dependencies from <paramref name="dependencyDictionary"/> and wire up
		/// objects that the initializer knows about.
		/// </summary>
		/// <param name="setupTracerFactory"></param>
		/// <param name="dependencyDictionary"></param>
		void ImportDependencies(ITracerFactory setupTracerFactory, DependencyDictionary dependencyDictionary);

	}

}