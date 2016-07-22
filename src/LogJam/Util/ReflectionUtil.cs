// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionUtil.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;


    /// <summary>
    /// Reflection utility methods.
    /// </summary>
    internal static class ReflectionUtil
    {

        private static readonly CodeDomProvider s_csharpCodeDomProvider = CodeDomProvider.CreateProvider("CSharp");

        public static string GetCSharpName(this Type type)
        {
            var reference = new System.CodeDom.CodeTypeReference(type);
            return s_csharpCodeDomProvider.GetTypeOutput(reference);
        }

        internal static Type[] GetGenericTypeArgumentsFor(this Type objectType, Type parameterizedType)
        {
            Contract.Requires<ArgumentNullException>(objectType != null);
            Contract.Requires<ArgumentNullException>(parameterizedType != null);
            Contract.Requires<ArgumentException>(! objectType.ContainsGenericParameters);
            Contract.Requires<ArgumentException>(parameterizedType.ContainsGenericParameters);

            var genericObjectType = objectType.GetGenericTypeDefinition();
            if (parameterizedType == genericObjectType)
            {
                return genericObjectType.GetGenericArguments();
            }

            if (parameterizedType.IsInterface)
            {
                var matchingInterfaces =
                    objectType.GetInterfaces().Where(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == parameterizedType).ToArray();
                if (matchingInterfaces.Length == 0)
                {
                    return null;
                }
                else if (matchingInterfaces.Length == 1)
                {
                    return matchingInterfaces[0].GetGenericArguments();
                }
                else
                {
                    throw new ArgumentException(string.Format("Object {0} implements generic interface {1} more than once.", objectType, parameterizedType));
                }
            }
            else
            {
                // Recurse
                var baseType = objectType.BaseType;
                if (baseType != null)
                {
                    return GetGenericTypeArgumentsFor(baseType, parameterizedType);
                }
                else
                {
                    return null;
                }
            }
        }

        internal static object InvokeGenericMethod(this object objThis, Type[] typeArgs, string methodName, params object[] args)
        {
            Contract.Requires<ArgumentNullException>(objThis != null);
            Contract.Requires<ArgumentNullException>(typeArgs != null);
            Contract.Requires<ArgumentException>(! string.IsNullOrEmpty(methodName));

            var genericMethodInfo =
                objThis.GetType()
                       .GetMember(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                       .OfType<MethodInfo>()
                       .SingleOrDefault(mi => mi.IsGenericMethodDefinition && mi.GetGenericArguments().Length == typeArgs.Length);
            if (genericMethodInfo == null)
            {
                throw new MissingMemberException(objThis.GetType().GetCSharpName(), methodName);
            }

            var methodInfo = genericMethodInfo.MakeGenericMethod(typeArgs);
            return methodInfo.Invoke(objThis, args);
        }

    }

}
