// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemClock.cs">
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
    /// The default implementation of <see cref="ISystemClock"/>, which uses real system time calls.
    /// </summary>
    internal sealed class SystemClock : ISystemClock
    {

        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public IDisposable CreateTimer(TimerCallback timerCallback, object callbackState, TimeSpan dueTime, TimeSpan period)
        {
            return new Timer(timerCallback, callbackState, dueTime, period);
        }

    }

}
