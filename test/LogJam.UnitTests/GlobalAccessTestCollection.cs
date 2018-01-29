// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalAccessTestCollection.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using LogJam.Trace;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LogJam.UnitTests
{

    /// <summary>
    /// Defines the set of tests that access globals like <see cref="LogManager.Instance"/> and <see cref="TraceManager.Instance"/>, and thus should not
    /// be run in parallel.
    /// </summary>
    [CollectionDefinition(nameof(GlobalAccessTestCollection))]
    public sealed class GlobalAccessTestCollection
    { }

}
