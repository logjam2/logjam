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
#if !NETSTANDARD
    using System.CodeDom.Compiler;
#endif
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using LogJam.Shared.Internal;
    using System.Collections.Generic;
    using System.Diagnostics;


    /// <summary>
    /// Reflection utility methods.
    /// </summary>
    internal static class TypeExtensions
    {

#if !NETSTANDARD
        private static readonly CodeDomProvider s_csharpCodeDomProvider = CodeDomProvider.CreateProvider("CSharp");

        /// <summary>
        /// Returns the CSharp name for a type. Provides more readable types than <see cref="Type.ToString()"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="omitTypeParameters">If <c>true</c>, type parameters are omitted from the returned name.</param>
        /// <returns></returns>
        public static string GetCSharpName(this Type type)
        {
            // Old implementation
            var reference = new System.CodeDom.CodeTypeReference(type);
            return s_csharpCodeDomProvider.GetTypeOutput(reference);
        }
#else

        private static IEnumerable<KeyValuePair<Type, ClassNameInfo>> s_csharpAliases = new KeyValuePair<Type, ClassNameInfo>[] {
                new KeyValuePair<Type, ClassNameInfo>(typeof(string), new ClassNameInfo("string")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(bool), new ClassNameInfo("bool")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(byte), new ClassNameInfo("byte")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(sbyte), new ClassNameInfo("sbyte")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(char), new ClassNameInfo("char")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(decimal), new ClassNameInfo("decimal")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(double), new ClassNameInfo("double")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(float), new ClassNameInfo("float")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(int), new ClassNameInfo("int")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(uint), new ClassNameInfo("uint")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(long), new ClassNameInfo("long")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(ulong), new ClassNameInfo("ulong")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(object), new ClassNameInfo("object")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(short), new ClassNameInfo("short")),
                new KeyValuePair<Type, ClassNameInfo>(typeof(ushort), new ClassNameInfo("ushort"))
        };

        /// <summary>
        /// Holds multiple forms of a csharp class name.
        /// </summary>
        private struct ClassNameInfo
        {
            public bool hasTypeParameters;
            public string nameWithTypeParameters;
            public string nameWithoutTypeParameters;

            public ClassNameInfo(string name)
            {
                hasTypeParameters = false;
                nameWithoutTypeParameters = name;
                nameWithTypeParameters = null;
            }
        }

        /// <summary>
        /// Type -> name cache with no generic type parameters.
        /// </summary>
        private static IDictionary<Type, ClassNameInfo> s_typeCSharpNames = new System.Collections.Concurrent.ConcurrentDictionary<Type, ClassNameInfo>(s_csharpAliases);

        /// <summary>
        /// Returns the CSharp name for a type. Provides more readable types than <see cref="Type.ToString()"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="omitTypeParameters">If <c>true</c>, type parameters are omitted from the returned name.</param>
        /// <returns></returns>
        public static string GetCSharpName(this Type type, bool omitTypeParameters = false)
        {
            ClassNameInfo classNameInfo;
            if (s_typeCSharpNames.TryGetValue(type, out classNameInfo))
            {
                if (! classNameInfo.hasTypeParameters)
                {
                    Debug.Assert(classNameInfo.nameWithoutTypeParameters != null);
                    return classNameInfo.nameWithoutTypeParameters;
                }
                if (omitTypeParameters)
                {
                    if (classNameInfo.nameWithoutTypeParameters != null)
                    {
                        return classNameInfo.nameWithoutTypeParameters;
                    }
                }
                else
                {
                    if (classNameInfo.nameWithTypeParameters != null)
                    {
                        return classNameInfo.nameWithTypeParameters;
                    }
                }
            }

            var typeInfo = type.GetTypeInfo();

            var sb = new StringBuilder();
            if (typeInfo.IsNested)
            {
                sb.Append(typeInfo.DeclaringType.GetCSharpName(omitTypeParameters));
            }
            else {
                sb.Append(typeInfo.Namespace);
            }
            if (sb.Length > 0)
            {
                sb.Append('.');
            }

            sb.Append(typeInfo.Name);
            var genericTypeArguments = typeInfo.GenericTypeArguments;
            int typeArgumentLength = genericTypeArguments.Length;
            int typeParameterCount = typeInfo.GenericTypeParameters.Length + typeArgumentLength;
            if (typeParameterCount > 0)
            {
                // Chop off the "`2" suffix
                int ich = sb.Length - 1;
                while ((ich > 0) && char.IsDigit(sb[ich])) { --ich; }
                if (sb[ich] == '`')
                {
                    sb.Length = ich;
                }

                if (! omitTypeParameters)
                {
                    // Add generic arguments
                    sb.Append('<');
                    for (int i = 0; i < typeParameterCount; ++i)
                    {
                        if (i > 0)
                        {
                            sb.Append(',');
                        }

                        Type typeArgument;
                        if ((i < typeArgumentLength) &&
                            ((typeArgument = genericTypeArguments[i]) != null))
                        {
                            if (i > 0)
                            {
                                sb.Append(' ');
                            }
                            sb.Append(typeArgument.GetCSharpName(omitTypeParameters));
                        }
                    }
                    sb.Append('>');
                }
            }

            // Cache for future use
            var typeName = sb.ToString();
            classNameInfo.hasTypeParameters = typeParameterCount > 0;
            if (omitTypeParameters || (typeParameterCount == 0))
            {
                classNameInfo.nameWithoutTypeParameters = typeName;
            }
            else
            {
                classNameInfo.nameWithTypeParameters = typeName;
            }
            s_typeCSharpNames[type] = classNameInfo;

            return typeName;
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
            if (!objectType.IsConstructedGenericType)
            {
                throw new ArgumentException($"{objectType} must be a constructed generic type", nameof(objectType));
            }
            if (!parameterizedType.GetTypeInfo().ContainsGenericParameters)
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
            Arg.DebugRequires(!objectType.ContainsGenericParameters, "objectType must not contain generic parameters");
            Arg.DebugRequires(parameterizedType.ContainsGenericParameters, "parameterizedType must contain generic parameters");

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

    }

}
