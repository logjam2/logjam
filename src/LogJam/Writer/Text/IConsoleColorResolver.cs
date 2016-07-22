// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConsoleColorResolver.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;

    using LogJam.Config;


    /// <summary>
    /// An object that converts <see cref="ColorCategory" /> values to <see cref="ConsoleColor" /> values. May be passed to
    /// a <see cref="ConsoleFormatWriter" />, or configured on a <see cref="ConsoleLogWriterConfig" />.
    /// </summary>
    public interface IConsoleColorResolver
    {

        /// <summary>
        /// Converts a <see cref="ColorCategory" /> value to a <see cref="ConsoleColor" /> value.
        /// </summary>
        /// <param name="colorCategory">A <see cref="ColorCategory" /> value.</param>
        /// <returns>The <see cref="ConsoleColor" /> for <paramref name="colorCategory" />.</returns>
        ConsoleColor ResolveColor(ColorCategory colorCategory);

    }

}
