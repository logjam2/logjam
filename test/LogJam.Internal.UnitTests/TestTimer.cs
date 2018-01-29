// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestTimer.cs">
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
    /// A version of <see cref="System.Threading.Timer"/> that can be used for testing (without waiting a long time).
    /// </summary>
    public class TestTimer : IDisposable
    {

        private TimerCallback _timerCallback;
        private object _callbackState;

        public IDisposable SetCallback(TimerCallback timerCallback, object callbackState)
        {
            _timerCallback = timerCallback;
            _callbackState = callbackState;
            return this;
        }

        public void InvokeTimer()
        {
            _timerCallback?.Invoke(_callbackState);
        }

        public void Dispose()
        {
            _timerCallback = null;
            _callbackState = null;
        }
    }

}
