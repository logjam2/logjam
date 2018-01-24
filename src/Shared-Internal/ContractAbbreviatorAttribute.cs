// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContractAbbreviatorAttribute.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Shared.Internal
{
    using System;
    using System.Diagnostics;

// ContractAbbreviatorAttribute is provided in .NET 4.5 and newer
#if !CODECONTRACTS


    /// <summary>
    /// Conditional declaration (should evaluate to nothing when <c>CODECONTRACTS</c> is not defined) of <c>System.Diagnostics.Contracts.ContractAbbreviator</c>, so that
    /// all uses of this attribute don't have to be <c>#ifdef</c>'ed out.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Conditional("CODECONTRACTS")]
    internal sealed class ContractAbbreviatorAttribute : global::System.Attribute
    { }


#endif
}
