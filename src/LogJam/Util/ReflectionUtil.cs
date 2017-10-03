using LogJam.Shared.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LogJam.Util
{

    /// <summary>
    /// Reflection helper methods.
    /// </summary>
    internal static class ReflectionUtil
    {


        internal static object InvokeGenericMethod(this object objThis, Type[] typeArgs, string methodName, params object[] args)
        {
            Arg.NotNull(objThis, nameof(objThis));
            Arg.NotNullOrEmpty(typeArgs, nameof(typeArgs));
            Arg.NotNullOrWhitespace(methodName, nameof(methodName));

            for (var type = objThis.GetType().GetTypeInfo(); type != null; type = type.BaseType?.GetTypeInfo())
            {
                var genericMethodInfo = type.DeclaredMethods
                                            .SingleOrDefault(mi => !mi.IsStatic && string.Equals(mi.Name, methodName, StringComparison.Ordinal)
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
