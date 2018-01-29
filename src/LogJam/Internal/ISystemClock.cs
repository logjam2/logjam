// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISystemClock.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Threading;

namespace LogJam.Internal
{

    /// <summary>
    /// Exposes date + time functions that can be replaced with test fakes for testing.
    /// </summary>
    internal interface ISystemClock
    {

        /// <summary>
        /// Returns the current <see cref="DateTimeOffset"/> in UTC time.
        /// </summary>
        DateTimeOffset UtcNow { get; }

        /// <summary>
        /// Creates a timer which calls <paramref name="timerCallback"/> after <paramref name="dueTime"/> elapses, and then every <paramref name="period"/>.
        /// </summary>
        /// <param name="timerCallback">A callback delegate.</param>
        /// <param name="callbackState">A state object that is passed to <paramref name="timerCallback"/>.</param>
        /// <param name="dueTime">The amount of time to delay before <paramref name="timerCallback"/> is invoked, in milliseconds. 
        /// Specify Infinite to prevent the timer from starting. Specify zero (0) to start the timer immediately.</param>
        /// <param name="period">The time interval between invocations of <paramref name="timerCallback"/>, in milliseconds.
        /// Specify Infinite to disable periodic signaling.</param>
        /// <returns></returns>
        IDisposable CreateTimer(TimerCallback timerCallback, object callbackState, TimeSpan dueTime, TimeSpan period);

    }

}
