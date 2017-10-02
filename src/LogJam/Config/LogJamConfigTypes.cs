// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamConfigTypes.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    using LogJam.Trace.Config;
    using LogJam.Trace.Format;
    using LogJam.Trace.Switches;


    /// <summary>
    /// Manages all the configuration types that are used during config file serialization and deserialization - needed so that
    /// CLR types can be determined from untyped JSON.
    /// When new classes are created that will be used in LogJam config files, they need to be registered via the
    /// <see cref="Register" /> method.
    /// </summary>
    public static class LogJamConfigTypes
    {

        private static readonly ISet<Type> s_configTypes = new HashSet<Type>();
        private static readonly ISet<Type> s_assignableTypes = new HashSet<Type>();

        private static readonly Type[] s_standardConfigTypes =
        {
            typeof(TraceManagerConfig),
            typeof(TraceWriterConfig),
            typeof(ThresholdTraceSwitch),
            typeof(OnOffTraceSwitch),
            typeof(DefaultTraceFormatter),
            typeof(DebuggerLogWriterConfig),
            typeof(ListLogWriterConfig<>),
            typeof(NoOpLogWriterConfig),
            typeof(ConsoleLogWriterConfig)
        };

        static LogJamConfigTypes()
        {
            foreach (Type configType in s_standardConfigTypes)
            {
                Register(configType);
            }
        }

        public static void Register(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null);

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsAbstract || typeInfo.IsInterface || typeInfo.IsValueType)
            {
                throw new ArgumentException("Only instantiable reference types, or generic type definitions, can be registered.", "type");
            }

            lock (s_configTypes)
            {
                s_configTypes.Add(type);

                RegisterAssignableTypesFor(type);
            }
        }

        private static void RegisterAssignableTypesFor(Type type)
        {
            if (type == typeof(object))
            {
                return;
            }

            s_assignableTypes.Add(type);
            var typeInfo = type.GetTypeInfo();
            s_assignableTypes.UnionWith(typeInfo.ImplementedInterfaces);
            RegisterAssignableTypesFor(typeInfo.BaseType);
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="type" /> is a valid config type, or interface.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsRegisteredAssignableType(Type type)
        {

            return s_assignableTypes.Contains(type);
        }

        /// <summary>
        /// Returns all registered concrete types that can be assigned to properties of type <paramref name="baseType" />.
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetConcreteTypesFor(Type baseType)
        {
            Contract.Requires<ArgumentNullException>(baseType != null);

            var baseTypeInfo = baseType.GetTypeInfo();
            if (baseTypeInfo.ContainsGenericParameters)
            {
                throw new ArgumentException("Cannot determine concrete types for an open generic type.", "baseType");
            }

            List<Type> concreteTypes = new List<Type>();
            // Add all derived concrete types
            concreteTypes.AddRange(s_configTypes.Where(configType =>
                                                       {
                                                           var configTypeInfo = configType.GetTypeInfo();
                                                           return ! configTypeInfo.ContainsGenericParameters && baseTypeInfo.IsAssignableFrom(configTypeInfo);
                                                       }));

            if (baseTypeInfo.IsGenericType)
            {

                var gtd = baseType.GetGenericTypeDefinition();
                var gtdMatches = s_configTypes.Where(configType =>
                                                     {
                                                         var configTypeInfo = configType.GetTypeInfo();
                                                         return configTypeInfo.IsGenericType && gtd.GetTypeInfo().IsAssignableFrom(configTypeInfo);
                                                     });

                if (gtdMatches.Any())
                { // Try direct ordered application of type parameters from the base type to the config type
                    Type[] typeParameters = baseTypeInfo.GenericTypeArguments;

                    foreach (Type gtdMatch in gtdMatches)
                    {
                        try
                        {
                            var filledType = gtdMatch.MakeGenericType(typeParameters);
                            if (! filledType.GetTypeInfo().ContainsGenericParameters)
                            {
                                concreteTypes.Add(filledType);
                            }
                        }
                        catch
                        {} // This is valid; .MakeGenericType() could fail.
                    }
                }
            }

            return concreteTypes;
        }

    }

}
