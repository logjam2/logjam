// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextLogWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;

    using LogJam.Shared.Internal;
    using LogJam.Trace;
    using LogJam.Util;


    /// <summary>
    /// A <see cref="ILogWriter" />s that writes log entries to a target that receives formatted text.
    /// Text targets can be colorized and are generally optimized for readability. In contrast, binary targets are
    /// generally optimized for efficient and precise writing and parsing.
    /// </summary>
    public class TextLogWriter : BaseLogWriter
    {

        // Set to true when this is started.
        private bool _isStarted;

        private readonly FormatWriter _formatWriter;

        /// <summary>
        /// Creates a new <see cref="TextLogWriter" />.
        /// </summary>
        /// <param name="setupTracerFactory">The <see cref="ITracerFactory" /> to use for logging setup operations.</param>
        /// <param name="formatWriter">The <see cref="FormatWriter"/> to use for formatting log entries.</param>
        public TextLogWriter(ITracerFactory setupTracerFactory, FormatWriter formatWriter)
            : base(setupTracerFactory)
        {
            Arg.NotNull(setupTracerFactory, nameof(setupTracerFactory));
            Arg.NotNull(formatWriter, nameof(formatWriter));

            _formatWriter = formatWriter;

            // Default to true
            AutoFlush = true;
        }

        /// <summary>
        /// Returns <c>true</c> when this logwriter and its entrywriters are ready to log.
        /// </summary>
        public virtual bool IsEnabled { get { return _isStarted && _formatWriter.IsEnabled; } }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="TextLogWriter" /> will call <see cref="FormatWriter.Flush" />
        /// after
        /// each log entry is written.
        /// </summary>
        public bool AutoFlush { get; set; }

        /// <summary>
        /// Adds the specified <typeparamref name="TEntry" /> to this <see cref="TextWriterLogWriter" />, using
        /// <paramref name="entryFormatter" /> to
        /// format entries of type <c>TEntry</c>.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="entryFormatter"></param>
        /// <returns>this, for chaining calls in fluent style.</returns>
        // TODO: Rename to .IncludeEntry or .AddEntry or similar?
        public TextLogWriter AddFormat<TEntry>(EntryFormatter<TEntry> entryFormatter = null)
            where TEntry : ILogEntry
        {
            if (entryFormatter == null)
            { // Try creating the default entry formatter
                entryFormatter = DefaultFormatterAttribute.GetDefaultFormatterFor<TEntry>();
                if (entryFormatter == null)
                {
                    throw new ArgumentNullException(nameof(entryFormatter),
                                                    $"No [DefaultFormatter] attribute could be found for entry type {typeof(TEntry).FullName}, so {nameof(entryFormatter)} argument must be set.");
                }
            }

            AddEntryWriter(new InnerEntryWriter<TEntry>(this, entryFormatter));
            return this;
        }

        /// <summary>
        /// Adds the specified <typeparamref name="TEntry" /> to this <see cref="TextWriterLogWriter" />, using
        /// <paramref name="formatAction" /> to
        /// format entries of type <c>TEntry</c>.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="formatAction"></param>
        /// <returns>this, for chaining calls in fluent style.</returns>
        public TextLogWriter AddFormat<TEntry>(EntryFormatAction<TEntry> formatAction)
            where TEntry : ILogEntry
        {
            Arg.NotNull(formatAction, nameof(formatAction));

            return AddFormat((EntryFormatter<TEntry>) formatAction);
        }

        protected override void InternalStart()
        {
            _isStarted = true;
            (_formatWriter as IStartable).SafeStart(SetupTracerFactory);

            base.InternalStart();
        }

        protected override void InternalStop()
        {
            _isStarted = false;
            base.InternalStop();

            (_formatWriter as IStartable).SafeStop(SetupTracerFactory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _formatWriter.Flush();
            }
            _formatWriter.SafeDispose(SetupTracerFactory);
        }

        /// <summary>
        /// Central method to format and write <paramref name="entry" /> to the <see cref="FormatWriter" />.
        /// </summary>
        /// <typeparam name="TEntry">The log entry type</typeparam>
        /// <param name="entry">The log entry</param>
        /// <param name="entryFormatter">The <see cref="EntryFormatter{TEntry}" /></param>
        protected virtual void WriteFormattedEntry<TEntry>(ref TEntry entry, EntryFormatter<TEntry> entryFormatter)
            where TEntry : ILogEntry
        {
            if (_formatWriter.IsEnabled)
            {
                entryFormatter.Format(ref entry, _formatWriter);
                if (AutoFlush)
                {
                    _formatWriter.Flush();
                }
            }
        }


        /// <summary>
        /// Provides log writing to the <see cref="TextLogWriter" /> for entry type <typeparamref name="TEntry" />.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        internal class InnerEntryWriter<TEntry> : IEntryWriter<TEntry>
            where TEntry : ILogEntry
        {

            private readonly TextLogWriter _parent;
            private readonly EntryFormatter<TEntry> _formatter;

            public InnerEntryWriter(TextLogWriter parent, EntryFormatter<TEntry> entryFormatter)
            {
                _parent = parent;
                _formatter = entryFormatter;
            }

            public void Write(ref TEntry entry)
            {
                _parent.WriteFormattedEntry(ref entry, _formatter);
            }

            public bool IsEnabled => _parent.IsEnabled;

            public Type LogEntryType => typeof(TEntry);

            internal EntryFormatter<TEntry> Formatter => _formatter;

            internal TextLogWriter Parent => _parent;

        }

    }

}
