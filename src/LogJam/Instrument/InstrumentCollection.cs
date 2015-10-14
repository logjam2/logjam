// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstrumentCollection.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;


	/// <summary>
	/// A collection + dictionary of <typeparamref name="TInstrument"/>s.
	/// </summary>
	/// <typeparam name="TInstrument">The instrument type contained in this <c>InstrumentCollection</c>.</typeparam>
	public class InstrumentCollection<TInstrument> : ConcurrentDictionary<InstrumentKey, TInstrument>
		where TInstrument : IInstrument
	{

		public InstrumentCollection()
		{}

		public InstrumentCollection(IEnumerable<TInstrument> collection)
			: base(collection.Select(instrument => new KeyValuePair<InstrumentKey, TInstrument>(instrument.Key, instrument)))
		{
			Contract.Requires<ArgumentNullException>(collection != null);
		}

		public bool TryAdd(TInstrument instrument)
		{
			Contract.Requires<ArgumentNullException>(instrument != null);

			return base.TryAdd(instrument.Key, instrument);
		}

	}

}