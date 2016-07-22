// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamSetupException.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;


    /// <summary>
    /// Signifies an error in a setup operation.
    /// </summary>
    public class LogJamSetupException : LogJamException
    {

        protected internal LogJamSetupException(string message, object source)
            : base(message, source)
        {}

        protected internal LogJamSetupException(string message, Exception innerException, object source)
            : base(message, innerException, source)
        {}

    }

}
