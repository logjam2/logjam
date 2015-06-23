// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializedTypeNameAttribute.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Schema
{
    using System;
    using System.Diagnostics.Contracts;


    /// <summary>
    /// Supports overriding the type namespace and name to use when a type is serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class SerializedTypeNameAttribute : Attribute
    {

        public SerializedTypeNameAttribute(string name, string @namespace = null)
        {
            Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(name));

            Name = name;
            Namespace = @namespace;
        }

        public string Name { get; private set; }

        public string Namespace { get; private set; }

    }

}