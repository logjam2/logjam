// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamStartException.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;


    /// <summary>
    /// Thrown when LogJam could not be successfully started.
    /// </summary>
    public sealed class LogJamStartException : LogJamException
    {

        internal LogJamStartException(string message, Exception innerException, object source)
            : base(message, innerException, source)
        {}

        internal LogJamStartException(string message, object source)
            : base(message, source)
        {}

    }

}
