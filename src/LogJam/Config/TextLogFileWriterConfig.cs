// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextLogFileWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System;
    using System.IO;

    using LogJam.Trace;
    using LogJam.Writer.Text;


    /// <summary>
    /// Configures logging to a text log file.
    /// </summary>
    public class TextLogFileWriterConfig : TextLogWriterConfig, ILogFileWriterConfig
    {

        /// <summary>
        /// The default encoding for writing to text files.
        /// </summary>
        public const string DefaultEncoding = "UTF-8";

        /// <summary>
        /// The encoding to use for writing to the log file.
        /// </summary>
        public string Encoding { get; set; } = DefaultEncoding;

        /// <summary>
        /// Returns the <see cref="ILogFileConfig"/>, which is used to configure the log file.
        /// </summary>
        public LogFileConfig LogFile { get; set; } = new LogFileConfig();

        /// <summary>
        /// A delegate called to write a log file header.
        /// </summary>
        public Action<FileInfo, FormatWriter> WriteHeader { get; set; }

        /// <summary>
        /// A delegate called to write a log file footer.
        /// </summary>
        public Action<FileInfo, FormatWriter> WriteFooter { get; set; }

        protected override FormatWriter CreateFormatWriter(ITracerFactory setupTracerFactory)
        {
            if (LogFile == null)
            {
                throw new InvalidOperationException($"{nameof(TextLogFileWriterConfig)}.{nameof(LogFile)} must be set.");
            }

            var fileStream = LogFile.CreateNewLogFileStream();
            TextWriter textWriter = new StreamWriter(fileStream, System.Text.Encoding.GetEncoding(Encoding));
            // TODO: Add support for configuring which FormatWriter is used.
            return new TextWriterFormatWriter(setupTracerFactory, textWriter, true, FieldDelimiter, SpacesPerIndent);
        }

    }

}
