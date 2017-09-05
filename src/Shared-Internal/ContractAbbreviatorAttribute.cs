using System;
using System.Collections.Generic;
using System.Text;

namespace System.Diagnostics.Contracts
{
    using System.Diagnostics;


    /// <summary>
    /// Enables writing abbreviations for contracts that get copied to other methods. Has to be declared b/c not provided by System.Diagnostics.Contracts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Conditional("CODECONTRACTS")]
    internal sealed class ContractAbbreviatorAttribute : global::System.Attribute
    {}
}
