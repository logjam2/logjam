// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStartable.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{

    /// <summary>
    /// Interface for LogJam objects that can be started, stopped, and re-started.
    /// </summary>
    public interface IStartable
    {

        /// <summary>
        /// Starts the object - after <c>Start()</c> is called, the object is active and should operate normally.
        /// <c>Start()</c> must be called after the object is created to prepare it for operation.
        /// </summary>
        /// <exception cref="LogJamStartException">
        /// If activation cannot complete.  If the problem is corrected, <c>Start()</c> can
        /// be called again.
        /// </exception>
        void Start();

        /// <summary>
        /// Stops the object - after <c>Stop()</c> is called, the object is considered inactive.  In many cases
        /// <see cref="Start" /> can be called after <c>Stop()</c> to re-activate the object.
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns <c>true</c> when the object has been successfully <see cref="Start" />ed, and has not yet been
        /// <see cref="Stop" />ped.
        /// </summary>
        bool IsStarted { get; }

    }

}
