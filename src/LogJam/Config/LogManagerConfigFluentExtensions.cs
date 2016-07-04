// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerConfigFluentExtensions.cs">
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
    using System.IO;
    using System.Linq;

    using LogJam.Trace;
    using LogJam.Writer;
    using LogJam.Writer.Text;


    /// <summary>
    /// Extension methods to <see cref="LogManagerConfig" /> for configuring using fluent syntax.
    /// </summary>
    public static class LogManagerConfigFluentExtensions
    {

        public static TextWriterLogWriterConfig UseTextWriter(this LogManagerConfig logManagerConfig, TextWriter textWriter)
        {
            Contract.Requires<ArgumentNullException>(logManagerConfig != null);
            Contract.Requires<ArgumentNullException>(textWriter != null);

            var writerConfig = new TextWriterLogWriterConfig(textWriter);
            logManagerConfig.Writers.Add(writerConfig);
            return writerConfig;
        }

        public static TextWriterLogWriterConfig UseTextWriter(this LogManagerConfig logManagerConfig, Func<TextWriter> createTextWriterFunc)
        {
            Contract.Requires<ArgumentNullException>(logManagerConfig != null);
            Contract.Requires<ArgumentNullException>(createTextWriterFunc != null);

            var writerConfig = new TextWriterLogWriterConfig(createTextWriterFunc);
            logManagerConfig.Writers.Add(writerConfig);
            return writerConfig;
        }

        /// <summary>
        /// Ensures that a single <see cref="DebuggerLogWriterConfig" /> is configured in the <paramref name="logManagerConfig" />.
        /// </summary>
        /// <param name="logManagerConfig"></param>
        /// <returns></returns>
        public static DebuggerLogWriterConfig UseDebugger(this LogManagerConfig logManagerConfig)
        {
            Contract.Requires<ArgumentNullException>(logManagerConfig != null);

            var debuggerConfig = logManagerConfig.Writers.OfType<DebuggerLogWriterConfig>().FirstOrDefault();
            if (debuggerConfig == null)
            {
                debuggerConfig = new DebuggerLogWriterConfig();
                logManagerConfig.Writers.Add(debuggerConfig);
            }
            return debuggerConfig;
        }

        /// <summary>
        /// Ensures that a single <see cref="ConsoleLogWriterConfig" /> is configured in the <paramref name="logManagerConfig" />.
        /// </summary>
        /// <param name="logManagerConfig"></param>
        /// <param name="colorize"></param>
        /// <returns>The <see cref="ConsoleLogWriterConfig" />, which can be altered as needed.</returns>
        public static ConsoleLogWriterConfig UseConsole(this LogManagerConfig logManagerConfig, bool colorize = true)
        {
            Contract.Requires<ArgumentNullException>(logManagerConfig != null);

            var consoleWriterConfig = logManagerConfig.Writers.OfType<ConsoleLogWriterConfig>().FirstOrDefault();
            if (consoleWriterConfig == null)
            {
                consoleWriterConfig = new ConsoleLogWriterConfig();
                logManagerConfig.Writers.Add(consoleWriterConfig);
            }

            consoleWriterConfig.UseColor = colorize;

            return consoleWriterConfig;
        }

        /// <summary>
        /// Use the specified <paramref name="logWriter"/>.
        /// </summary>
        /// <param name="logManagerConfig"></param>
        /// <param name="logWriter"></param>
        /// <returns>A <see cref="UseExistingLogWriterConfig"/> that will return <paramref name="logWriter"/> when the <see cref="LogManager"/> is started.</returns>
        public static UseExistingLogWriterConfig UseLogWriter(this LogManagerConfig logManagerConfig, ILogWriter logWriter)
        {
            Contract.Requires<ArgumentNullException>(logManagerConfig != null);
            Contract.Requires<ArgumentNullException>(logWriter != null);

            var useExistingConfig = new UseExistingLogWriterConfig(logWriter);
            logManagerConfig.Writers.Add(useExistingConfig);
            return useExistingConfig;
        }

        /// <summary>
        /// Log to <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="TEntry">The <see cref="ILogEntry"/> type to log to the list.</typeparam>
        /// <param name="logManagerConfig">The <see cref="LogManagerConfig"/> being configured.</param>
        /// <param name="list">A list object.</param>
        /// <returns>A <see cref="ListLogWriterConfig{TEntry}"/> holding the configuration for the <see cref="ListLogWriter{TEntry}"/>. Can be further configured.</returns>
        public static ListLogWriterConfig<TEntry> UseList<TEntry>(this LogManagerConfig logManagerConfig, IList<TEntry> list)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(logManagerConfig != null);
            Contract.Requires<ArgumentNullException>(list != null);

            var listLogWriterConfig = new ListLogWriterConfig<TEntry>()
            {
                List = list
            };
            logManagerConfig.Writers.Add(listLogWriterConfig);
            return listLogWriterConfig;
        }

        /// <summary>
        /// Adds <paramref name="entryFormatter" /> or the default formatter for <typeparamref name="TEntry" /> to each of
        /// the <see cref="ILogWriterConfig" /> objects in <paramref name="logWriterConfigs" /> that are of type
        /// <see cref="TextLogWriterConfig" />.
        /// <para>
        /// This is a convenience function to make it easy to apply the same formatting for the same entry types to multiple
        /// logwriters.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntry"></typeparam>
        /// <param name="logWriterConfigs">A collection of <see cref="ILogWriterConfig" /> objects.</param>
        /// <param name="entryFormatter">
        /// The <see cref="EntryFormatter{TEntry}" /> to use for all entries of type <typeparamref name="TEntry" />;
        /// or <c>null</c> to use the default entryformatter for <typeparamref name="TEntry" />.
        /// </param>
        /// <param name="overwriteExistingFormatters">
        /// If <c>true</c>, existing formatters for <typeparamref name="TEntry" /> are
        /// overwritten with default formatters.
        /// </param>
        /// <returns>The <paramref name="logWriterConfigs" /> parameter, for fluent call chaining.</returns>
        public static IEnumerable<ILogWriterConfig> FormatAll<TEntry>(this IEnumerable<ILogWriterConfig> logWriterConfigs,
                                                                      EntryFormatter<TEntry> entryFormatter = null,
                                                                      bool overwriteExistingFormatters = false)
            where TEntry : ILogEntry
        {
            Contract.Requires<ArgumentNullException>(logWriterConfigs != null);

            if (entryFormatter == null)
            { // Try creating the default entry formatter
                entryFormatter = DefaultFormatterAttribute.GetDefaultFormatterFor<TEntry>();
                if (entryFormatter == null)
                {
                    throw new ArgumentNullException(nameof(entryFormatter),
                                                    $"No [DefaultFormatter] attribute could be found for entry type {typeof(TEntry).FullName}, so {nameof(entryFormatter)} argument must be set.");
                }
            }

            foreach (var logWriterConfig in logWriterConfigs)
            {
                var textLogWriterConfig = logWriterConfig as TextLogWriterConfig;
                if (textLogWriterConfig != null)
                {
                    if (overwriteExistingFormatters || ! textLogWriterConfig.HasFormatterFor<TEntry>())
                    {
                        textLogWriterConfig.Format(entryFormatter);
                    }
                }
            }
            return logWriterConfigs;
        }

    }

}
