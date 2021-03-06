﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogWriterConfig.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
    using System;

    using LogJam.Config.Json;
    using LogJam.Trace;
    using LogJam.Writer.Text;


    /// <summary>
    /// Configures a log writer that writes log output to the console, aka stdout.
    /// </summary>
    /// <remarks>
    /// All instances of <see cref="ConsoleLogWriterConfig"/> are equal, to prevent duplicate instances in <see cref="LogManagerConfig.Writers"/>.
    /// </remarks>
    [JsonTypeHint("Target", "Console")]
    public sealed class ConsoleLogWriterConfig : TextLogWriterConfig
    {

        /// <summary>
        /// Creates a new <see cref="ConsoleLogWriterConfig" />.
        /// </summary>
        public ConsoleLogWriterConfig()
        {}

        /// <summary>
        /// Gets or sets a value that specifies whether created <see cref="ConsoleFormatWriter" />s will write colored text output.
        /// </summary>
        public bool UseColor
        {
            get { return ColorResolverFactory != null; }
            set
            {
                if (UseColor == value)
                {
                    return;
                }

                if (value)
                {
                    ColorResolverFactory = () => new DefaultConsoleColorResolver();
                }
                else
                {
                    ColorResolverFactory = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a function that creates new <see cref="IConsoleColorResolver" /> instances for colorizing console log
        /// output.
        /// May be <c>null</c>, in which case console log output is not colorized.
        /// </summary>
        public Func<IConsoleColorResolver> ColorResolverFactory { get; set; }

        protected override FormatWriter CreateFormatWriter(ITracerFactory setupTracerFactory)
        {
            IConsoleColorResolver colorResolver = ColorResolverFactory?.Invoke();
            return new ConsoleFormatWriter(setupTracerFactory, colorResolver, FieldDelimiter, SpacesPerIndent);
        }

        public override bool Equals(object obj)
        {
            return obj is ConsoleLogWriterConfig;
        }

        public override int GetHashCode()
        {
            // All instances of this type have the same HashCode
            return GetType().GetHashCode();
        }

    }

}
