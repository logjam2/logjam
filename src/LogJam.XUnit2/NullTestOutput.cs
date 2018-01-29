// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullTestOutput.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
    using Xunit.Abstractions;

    /// <summary>
    /// An <see cref="ITestOutputHelper"/> that does nothing.
    /// </summary>
    internal sealed class NullTestOutput : ITestOutputHelper
    {

        /// <summary>Adds a line of text to the output.</summary>
        /// <param name="message">The message</param>
        public void WriteLine(string message)
        {}

        /// <summary>Formats a line of text and adds it to the output.</summary>
        /// <param name="format">The message format</param>
        /// <param name="args">The format arguments</param>
        public void WriteLine(string format, params object[] args)
        {}

    }

}