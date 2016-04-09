// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionUtil.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Test.Shared
{
    using System;
    using System.CodeDom.Compiler;


    /// <summary>
    /// Reflection utility methods
    /// </summary>
    public static class ReflectionUtil
    {

        private static readonly CodeDomProvider s_csharpCodeDomProvider = CodeDomProvider.CreateProvider("CSharp");

        public static string GetCSharpName(this Type type)
        {
            var reference = new System.CodeDom.CodeTypeReference(type);
            return s_csharpCodeDomProvider.GetTypeOutput(reference);
        }

    }
}
