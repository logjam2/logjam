// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamException.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;


    /// <summary>
    /// Base class for LogJam exceptions.
    /// </summary>
    public class LogJamException : Exception
    {

        /// <summary>
        /// Creates a new <see cref="LogJamException" />.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="source">The object throwing the exception; may be a type or <c>null</c></param>
        internal LogJamException(string message, object source)
            : base(message)
        {
            if (source != null)
            {
                base.Source = source.ToString();
            }
        }

        /// <summary>
        /// Creates a new <see cref="LogJamException" /> triggered by <paramref name="innerException" />.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="source">The object throwing the exception; may be a type or <c>null</c></param>
        internal LogJamException(string message, Exception innerException, object source)
            : base(message, innerException)
        {
            if (source != null)
            {
                base.Source = source.ToString();
            }
        }

    }

}
