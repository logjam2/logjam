// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredAttribute.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Schema
{
    using System;


    /// <summary>
    /// Labels a field or property in a log entry as required (may not be null) or nullable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RequiredAttribute : Attribute
    {
        public RequiredAttribute()
            : this(true)
        {}

        public RequiredAttribute(bool isRequired)
        {
            IsRequired = isRequired;
        }

        /// <summary>
        /// If <c>false</c>, the field or property may be <c>null</c>.  If <c>true</c>, the field or property may not be null.
        /// </summary>
        public bool IsRequired { get; private set; }

    }

}