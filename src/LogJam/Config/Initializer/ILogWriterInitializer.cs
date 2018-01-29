// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogWriterInitializer.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Initializer
{
	using LogJam.Writer;


	/// <summary>
	/// Base interface for log writer initializers, so all types of initializers can be combined in a single collection 
	/// (<see cref="ILogWriterConfig.Initializers"/> and <see cref="LogManagerConfig.Initializers"/>). Log writer initializers are used both
	/// for cross-cutting concerns, and as a form of lightweight dependency injection that is resolved just after the log writer is created (before it is started).
	/// </summary>
	/// <remarks>
	/// Each log writer initializer should implement one or more of <see cref="IExtendLogWriterPipeline"/> and
	/// <see cref="IImportInitializer"/>. Initializers are called in this order:
	/// <list type="number">
	/// <item><term><see cref="IExtendLogWriterPipeline"/></term>
	/// <description>are called in order, to modify the logwriter pipeline, eg to create a <see cref="ProxyLogWriter"/> that enhances the behavior of the logwriter. In addition,
	/// initializers should export any objects of interest to other participants in the pipeline, via the <see cref="DependencyDictionary"/>.</description></item>
	/// <item><term><see cref="IImportInitializer"/></term>
	/// <description>are called in order, to connect objects in the log writer pipeline to services that have been exported by other initializers.</description></item>
	/// </list>
	/// </remarks>
	public interface ILogWriterInitializer
	{
	}

}