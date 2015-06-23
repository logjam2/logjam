// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogRequestHeader.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;
    using System.Threading;


    /// <summary>
    /// Header entry for network logging.  The first log entry in any request must be one of these.
    /// </summary>
    public struct LogRequestHeader : ITimestampedLogEntry
    {

        private static ApplicationEntry s_lastApplicationEntry;
        private static long s_lastRequestNumber;

        private Guid _applicationInstanceId;
        private long _requestNumber;
        private DateTime _timestampUtc;

        public LogRequestHeader(ApplicationEntry applicationEntry)
        {
            Contract.Requires<ArgumentNullException>(applicationEntry != null);

            if (! ReferenceEquals(s_lastApplicationEntry, applicationEntry))
            {
                s_lastApplicationEntry = applicationEntry;
                s_lastRequestNumber = 0;
                _requestNumber = 0;
            }
            else
            {
                _requestNumber = Interlocked.Increment(ref s_lastRequestNumber);
            }
            _applicationInstanceId = applicationEntry.ApplicationInstanceId;
            _timestampUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Uniquely identifies the application instance
        /// </summary>
        public Guid ApplicationInstanceId
        {
            get { return _applicationInstanceId; }
            set { _applicationInstanceId = value; }
        }

        /// <summary>
        /// The request number - request numbers are monotonically increasing for subsequent requests.
        /// </summary>
        public long LogRequestNumber
        {
            get { return _requestNumber; }
            set { _requestNumber = value; }
        }

        /// <summary>
        /// Timestamp in UTC that the log request began.  Used as the default timestamp
        /// for all the log entries in the request that don't implement <see cref="ITimestampedLogEntry"/>.
        /// </summary>
        public DateTime TimestampUtc
        {
            get { return _timestampUtc; }
            set { _timestampUtc = value; }
        }

    }

}