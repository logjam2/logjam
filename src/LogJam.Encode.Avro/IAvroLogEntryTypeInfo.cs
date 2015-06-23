// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAvroLogEntryTypeInfo.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode.Avro
{
    using System;


    /// <summary>
    /// Provides a log entry <see cref="Type"/> that is logged to Avro.
    /// </summary>
    internal interface IAvroLogEntryTypeInfo
    {

        /// <summary>
        /// Returns the log entry type that is serialized to Avro.
        /// </summary>
        Type LogEntryType { get; }

    }

}