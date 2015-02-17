// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionUtil.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{
	using System;
	using System.CodeDom.Compiler;
	using System.Diagnostics.Contracts;
	using System.Linq;


	/// <summary>
	/// Reflection utility methods.
	/// </summary>
	internal static class ReflectionUtil
	{
		private static CodeDomProvider csharpCodeDomProvider = CodeDomProvider.CreateProvider("CSharp");

		public static string GetCSharpName(this Type type)
		{
			var reference = new System.CodeDom.CodeTypeReference(type);
			return csharpCodeDomProvider.GetTypeOutput(reference);
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
				var matchingInterfaces = objectType.GetInterfaces().Where(interfaceType => interfaceType.GetGenericTypeDefinition() == parameterizedType).ToArray();
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

	}

}