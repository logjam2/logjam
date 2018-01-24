// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartableStateChangingEventArgs.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;

    using LogJam.Trace;


    /// <summary>
    /// Event args for when <see cref="LogManager"/> or <see cref="TraceManager"/> state changes.
    /// </summary>
    public sealed class StartableStateChangingEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new <see cref="StartableStateChangingEventArgs"/> instance.
        /// </summary>
        /// <param name="priorState"></param>
        /// <param name="newState"></param>
        public StartableStateChangingEventArgs(StartableState priorState, StartableState newState)
        {
            PriorState = priorState;
            NewState = newState;
        }

        /// <summary>
        /// The prior state.
        /// </summary>
        public StartableState PriorState { get; }

        /// <summary>
        /// The new state.
        /// </summary>
        public StartableState NewState { get; }

    }

}
