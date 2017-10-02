// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs">
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
    using System.Text;

    using LogJam.Shared.Internal;
    using System.Collections.Generic;


    /// <summary>
    /// Reflection utility methods.
    /// </summary>
    internal static class TypeExtensions
    {

#if !NETSTANDARD
        private static readonly CodeDomProvider s_csharpCodeDomProvider = CodeDomProvider.CreateProvider("CSharp");

        public static string GetCSharpName(this Type type)
        {

            // Old implementation
            var reference = new System.CodeDom.CodeTypeReference(type);
            return s_csharpCodeDomProvider.GetTypeOutput(reference);
        }
#else

        private static IEnumerable<KeyValuePair<Type, string>> s_csharpAliases = new KeyValuePair<Type, string>[] {
                new KeyValuePair<Type, string>(typeof(string), "string"),
                new KeyValuePair<Type, string>(typeof(bool), "bool" ),
                new KeyValuePair<Type, string>(typeof(byte), "byte" ),
                new KeyValuePair<Type, string>(typeof(sbyte), "sbyte" ),
                new KeyValuePair<Type, string>(typeof(char), "char" ),
                new KeyValuePair<Type, string>(typeof(decimal), "decimal" ),
                new KeyValuePair<Type, string>(typeof(double), "double" ),
                new KeyValuePair<Type, string>(typeof(float), "float" ),
                new KeyValuePair<Type, string>(typeof(int), "int" ),
                new KeyValuePair<Type, string>(typeof(uint), "uint" ),
                new KeyValuePair<Type, string>(typeof(long), "long" ),
                new KeyValuePair<Type, string>(typeof(ulong), "ulong" ),
                new KeyValuePair<Type, string>(typeof(object), "object" ),
                new KeyValuePair<Type, string>(typeof(short), "short" ),
                new KeyValuePair<Type, string>(typeof(ushort), "ushort" )
};

        private static IDictionary<Type, string> s_typeCSharpNames = new System.Collections.Concurrent.ConcurrentDictionary<Type, string>(s_csharpAliases);


        /// <summary>
        /// Returns the CSharp name for a type. Provides more readable types than <see cref="Type.ToString()"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetCSharpName(this Type type)
        {
            string typeName;
            if (s_typeCSharpNames.TryGetValue(type, out typeName))
            {
                return typeName;
            }

            var sb = new StringBuilder();
            sb.Append(type.Namespace);
            if (sb.Length > 0)
            {
                sb.Append('.');
            }
            sb.Append(type.Name);
            return sb.ToString();
        }
#endif

        /// <summary>
        /// Returns the generic type arguments for <paramref name="parameterizedType"/> that were used
        /// to create <paramref name="objectType"/>.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="parameterizedType"></param>
        /// <returns></returns>
        internal static Type[] GetGenericTypeArgumentsFor(this Type objectType, Type parameterizedType)
        {
            Arg.DebugNotNull(objectType, nameof(objectType));
            Arg.DebugNotNull(parameterizedType, nameof(parameterizedType));
#if DEBUG
            if (! objectType.IsConstructedGenericType)
            {
                throw new ArgumentException($"{objectType} must be a constructed generic type", nameof(objectType));
            }
            if (! parameterizedType.GetTypeInfo().ContainsGenericParameters)
            {
                throw new ArgumentException($"{parameterizedType} must have generic type parameters", nameof(parameterizedType));
            }
#endif

            var genericObjectType = objectType.GetGenericTypeDefinition();
            if (parameterizedType == genericObjectType)
            {
                return objectType.GenericTypeArguments;
            }

            return GetGenericTypeArgumentsFor(objectType.GetTypeInfo(), parameterizedType.GetTypeInfo());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="parameterizedType"></param>
        /// <returns></returns>
        internal static Type[] GetGenericTypeArgumentsFor(this TypeInfo objectType, TypeInfo parameterizedType)
        {
            Arg.DebugNotNull(objectType, nameof(objectType));
            Arg.DebugNotNull(parameterizedType, nameof(parameterizedType));
#if DEBUG
#if NETSTANDARD
            Contract.Requires<ArgumentException>(!objectType.ContainsGenericParameters);
            Contract.Requires<ArgumentException>(parameterizedType.ContainsGenericParameters);
#else
            Contract.Requires<ArgumentException>(! objectType.ContainsGenericParameters);
            Contract.Requires<ArgumentException>(parameterizedType.ContainsGenericParameters);
#endif
#endif

            var genericObjectType = objectType.GetGenericTypeDefinition()?.GetTypeInfo();
            if (parameterizedType == genericObjectType)
            {
                return genericObjectType.GenericTypeArguments;
            }

            if (parameterizedType.IsInterface)
            {
                var matchingInterfaces =
                    objectType.ImplementedInterfaces.Where(interfaceType => interfaceType.GetTypeInfo().IsGenericType
                    && interfaceType.GetGenericTypeDefinition().GetTypeInfo() == parameterizedType).ToArray();
                if (matchingInterfaces.Length == 0)
                {
                    return null;
                }
                else if (matchingInterfaces.Length == 1)
                {
                    return matchingInterfaces[0].GenericTypeArguments;
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
                    return GetGenericTypeArgumentsFor(baseType.GetTypeInfo(), parameterizedType);
                }
                else
                {
                    return null;
                }
            }
        }

        internal static object InvokeGenericMethod(this object objThis, Type[] typeArgs, string methodName, params object[] args)
        {
            Arg.NotNull(objThis, nameof(objThis));
            Arg.NotNullOrEmpty(typeArgs, nameof(typeArgs));
            Arg.NotNullOrWhitespace(methodName, nameof(methodName));

            for (var type = objThis.GetType().GetTypeInfo(); type != null; type = type.BaseType?.GetTypeInfo())
            {
                var genericMethodInfo = type.DeclaredMethods
                                            .SingleOrDefault(mi => ! mi.IsStatic && string.Equals(mi.Name, methodName, StringComparison.Ordinal)
                                                                   && mi.IsGenericMethodDefinition && mi.GetGenericArguments().Length == typeArgs.Length);
                if (genericMethodInfo != null)
                {
                    var methodInfo = genericMethodInfo.MakeGenericMethod(typeArgs);
                    return methodInfo.Invoke(objThis, args);
                }

            }
            throw new MissingMemberException($"Matching method {methodName} not found on type {objThis.GetType().GetCSharpName()}");
        }

    }

}
