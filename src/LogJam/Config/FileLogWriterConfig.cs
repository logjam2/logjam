// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using LogJam.Trace;
    using LogJam.Writer;


    /// <summary>
    /// Configures a log writer that writes to the specified <see cref="File" />.
    /// </summary>
    public sealed class FileLogWriterConfig : LogWriterConfig
    {

        public string Directory { get; set; }

        public string File { get; set; }

        public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
        {
            throw new System.NotImplementedException();
        }

    }

}
