// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultConsoleColorResolver.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Text
{
    using System;

    using LogJam.Util;


    /// <summary>
    /// Provides a default <see cref="IConsoleColorResolver"/> for writing colorized text to the console.
    /// </summary>
    public sealed class DefaultConsoleColorResolver : Startable, IConsoleColorResolver
    {
        private ConsoleColor _initialForegroundColor, _initialBackgroundColor;

        /// <summary>
        /// Called during console writing startup.
        /// </summary>
        protected override void InternalStart()
        {
            _initialForegroundColor = Console.ForegroundColor;
            _initialBackgroundColor = Console.BackgroundColor;
        }

        /// <summary>
        /// Converts a <see cref="ColorCategory"/> value to a <see cref="ConsoleColor"/> value.
        /// </summary>
        /// <param name="colorCategory">A <see cref="ColorCategory"/> value.</param>
        /// <returns>The <see cref="ConsoleColor"/> for <paramref name="colorCategory"/>.</returns>
        public ConsoleColor ResolveColor(ColorCategory colorCategory)
        {
            switch (colorCategory)
            {
                case ColorCategory.Detail:
                    return ConsoleColor.Gray;

                case ColorCategory.Info:
                    return ConsoleColor.White;

                case ColorCategory.None:
                    return _initialForegroundColor;

                case ColorCategory.Markup:
                    return ConsoleColor.DarkGray;

                case ColorCategory.Warning:
                    return ConsoleColor.DarkYellow;

                case ColorCategory.Error:
                    return ConsoleColor.Red;

                case ColorCategory.SevereError:
                    return ConsoleColor.Red;

                case ColorCategory.Debug:
                    return ConsoleColor.DarkGray;

                case ColorCategory.Success:
                    return ConsoleColor.Green;

                case ColorCategory.Important:
                    return ConsoleColor.Blue;

                default:
                    return _initialForegroundColor;
            }
        }

    }

}