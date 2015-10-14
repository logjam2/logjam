// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateHealthItem.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument.Health
{
	using System.Collections;
	using System.Collections.Generic;


	/// <summary>
	/// An <see cref="IHealthItem"/> that represents a collection of contained <see cref="IHealthItem"/>s.
	/// </summary>
	public class AggregateHealthItem : InstrumentCollection<IHealthItem>, IHealthItem
	{

		private readonly InstrumentKey _key;

		private HealthStatus _lastStatus;

		public AggregateHealthItem(InstrumentKey key)
		{
			_key = key;
		}

		#region IHealthItem

		public InstrumentKey Key { get { return _key; } }

		public HealthStatus GetCurrentStatus()
		{
			throw new System.NotImplementedException();
		}

		public event HealthStatusChanging StatusChanging;

		#endregion

	}

}