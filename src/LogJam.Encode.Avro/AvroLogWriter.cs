// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvroLogWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode.Avro
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;

    using LogJam.Encode.Avro.Trace;
    using LogJam.Trace;
    using LogJam.Writer;

    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Container;


    /// <summary>
    /// Writes a series of <see cref="LogRequest"/>s to an Avro Container stream, or an avro binary log file.
    /// </summary>
    public sealed class AvroLogWriter : BaseLogWriter
    {

        /// <summary>
        /// A factory for custom <see cref="AvroEntryWriter{TEntry}"/>s for specific log entry types.
        /// </summary>
        private static IDictionary<Type, Func<AvroLogWriter, object>> s_entryWriterFactories = new Dictionary<Type, Func<AvroLogWriter, object>>()
                                                                                            {
                                                                                                { typeof(TraceEntry), (parent) => new AvroTraceEntryWriter(parent) }
                                                                                            };

        /// <summary>
        /// Set of default known types for Avro serializer construction.
        /// </summary>
        private static Type[] s_defaultKnownTypes = {
                                                        typeof(ILogEntry),
                                                        typeof(ITimestampedLogEntry),
                                                        typeof(LogRequestHeader),
                                                        typeof(ApplicationEntry)
                                                    };

        private Stream _stream;
        private readonly bool _disposeStream;

        private IAvroWriter<ILogEntry> _avroContainerWriter;
        private SequentialWriter<ILogEntry> _sequentialWriter;


        public static void RegisterEntryWriterFactory<TEntry>(Func<AvroLogWriter, AvroEntryWriter<TEntry>> createEntryWriter)
            where TEntry : ILogEntry 
        {
            Contract.Requires<ArgumentNullException>(createEntryWriter != null);

            s_entryWriterFactories[typeof(TEntry)] = createEntryWriter;
        }

        public AvroLogWriter(ITracerFactory setupTracerFactory)
            : base(setupTracerFactory)
        {
            AddEntryType<LogRequestHeader>();
            AddEntryType<ApplicationEntry>();
        }


        public AvroLogWriter(ITracerFactory setupTracerFactory, Stream stream, bool disposeStream = true)
            : this(setupTracerFactory)
        {
            OutputStream = stream;
            _disposeStream = disposeStream;
        }

        public AvroLogWriter(ITracerFactory setupTracerFactory, FileInfo file)
            : this(setupTracerFactory)
        {
            Contract.Requires<ArgumentNullException>(file != null);

            SetOutputFile(file);
            _disposeStream = true;
        }

        public Stream OutputStream
        {
            get { return _stream; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                if (_stream == value)
                {
                    return;
                }
                if (IsStarted)
                {
                    throw new LogWriterException("Cannot set AvroLogWriter.OutputStream while the logwriter is started.", this);
                }
                _stream = value;
            }
        }

        public void SetOutputFile(FileInfo file)
        {
            Contract.Requires<ArgumentNullException>(file != null);

            if (IsStarted)
            {
                throw new LogWriterException("Cannot set AvroLogWriter OutputFile while the logwriter is started.", this);
            }

            if (file.Exists)
            {
                if (file.Length == 0)
                {
                    _stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Write, FileShare.None);
                }
                else
                {
                    throw new LogWriterException("AvroLogWriter cannot append to an existing file. File exists: " + file.FullName, this);
                }
            }
            else
            {
                _stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write, FileShare.None);
            }
        }

        /// <summary>
        /// Adds entry type <typeparamref name="TEntry"/> to the schema for this <see cref="AvroLogWriter"/>.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <returns>this, for chaining calls in fluent style.</returns>
        public IEntryWriter<TEntry> AddEntryType<TEntry>()
            where TEntry : ILogEntry
        {
            IEntryWriter<TEntry> existingEntryWriter;
            if (base.TryGetEntryWriter<TEntry>(out existingEntryWriter))
            {
                return existingEntryWriter;
            }

            AvroEntryWriter<TEntry> entryWriter;

            Func<AvroLogWriter, object> entryWriterFactory;
            if (s_entryWriterFactories.TryGetValue(typeof(TEntry), out entryWriterFactory))
            {
                // Use the factory function to create a custom AvroEntryWriter.
                entryWriter = (AvroEntryWriter<TEntry>) entryWriterFactory(this);
            }
            else
            {
                // Use a plain vanilla AvroEntryWriter
                entryWriter = new AvroEntryWriter<TEntry>(this);
            }

            AddEntryWriter(entryWriter);
            return entryWriter;
        }

        /// <summary>
        /// Returns the Avro schema based on all the entry types that have been registered.
        /// </summary>
        /// <remarks>
        /// Make sure to call <see cref="AddEntryType{TEntry}"/> for all entry types before calling <c>GetAvroSchema()</c>.
        /// </remarks>
        /// <returns>The JSON Avro schema for log files using all the registered entry types.</returns>
        public string GetAvroSchema()
        {
            var serializer = AvroSerializer.Create<ILogEntry>(CreateAvroSerializerSettings());
            return serializer.WriterSchema.ToString();
        }

        /// <summary>
        /// Avro logging is <em>not</em> thread-safe.  Other LogJam functionality for enforcing synchronization must be used.
        /// </summary>
        public override bool IsSynchronized { get { return false; } }

        /// <summary>
        /// Returns <c>true</c> when this logwriter and its entrywriters are ready to log.
        /// </summary>
        public bool IsEnabled { get { return IsStarted; } }

        protected override void InternalStart()
        {
            if (_stream == null)
            {
                throw new LogWriterException("The AvroLogWriter stream or file must be set to start logging.", this);
            }

            _avroContainerWriter = AvroContainer.CreateWriter<ILogEntry>(_stream, CreateAvroSerializerSettings(), Codec.Deflate);
            _sequentialWriter = new SequentialWriter<ILogEntry>(_avroContainerWriter, 512);
        }

        internal void WriteAvroLogEntry(ILogEntry entry)
        {
            if (! IsStarted)
            {
                return;
            }

            _sequentialWriter.Write(entry);
        }

        protected override void InternalStop()
        {
            if (_sequentialWriter != null)
            {
                _sequentialWriter.Dispose();
                _sequentialWriter = null;
            }
            if (_avroContainerWriter != null)
            {
                _avroContainerWriter.Dispose();
                _avroContainerWriter = null;
            }

            if ((_stream != null) && _disposeStream)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        private AvroSerializerSettings CreateAvroSerializerSettings()
        {
            return new AvroSerializerSettings()
                   {
                       GenerateSerializer = true,
                       GenerateDeserializer = false,
                       KnownTypes = EntryWriters.Select(kvp => ((IAvroLogEntryTypeInfo)kvp.Value).LogEntryType).Concat(s_defaultKnownTypes).Distinct(),
                       Resolver = new AvroLogEntryContractResolver()
                   };
        }

    }

}