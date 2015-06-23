// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogRequest.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode.Avro
{

    /// <summary>
    /// Represents a chunk of <see cref="ILogEntry"/>s sent in a single request.  
    /// </summary>
    public sealed class LogRequest
    {

        public LogRequestHeader Header { get; set; }

        public ILogEntry[] Entries { get; set; }

    }

}