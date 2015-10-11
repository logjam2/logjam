// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntryFormatter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Format
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;


    /// <summary>
    /// Definition of log entry formatters, which format <typeparamref name="TEntry" /> objects into a text representation.
    /// </summary>
    /// <typeparam name="TEntry">The log entry type.</typeparam>
    public abstract class EntryFormatter<TEntry>
        where TEntry : ILogEntry
    {
        #region Public Methods and Operators

        /// <summary>
        /// Format <paramref name="entry" /> and write output to <paramref name="textWriter" />.
        /// </summary>
        /// <param name="entry">A log entry to format.</param>
        /// <param name="textWriter">The text writer that receives formatted log output</param>
        public abstract void Format(ref TEntry entry, TextWriter textWriter);

        /// <summary>
        /// Formats <paramref name="entry" /> and returns the formatted result as a <see cref="string" />.
        /// </summary>
        /// <param name="entry">A log entry to format.</param>
        /// <returns>A string containing the formatted log entry.</returns>
        public virtual string Format(ref TEntry entry)
        {
            StringWriter sw = new StringWriter();
            Format(ref entry, sw);
            return sw.ToString();
        }

        /// <summary>
        /// Provides automatic conversion from <see cref="FormatAction{TEntry}" /> to <see cref="EntryFormatter{TEntry}" />.
        /// </summary>
        /// <param name="formatAction">A <see cref="FormatAction{TEntry}" /></param>
        /// <returns>A <see cref="EntryFormatter{TEntry}" /> that calls <paramref name="formatAction" /> to format text.</returns>
        public static explicit operator EntryFormatter<TEntry>(FormatAction<TEntry> formatAction)
        {
            Contract.Requires<ArgumentNullException>(formatAction != null);

            return new ActionFormatter<TEntry>(formatAction);
        }

        #endregion
    }


    /// <summary>
    /// Signature for format actions, which can be used in place of <see cref="EntryFormatter{TEntry}" />.
    /// </summary>
    /// <typeparam name="TEntry">The log entry type.</typeparam>
    /// <param name="entry">A log entry to format.</param>
    /// <param name="textWriter">The text writer that receives formatted log output.</param>
    /// <remarks>
    /// Note that subclassing <see cref="EntryFormatter{TEntry}" /> is more efficient for value-typed
    /// <c>TEntry</c>, because the log entry is not copied.  In this delegate, <paramref name="entry" />
    /// is not a <c>ref</c> parameter to allow lambda functions to be used for formatting.
    /// </remarks>
    public delegate void FormatAction<in TEntry>(TEntry entry, TextWriter textWriter)
        where TEntry : ILogEntry;

}
