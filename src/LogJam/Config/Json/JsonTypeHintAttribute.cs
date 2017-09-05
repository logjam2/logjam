// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonTypeHintAttribute.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using LogJam.Shared.Internal;


    /// <summary>
    /// An attribute for classes to add a JSON property + value during JSON serialization; and
    /// use the same property + value during deserialization to determine the entity type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class JsonTypeHintAttribute : Attribute
    {

        /// <summary>
        /// Creates a new <see cref="JsonTypeHintAttribute" /> using the specified <paramref name="property" /> and
        /// <paramref name="value" />.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public JsonTypeHintAttribute(string property, string value)
        {
            Arg.NotNullOrWhitespace(property, nameof(property));

            Property = property;
            Value = value;
        }

        /// <summary>
        /// Returns the property name for this <see cref="JsonTypeHintAttribute" />.
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// Returns the property value for this <see cref="JsonTypeHintAttribute" />.
        /// </summary>
        public string Value { get; }

        #region Equality/hash support

        private bool Equals(JsonTypeHintAttribute other)
        {
            return other != null
                   && string.Equals(Property, other.Property, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is JsonTypeHintAttribute && Equals((JsonTypeHintAttribute) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 397 ^ (Property != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Property) : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Value) : 0);
                return hashCode;
            }
        }

        #endregion

        /// <summary>
        /// Returns the <see cref="JsonTypeHintAttribute" />s attached to the specified <paramref name="type" />.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<JsonTypeHintAttribute> For(Type type)
        {
            Arg.NotNull(type, nameof(type));

            return type.GetCustomAttributes(typeof(JsonTypeHintAttribute), false).Cast<JsonTypeHintAttribute>();
        }

    }

}
