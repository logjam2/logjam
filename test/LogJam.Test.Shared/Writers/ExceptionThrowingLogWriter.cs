// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionThrowingLogWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Test.Shared.Writers
{
    using System;

    using LogJam.Trace;
    using LogJam.Writer;


    /// <summary>
    /// An <see cref="ILogWriter" /> that throws exceptions when writing and disposing.
    /// </summary>
    public class ExceptionThrowingLogWriter<TEntry> : SingleEntryTypeLogWriter<TEntry>, IDisposable
        where TEntry : ILogEntry
    {

        public int CountExceptionsThrown = 0;

        public ExceptionThrowingLogWriter(ITracerFactory setupTracerFactory)
            : base(setupTracerFactory)
        {}

        public override bool IsSynchronized { get { return true; } }

        public override void Write(ref TEntry entry)
        {
            CountExceptionsThrown++;
            throw new ApplicationException();
        }

        protected override void Dispose(bool isDisposing)
        {
            CountExceptionsThrown++;
            throw new ApplicationException();
        }

    }

}
