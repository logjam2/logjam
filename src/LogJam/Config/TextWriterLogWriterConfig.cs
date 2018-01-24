// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterLogWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System;
    using System.IO;

    using LogJam.Shared.Internal;
    using LogJam.Trace;
    using LogJam.Writer.Text;


    /// <summary>
    /// Configures use of a <see cref="TextWriterLogWriter" />, which writes text log output to
    /// any <see cref="TextWriter"/>.
    /// </summary>
    public class TextWriterLogWriterConfig : TextLogWriterConfig
    {

        /// <summary>
        /// Initializes a new config object that will create <see cref="TextWriterLogWriter" /> instances
        /// that write to a <see cref="TextWriter" /> returned from <paramref name="createTextWriterFunc" />.
        /// </summary>
        /// <param name="createTextWriterFunc">
        /// A function that returns a <see cref="TextWriter" />. This function is
        /// called each time the parent <see cref="LogManager" /> is <c>Start()</c>ed.
        /// </param>
        public TextWriterLogWriterConfig(Func<TextWriter> createTextWriterFunc)
        {
            Arg.NotNull(createTextWriterFunc, nameof(createTextWriterFunc));

            CreateTextWriter = createTextWriterFunc;
            DisposeTextWriter = true;
        }

        /// <summary>
        /// Initializes a new config object that will create <see cref="TextWriterLogWriter" /> instances
        /// that write to <paramref name="textWriter" />.
        /// </summary>
        /// <param name="textWriter"></param>
        /// <remarks>
        /// Note that <paramref name="textWriter" /> will not be automatically <c>Dispose()</c>ed each time
        /// the <see cref="LogManager" /> is stopped. To cause automatic <c>Dispose()</c>, set
        /// <see cref="DisposeTextWriter" /> to <c>true</c>.
        /// </remarks>
        public TextWriterLogWriterConfig(TextWriter textWriter)
            : this(() => textWriter)
        {
            Arg.NotNull(textWriter, nameof(textWriter));

            // Default to not Disposing the TextWriter
            DisposeTextWriter = false;
        }

        /// <summary>
        /// Initializes a new <see cref="TextWriterLogWriterConfig" />.  <see cref="CreateTextWriter" /> must be set before
        /// the config object can be used.
        /// </summary>
        public TextWriterLogWriterConfig()
        {
            DisposeTextWriter = true;
        }

        public Func<TextWriter> CreateTextWriter { get; set; }

        /// <summary>
        /// Set to <c>true</c> to call <see cref="IDisposable.Dispose" /> on the created <see cref="TextWriter" />.
        /// </summary>
        public bool DisposeTextWriter { get; set; }

        #region TextLogWriterConfig overrides

        protected override FormatWriter CreateFormatWriter(ITracerFactory setupTracerFactory)
        {
            if (CreateTextWriter == null)
            {
                throw new LogJamSetupException("CreateTextWriter delegate must be set before " + GetType() + " can create a LogWriter.", this);
            }

            return new TextWriterFormatWriter(setupTracerFactory, CreateTextWriter(), DisposeTextWriter, FieldDelimiter, SpacesPerIndent);
        }

        #endregion
    }

}
