// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleEntryTypeLogWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System;
    using System.Collections.Generic;

    using LogJam.Trace;


    /// <summary>
    /// Common implementation for <see cref="ILogWriter" />s that can only have a single <typeparamref name="TEntry" /> type.
    /// </summary>
    /// <typeparamref name="TEntry">The log entry type</typeparamref>
    public abstract class SingleEntryTypeLogWriter<TEntry> : BaseLogWriter, IEntryWriter<TEntry>
        where TEntry : ILogEntry
    {

        protected SingleEntryTypeLogWriter(ITracerFactory setupTracerFactory)
            : base(setupTracerFactory)
        {}

        public override bool TryGetEntryWriter<TEntry1>(out IEntryWriter<TEntry1> entryWriter)
        {
            if (typeof(TEntry) == typeof(TEntry1))
            {
                entryWriter = (IEntryWriter<TEntry1>) this;
                return true;
            }
            else
            {
                entryWriter = null;
                return false;
            }
        }

        public override IEnumerable<KeyValuePair<Type, object>> EntryWriters { get { return new[] { new KeyValuePair<Type, object>(typeof(TEntry), this) }; } }

        #region IEntryWriter<TEntry>

        public virtual bool IsEnabled { get { return IsStarted; } }

        public abstract void Write(ref TEntry entry);

        #endregion
        #region Startable overrides

        /// <summary>
        /// Override <c>InternalStart</c> to NOT start the <see cref="EntryWriters"/>, since the <see cref="IEntryWriter{TEntry}"/> is <c>this</c>.
        /// </summary>
        protected override void InternalStart()
        {}

        /// <summary>
        /// Override <c>InternalStart</c> to NOT stop the <see cref="EntryWriters"/>, since the <see cref="IEntryWriter{TEntry}"/> is <c>this</c>.
        /// </summary>
        protected override void InternalStop()
        {}
        
        #endregion

    }

}
