// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorCategory.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Format
{

    /// <summary>
    /// Values for "meanings" or "intents" of different colors.  These can be mapped to a real color based on user preference and 
    /// the capabilities of the presentation system.
    /// </summary>
    public enum ColorCategory : byte
    {
        None = 0,
        Success = 1,
        Failure = 2,
        Warning = 3,
        Important = 4,
        Info = 5,
        Detail = 6,
        Debug = 7
    }

}