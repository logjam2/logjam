// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogEncoder.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode
{
    using System.IO;


    /// <summary>
    /// Encodes log entries for an output byte stream.
    /// </summary>
    public interface ILogEncoder<TEntry>
        where TEntry : ILogEntry
    {

        void EncodeEntry(ref TEntry entry, Stream stream);

    }

}
