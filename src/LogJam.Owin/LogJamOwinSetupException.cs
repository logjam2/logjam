// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamOwinSetupException.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin
{
    using System;


    /// <summary>
    /// Exception thrown when invalid setup is detected.
    /// </summary>
    public sealed class LogJamOwinSetupException : LogJamSetupException
    {

        internal LogJamOwinSetupException(string message, object source)
            : base(message, source)
        {}

        internal LogJamOwinSetupException(string message, Exception innerException, object source)
            : base(message, innerException, source)
        {}

    }

}
