// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSystemClock.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Threading;

namespace LogJam.Internal.UnitTests
{

    /// <summary>
    /// Enables settable time for tests.
    /// </summary>
    public class TestSystemClock : ISystemClock
    {

        public DateTimeOffset UtcNow { get; set; }

        public TestTimer TestTimer { get; } = new TestTimer();

        public IDisposable CreateTimer(TimerCallback timerCallback, object callbackState, TimeSpan dueTime, TimeSpan period)
        {
            return TestTimer.SetCallback(timerCallback, callbackState);
        }

    }

}
