// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startable.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;

namespace LogJam.Util
{

    /// <summary>
    /// Base class for <see cref="IStartable" /> implementations.
    /// </summary>
    public abstract class Startable : IStartable
    {

        private StartableState _startableState;

        protected Startable()
        {
            _startableState = StartableState.Unstarted;
        }

        /// @inheritdoc
        public StartableState State { get => _startableState; protected set => _startableState = value; }

        /// <summary>
        /// Returns <c>true</c> if this object is not started and is ready to be started.
        /// </summary>
        public virtual bool IsReadyToStart
        {
            get
            {
                var state = _startableState;
                return (state == StartableState.Unstarted) || (state == StartableState.Stopped);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this object is in a state that can be stopped.
        /// </summary>
        public virtual bool CanBeStopped
        {
            get
            {
                var state = _startableState;
                return (state != StartableState.Stopped)
                       && (state != StartableState.Stopping)
                       && (state != StartableState.Unstarted)
                       && (state < StartableState.Disposing);
            }
        }

        /// @inheritdoc
        public virtual void Start()
        {
            lock (this)
            {
                EnsureNotDisposed();
                if (IsReadyToStart)
                {
                    _startableState = StartableState.Starting;
                    try
                    {
                        InternalStart();
                        _startableState = StartableState.Started;
                    }
                    catch
                    {
                        _startableState = StartableState.FailedToStart;
                        throw;
                    }
                }
            }
        }

        /// @inheritdoc
        public void Stop()
        {
            lock (this)
            {
                if (CanBeStopped)
                {
                    _startableState = StartableState.Stopping;
                    try
                    {
                        InternalStop();
                        _startableState = StartableState.Stopped;
                    }
                    catch
                    {
                        _startableState = StartableState.FailedToStop;
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Can be overridden to provide logic that runs when the object is started.
        /// </summary>
        protected virtual void InternalStart()
        { }

        /// <summary>
        /// Can be overridden to provide logic that runs when the object is stopped.  <c>InternalStop</c> methods must do any
        /// cleanup necessary, and must be reliable even after startup failed.
        /// </summary>
        protected virtual void InternalStop()
        { }

        /// @inheritdoc
        /// <summary>
        /// Override ToString() to provide more descriptive start/stop logging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // This makes Start/Stop logging friendlier, but subclasses are welcome to provider a better ToString()
            return GetType().GetCSharpName();
        }

        protected void EnsureNotDisposed()
        {
            if (this.IsDisposed())
            {
                throw new ObjectDisposedException(ToString());
            }
        }

    }

}
