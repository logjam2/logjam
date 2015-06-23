// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvroEntryWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode.Avro
{
    using System;

    using LogJam.Writer;


    /// <summary>
    /// Standard entry writer for Avro logging.  Logs entries of type <typeparamref name="TEntry"/> to the 
    /// <see cref="AvroLogWriter"/>.
    /// </summary>
    /// <remarks>The default behavior is for the received entry to be written directly to the Avro serializer.  However, 
    /// this class can be subclassed to wrap or replace the entry before it is written to Avro, so a different entry type
    /// can be used alltogether.  To do this, override <see cref="Write"/> and <see cref="LogEntryType"/>, and register the type
    /// with <see cref="AvroLogWriter.RegisterEntryWriterFactory"/>.</remarks>
    /// <typeparam name="TEntry">The strongly-typed log entry type that this <see cref="IEntryWriter{TEntry}"/> receives.</typeparam>
    public class AvroEntryWriter<TEntry> : IEntryWriter<TEntry>, IAvroLogEntryTypeInfo
        where TEntry : ILogEntry
    {

        private readonly AvroLogWriter _parent;

        public AvroEntryWriter(AvroLogWriter parent)
        {
            _parent = parent;
        }

        public virtual void Write(ref TEntry entry)
        {
            WriteAvroLogEntry(entry);
        }

        protected void WriteAvroLogEntry(ILogEntry entry)
        {
            _parent.WriteAvroLogEntry(entry);
        }

        public bool IsEnabled { get { return _parent.IsEnabled; } }

        internal AvroLogWriter Parent { get { return _parent; } }

        #region IAvroLogEntryTypeInfo
        
        /// <summary>
        /// Returns the type that is logged to Avro.
        /// </summary>
        public virtual Type LogEntryType { get { return typeof(TEntry); } }

        #endregion

    }

}