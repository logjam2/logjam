// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextLogWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LogJam.Shared.Internal;
    using LogJam.Trace;
    using LogJam.Writer;
    using LogJam.Writer.Text;


    /// <summary>
    /// Configures use of a <see cref="TextLogWriter" />.
    /// </summary>
    public abstract class TextLogWriterConfig : LogWriterConfig
    {

        // Configured formatters are stored as a list of logentry Type, logentry entryFormatter, configure action
        private readonly List<Tuple<Type, object, Action<TextLogWriter>>> _formatters;

        /// <summary>
        /// Delimiter between fields.
        /// </summary>
        private string _fieldDelimiter;

        /// <summary>
        /// The number of spaces for each indent level.
        /// </summary>
        private int _spacesPerIndent;

        /// <summary>
        /// TimeZone to use for formatting dates and times.
        /// </summary>
        private TimeZoneInfo _timeZone;

        /// <summary>
        /// Initializes a new <see cref="TextLogWriterConfig" />.
        /// </summary>
        protected TextLogWriterConfig()
        {
            _formatters = new List<Tuple<Type, object, Action<TextLogWriter>>>();
            _fieldDelimiter = FormatWriter.DefaultFieldDelimiter;
            _spacesPerIndent = FormatWriter.DefaultSpacesPerIndent;
            _timeZone = TimeZoneInfo.Local;
            IncludeDate = false;
            IncludeTime = true;
        }

        /// <summary>
        /// The delimiter used to separate fields - eg single space, 2 spaces (default value), tab, comma.
        /// </summary>
        public string FieldDelimiter
        {
            get { return _fieldDelimiter; }
            set
            {
                Arg.NotNull(value, nameof(value));

                _fieldDelimiter = value;
            }
        }

        /// <summary>
        /// The number of spaces for each indent level.
        /// </summary>
        public int SpacesPerIndent
        {
            get { return _spacesPerIndent; }
            set
            {
                Arg.InRange(value, 0, 100, nameof(value));

                _spacesPerIndent = value;
            }
        }

        /// <summary>
        /// <c>true</c> to include the Date when formatting log entries with a date/time field. Default is <c>false</c>.
        /// </summary>
        public bool IncludeDate { get; set; }

        /// <summary>
        /// <c>true</c> to include the Timestamp when formatting log entries with a date/time field. Default is <c>true</c>.
        /// </summary>
        public bool IncludeTime { get; set; }

        /// <summary>
        /// Specifies the TimeZone to use when formatting the timestamp for a log entry. Defaults to local time.
        /// </summary>
        public TimeZoneInfo TimeZone
        {
            get { return _timeZone; }
            set
            {
                Arg.NotNull(value, nameof(value));

                _timeZone = value;
            }
        }

        public bool HasFormatterFor<TEntry>()
            where TEntry : ILogEntry
        {
            return HasFormatterFor(typeof(TEntry));
        }

        public bool HasFormatterFor(Type logEntryType)
        {
            return _formatters.Any(tuple => tuple.Item1 == logEntryType);
        }

        /// <summary>
        /// Sets formatting for entry types <typeparamref name="TEntry" /> using <paramref name="entryFormatter" />.
        /// </summary>
        /// <typeparam name="TEntry">The log entry type for the specified <paramref name="entryFormatter" />.</typeparam>
        /// <param name="entryFormatter">
        /// The <see cref="EntryFormatter{TEntry}" /> to use to format <typeparamref name="TEntry" /> objects.
        /// If <c>null</c>, the <see cref="DefaultFormatterAttribute" /> on <typeparamref name="TEntry" /> is used to resolve a
        /// default formatter.
        /// </param>
        /// <returns><c>this</c>, to support chaining configuration calls in a fluent manner.</returns>
        /// <remarks>
        /// If <see cref="Format{TEntry}(LogJam.Writer.Text.EntryFormatter{TEntry})" /> is called more than once for the same
        /// <typeparamref name="TEntry" /> value, the last formatter is the only one used. In other words, repeating the call
        /// replaces
        /// earlier formatters.
        /// </remarks>
        public TextLogWriterConfig Format<TEntry>(EntryFormatter<TEntry> entryFormatter = null)
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

            Type entryType = typeof(TEntry);
            Action<TextLogWriter> configureAction = (mw) => mw.AddFormat(entryFormatter);

            // Remove any existing formatters for the same TEntry.
            _formatters.RemoveAll(tuple => tuple.Item1 == entryType);

            _formatters.Add(new Tuple<Type, object, Action<TextLogWriter>>(entryType, entryFormatter, configureAction));
            return this;
        }

        /// <summary>
        /// Sets formatting for entry types <typeparamref name="TEntry" /> using <paramref name="formatAction" />.
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="formatAction"></param>
        /// <returns></returns>
        public TextLogWriterConfig Format<TEntry>(EntryFormatAction<TEntry> formatAction)
            where TEntry : ILogEntry
        {
            Arg.NotNull(formatAction, nameof(formatAction));

            return Format((EntryFormatter<TEntry>) formatAction);
        }

        #region ILogWriterConfig

        /// <inheritdoc />
        public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
        {
            var formatWriter = CreateFormatWriter(setupTracerFactory);
            formatWriter.FieldDelimiter = FieldDelimiter;
            formatWriter.SpacesPerIndent = SpacesPerIndent;
            formatWriter.IncludeDate = IncludeDate;
            formatWriter.IncludeTime = IncludeTime;
            formatWriter.OutputTimeZone = TimeZone;

            var logWriter = new TextLogWriter(setupTracerFactory, formatWriter);
            ApplyConfiguredFormatters(logWriter);
            return logWriter;
        }

        #endregion

        protected abstract FormatWriter CreateFormatWriter(ITracerFactory setupTracerFactory);

        /// <summary>
        /// Applies all configured formatters to <paramref name="writer" />.
        /// </summary>
        /// <param name="writer"></param>
        protected void ApplyConfiguredFormatters(TextLogWriter writer)
        {
            foreach (var formatterTuple in _formatters)
            {
                var configureFormatterAction = formatterTuple.Item3;
                configureFormatterAction(writer);
            }
        }

    }

}
