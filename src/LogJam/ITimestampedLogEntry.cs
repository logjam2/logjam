// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimestampedLogEntry.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;


    /// <summary>
    /// A log entry with a single timestamp property.
    /// </summary>
    public interface ITimestampedLogEntry : ILogEntry
    {
        /// <summary>
        /// Timestamp in UTC for the log entry.
        /// </summary>
        DateTime TimestampUtc { get; }

    }

}