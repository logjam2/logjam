// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterException.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;
    using System.Diagnostics.Contracts;

    using LogJam.Writer;


    /// <summary>
    /// Thrown when an <see cref="IEntryWriter{TEntry}" /> failed to write to a log.
    /// </summary>
    public sealed class LogWriterException : LogJamException
    {

        private readonly ILogWriter _logWriter;

        public LogWriterException(string message, Exception innerException, ILogWriter logWriter)
            : base(message, innerException, logWriter)
        {
            Contract.Requires<ArgumentNullException>(logWriter != null);

            _logWriter = logWriter;
        }

        public LogWriterException(string message, ILogWriter logWriter)
            : base(message, logWriter)
        {
            Contract.Requires<ArgumentNullException>(logWriter != null);

            _logWriter = logWriter;
        }

    }

}
