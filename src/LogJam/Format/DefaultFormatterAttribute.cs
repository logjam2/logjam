// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultFormatterAttribute.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Format
{
    using System;
    using System.Diagnostics.Contracts;


    /// <summary>
    /// Can be used to specify the default <see cref="EntryFormatter{TEntry}"/> for a log entry type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public sealed class DefaultFormatterAttribute : Attribute
    {

        private readonly Type _formatterType;

        public DefaultFormatterAttribute(Type formatterType)
        {
            Contract.Requires<ArgumentNullException>(formatterType != null);

            _formatterType = formatterType;
        }

        /// <summary>
        /// Returns the <see cref="Type"/> specified in the constructor.
        /// </summary>
        public Type FormatterType { get { return _formatterType; } }

        /// <summary>
        /// Returns the default formatter for <typeparamref name="TEntry"/>, if one is specified as a <see cref="DefaultFormatterAttribute"/>.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <returns>The default formatter, or <c>null</c> if no default formatter is specified on the entry type.</returns>
        public static EntryFormatter<TEntry> GetDefaultFormatterFor<TEntry>()
            where TEntry : ILogEntry
        {
            Type entryType = typeof(TEntry);
            var attributes = entryType.GetCustomAttributes(typeof(DefaultFormatterAttribute), inherit: true);
            if (attributes.Length == 0)
            {
                return null;
            }

            var defaultFormatterAttribute = (DefaultFormatterAttribute) attributes[0];
            Type formatterType = defaultFormatterAttribute.FormatterType;
            if (!typeof(EntryFormatter<TEntry>).IsAssignableFrom(formatterType))
            {
                throw new LogJamSetupException(string.Format("FormatterType={0} cannot be converted to EntryFormatter<{1}> - check [DefaultFormatter] attribute on {1}",
                                                             formatterType.FullName,
                                                             entryType.FullName), defaultFormatterAttribute);
            }

            return Activator.CreateInstance(formatterType) as EntryFormatter<TEntry>;
        }

    }

}