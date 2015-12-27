// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntryFormatter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;
    using System.Diagnostics.Contracts;


    /// <summary>
    /// Base class for log entry formatters, which write <typeparamref name="TEntry" /> objects to a <see cref="FormatWriter"/>.
    /// </summary>
    /// <typeparam name="TEntry">The log entry type.</typeparam>
    public abstract class EntryFormatter<TEntry>
        where TEntry : ILogEntry
    {
        #region Public Methods and Operators

        /// <summary>
        /// Format <paramref name="entry" /> and write output to <paramref name="formatWriter" />.
        /// </summary>
        /// <param name="entry">A log entry to format.</param>
        /// <param name="formatWriter">The <see cref="FormatWriter"/> that receives formatted log output</param>
        public abstract void Format(ref TEntry entry, FormatWriter formatWriter);

        /// <summary>
        /// Provides automatic conversion from <see cref="EntryFormatActionrmatAction{TEntry}" /> to <see cref="EntryFormatter{TEntry}" />.
        /// </summary>
        /// <param name="formatAction">A <see cref="EntryFormatActionrmatAction{TEntry}" /></param>
        /// <returns>A <see cref="EntryFormatter{TEntry}" /> that calls <paramref name="formatAction" /> to format text.</returns>
        public static explicit operator EntryFormatter<TEntry>(EntryFormatAction<TEntry> formatAction)
        {
            Contract.Requires<ArgumentNullException>(formatAction != null);

            return new EntryActionFormatter<TEntry>(formatAction);
        }

        #endregion
    }


    /// <summary>
    /// Signature for entry format actions, which can be used in place of <see cref="EntryFormatter{TEntry}" />.
    /// </summary>
    /// <typeparam name="TEntry">The log entry type.</typeparam>
    /// <param name="entry">A log entry to format.</param>
    /// <param name="writer">The <see cref="FormatWriter"/> that receives formatted log output.</param>
    /// <remarks>
    /// Note that subclassing <see cref="EntryFormatter{TEntry}" /> is more efficient for value-typed
    /// <c>TEntry</c>, because the log entry is not copied.  In this delegate, <paramref name="entry" />
    /// is not a <c>ref</c> parameter to allow lambda functions to be used for formatting.
    /// </remarks>
    public delegate void EntryFormatAction<in TEntry>(TEntry entry, FormatWriter writer)
        where TEntry : ILogEntry;

}
