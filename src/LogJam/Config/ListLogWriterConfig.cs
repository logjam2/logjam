// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListLogWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System.Collections.Generic;

	using LogJam.Config.Json;
    using LogJam.Trace;
    using LogJam.Writer;


    /// <summary>
    /// Log writer configuration that creates a <see cref="ListLogWriter{TEntry}" />
    /// </summary>
    [JsonTypeHint("Target", "List")]
    public class ListLogWriterConfig<TEntry> : LogWriterConfig
        where TEntry : ILogEntry
    {

		/// <summary>
		/// Gets or sets the list to write to.
		/// </summary>
		public IList<TEntry> List { get; set; }


		public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
        {
            return new ListLogWriter<TEntry>(setupTracerFactory, List, Synchronize);
        }

    }

}
