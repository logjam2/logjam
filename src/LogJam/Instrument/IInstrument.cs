// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInstrument.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument
{

	/// <summary>
	/// Common signature for all instrument types.
	/// </summary>
	public interface IInstrument
	{
		
		/// <summary>
		/// An instrument's key, plus its instrument type, uniquely identify an instrument. The <c>Key</c> for any given instrument is immutable.
		/// </summary>
		InstrumentKey Key { get; }

	}

}