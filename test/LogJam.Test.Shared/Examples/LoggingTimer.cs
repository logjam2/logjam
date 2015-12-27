// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingTimer.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Examples
{
    using System;
    using System.Threading;

    using LogJam.Writer;


    /// <summary>
    /// Provides timings and logs timing records - this isn't intended as an awesome timer implementation, just
    /// something to test logging multiple record types to a <see cref="IMultiLogWriter" />.
    /// </summary>
    public sealed class LoggingTimer
    {

        private static long s_nextTimingId = 1;

        private readonly IEntryWriter<StartRecord> _startWriter;
        private readonly IEntryWriter<StopRecord> _stopWriter;

        public static void RestartTimingIds()
        {
            s_nextTimingId = 1;
        }

        public LoggingTimer(string name, LogManager logManager)
        {
            Name = name;
            _startWriter = logManager.GetEntryWriter<StartRecord>();
            _stopWriter = logManager.GetEntryWriter<StopRecord>();
        }

        public string Name { get; private set; }

        public Timing Start()
        {
            long timingId = Interlocked.Increment(ref s_nextTimingId);
            return new Timing(this, timingId);
        }


        /// <summary>
        /// Tracks timing since <see cref="LoggingTimer.Start" /> was called.
        /// </summary>
        public struct Timing
        {

            private readonly LoggingTimer _parentTimer;
            private readonly long _id;
            private readonly DateTime _startDateTime;
            private DateTime? _stopDateTime;

            internal Timing(LoggingTimer timer, long timingId)
            {
                _parentTimer = timer;
                _id = timingId;
                _startDateTime = DateTime.Now;
                _stopDateTime = null;

                var startRecord = new StartRecord()
                                  {
                                      StartDateTime = _startDateTime,
                                      Timer = _parentTimer,
                                      TimingId = _id
                                  };
                _parentTimer._startWriter.Write(ref startRecord);
            }

            public void Stop()
            {
                var now = DateTime.Now;
                _stopDateTime = now;
                var stopRecord = new StopRecord()
                                 {
                                     StopDateTime = now,
                                     ElapsedTime = now.Subtract(_startDateTime),
                                     Timer = _parentTimer,
                                     TimingId = _id
                                 };
                _parentTimer._stopWriter.Write(ref stopRecord);
            }

        }


        /// <summary>
        /// Records a timer start.
        /// </summary>
        public struct StartRecord : ILogEntry
        {

            public long TimingId;
            public DateTime StartDateTime;
            public LoggingTimer Timer;

        }


        /// <summary>
        /// Records a timer stop.
        /// </summary>
        public struct StopRecord : ILogEntry
        {

            public long TimingId;
            public DateTime StopDateTime;
            public TimeSpan ElapsedTime;
            public LoggingTimer Timer;

        }

    }

}
