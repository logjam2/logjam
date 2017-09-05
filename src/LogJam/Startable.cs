// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startable.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;

    using LogJam.Util;


    /// <summary>
    /// Base class for <see cref="IStartable" /> implementations.
    /// </summary>
    public abstract class Startable : IStartable
    {

        private StartableState _startableState;

        /// <summary>
        /// Initializes this <see cref="Startable"/> to <see cref="StartableState.Unstarted"/>.
        /// </summary>
        protected Startable()
        {
            _startableState = StartableState.Unstarted;
        }

        /// @inheritdoc
        public StartableState State { get { return _startableState; } protected set { _startableState = value; } }

        /// @inheritdoc
        public bool IsStarted { get { return _startableState == StartableState.Started; } }

        /// <summary>
        /// Returns <c>true</c> if this object is in a state that can be <see cref="Start"/>ed.
        /// </summary>
        public virtual bool ReadyToStart
        {
            get
            {
                var state = _startableState;
                return ((state == StartableState.Unstarted) || (state == StartableState.Stopped));
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this object is being disposed or has been disposed.
        /// </summary>
        protected bool IsDisposed { get { return _startableState >= StartableState.Disposing; } }

        /// @inheritdoc
        public virtual void Start()
        {
            lock (this)
            {
                if (IsStarted || (_startableState == StartableState.Starting))
                { // Do nothing if already started or starting.
                    return;
                }
                if (_startableState >= StartableState.Disposing)
                { // Do nothing if already started.
                    throw new ObjectDisposedException(this.ToString(), "Cannot be started; state is: " + _startableState);
                }
                if (! ReadyToStart)
                {
                    throw new LogJamStartException(this + " cannot be started; state is: " + _startableState, this);
                }
                _startableState = StartableState.Starting;
            }

            try
            {
                InternalStart();
                _startableState = StartableState.Started;
            }
            catch (LogJamStartException)
            {
                _startableState = StartableState.FailedToStart;
                throw;
            }
            catch (Exception exception)
            {
                _startableState = StartableState.FailedToStart;
                throw new LogJamStartException("Exception starting " + this, exception, this);
            }
        }

        /// <summary>
        /// Can be overridden to provide logic that runs when the object is started.
        /// </summary>
        protected virtual void InternalStart()
        {}

        /// @inheritdoc
        public void Stop()
        {
            lock (this)
            {
                if (! IsStarted)
                {
                    return;
                }
                _startableState = StartableState.Stopping;
            }

            try
            {
                InternalStop();
                _startableState = StartableState.Stopped;
            }
            catch (Exception exception)
            {
                _startableState = StartableState.FailedToStop;
                throw new LogJamException("Exception stopping " + this, exception, this);
            }
        }

        /// <summary>
        /// Can be overridden to provide logic that runs when the object is stopped.
        /// </summary>
        protected virtual void InternalStop()
        {}

        /// <summary>
        /// Override ToString() to provide more descriptive start/stop logging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // This makes Start/Stop logging friendlier, but subclasses are welcome to provider a better ToString()
            return GetType().GetCSharpName();
        }

    }

}
