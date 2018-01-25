// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseExistingLogFileRotatorConfig.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using LogJam.Shared.Internal;
using LogJam.Writer.Rotator;

namespace LogJam.Config
{

    /// <summary>
    /// A <see cref="LogFileRotatorConfig" /> implementation that holds a passed in <see cref="ILogFileRotator" />.
    /// </summary>
    public class UseExistingLogFileRotatorConfig : LogFileRotatorConfig
    {

        private readonly ILogFileRotator _rotator;

        public UseExistingLogFileRotatorConfig(ILogFileRotator rotator)
        {
            Arg.NotNull(rotator, nameof(rotator));

            _rotator = rotator;
        }

        /// <inheritdoc />
        public override ILogFileRotator CreateLogFileRotator()
        {
            return _rotator;
        }

    }

}
