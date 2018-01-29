// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStartable.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;


    /// <summary>
    /// Interface for LogJam objects that can be started, stopped, and re-started.
    /// </summary>
    public interface IStartable
    {

        /// <summary>
        /// Starts the object - after <c>Start()</c> is called, the object is active and should operate normally.
        /// <c>Start()</c> must be called after the object is created to prepare it for operation.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If this object was previously disposed.</exception>
        /// <exception cref="LogJamStartException">
        /// If the object or its children cannot be started. If the problem is corrected, <c>Start()</c> can
        /// be called again.
        /// </exception>
        void Start();

        /// <summary>
        /// Stops the object - after <c>Stop()</c> is called, the object is considered inactive. In many cases
        /// <see cref="Start" /> can be called after <c>Stop()</c> to re-activate the object.
        /// </summary>
        /// <remarks>
        /// Calls to <see cref="Stop"/> should not throw exceptions.
        /// </remarks>
        void Stop();

        /// <summary>
        /// The <see cref="StartableState"/> representing the current state of the object within the <see cref="IStartable"/> lifecycle.
        /// </summary>
        StartableState State { get; }

        /// <summary>
        /// Returns <c>true</c> if the object is ready to be <see cref="Start"/>ed.
        /// </summary>
        bool IsReadyToStart { get; }

    }

}
