// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorCategory.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{

    /// <summary>
    /// Values for "meanings" or "intents" of different text colors. These can be mapped to a real color based on user
    /// preference and
    /// the capabilities of the presentation system.
    /// </summary>
    public enum ColorCategory : byte
    {

        /// <summary>
        /// No color category specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Success content - usually colored green.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Important content - usually highlighted to stand out from standard text.
        /// </summary>
        Important = 2,

        /// <summary>
        /// Severe error content - usually colored red, may be colored differently to stand out from regular <see cref="Error" />s.
        /// </summary>
        SevereError = 3,

        /// <summary>
        /// Error content - usually colored red.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Warning content - usually colored yellow.
        /// </summary>
        Warning = 5,

        /// <summary>
        /// Informational content - usually the standard text color.
        /// </summary>
        Info = 6,

        /// <summary>
        /// Detailed content - usually colored slightly darker than informational content.
        /// </summary>
        Detail = 7,

        /// <summary>
        /// Debug content - usually colorer darker than detail content.
        /// </summary>
        Debug = 8,

        /// <summary>
        /// Markup text, eg field delimiters, line prefixes, etc.
        /// </summary>
        Markup = 9

    }

}
