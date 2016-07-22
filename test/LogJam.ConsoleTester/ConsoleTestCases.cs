// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTestCases.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.ConsoleTester
{
    using LogJam.Test.Shared;
    using LogJam.Trace;


    /// <summary>
    /// Some logging test cases.
    /// </summary>
    public sealed class ConsoleTestCases : TraceTestCases
    {

        public ConsoleTestCases(TraceManager traceManager)
            : base(traceManager)
        {}

    }

}
