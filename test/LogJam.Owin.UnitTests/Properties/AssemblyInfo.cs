// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Reflection;

using Xunit;

[assembly: AssemblyTitle("LogJam.Owin.UnitTests")]
[assembly: AssemblyDescription("Unit tests for the LogJam.Owin integration project.")]

// Unit test projects don't need to be CLS compliant

[assembly: CLSCompliant(false)]

// Disable test parallelization for this assembly

[assembly: CollectionBehavior(DisableTestParallelization = true)]
