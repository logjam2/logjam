// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartableExtensions.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{

    /// <summary>
    /// Extension methods for <see cref="IStartable"/> objects.
    /// </summary>
    public static class StartableExtensions
    {

        /// <summary>
        /// Returns <c>true</c> if <paramref name="startable"/> has successfully started.
        /// </summary>
        /// <param name="startable"></param>
        /// <returns></returns>
        public static bool IsStarted(this IStartable startable)
        {
            return startable?.State == StartableState.Started;
        }


        /// <summary>
        /// Returns <c>true</c> if <paramref name="startable"/> is being disposed or has been disposed.
        /// </summary>
        public static bool IsDisposed(this IStartable startable)
        {
            return startable?.State >= StartableState.Disposing;
        }

    }

}
