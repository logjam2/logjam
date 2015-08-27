// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHealthItem.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument.Health
{

	/// <summary>
	/// An <see cref="IInstrument"/> that tracks the health of a component.
	/// </summary>
	public interface IHealthItem : IInstrument
	{

		/// <summary>
		/// Returns the current health status for the component.
		/// </summary>
		HealthStatus GetCurrentStatus();

		/// <summary>
		/// An event that is raised when the health status materially changes.
		/// </summary>
		event HealthStatusChanging StatusChanging;

	}

}