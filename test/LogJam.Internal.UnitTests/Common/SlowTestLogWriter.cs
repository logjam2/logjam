// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlowTestLogWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Common
{
    using System.Threading;

    using LogJam.Trace;


    /// <summary>
    /// Fakes consistent slow logging to verify proper handling of slow log writers by LogJam.
    /// </summary>
    public sealed class SlowTestLogWriter<TEntry> : TestLogWriter<TEntry>
        where TEntry : ILogEntry
    {

        public SlowTestLogWriter(ITracerFactory setupTracerFactory, int msDelay, bool synchronize)
            : base(setupTracerFactory, synchronize)
        {
            WriteEntryDelayMs = msDelay;
            StartDelayMs = msDelay;
            StopDelayMs = msDelay;
            DisposeDelayMs = msDelay;
        }

        public int WriteEntryDelayMs { get; set; }
        public int StartDelayMs { get; set; }
        public int StopDelayMs { get; set; }
        public int DisposeDelayMs { get; set; }

        protected override void InternalStart()
        {
            Thread.Sleep(StartDelayMs);
            base.InternalStart();
        }

        protected override void InternalStop()
        {
            if (IsStarted)
            {
                Thread.Sleep(StopDelayMs);
                base.InternalStop();
            }
        }

        public override void Write(ref TEntry entry)
        {
            if (IsStarted)
            {
                Thread.Sleep(WriteEntryDelayMs);
                base.Write(ref entry);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Thread.Sleep(DisposeDelayMs);
            }
        }

    }

}
