// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthState.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument.Health
{

	/// <summary>
	/// The set of defined health states for <see cref="IHealthItem"/> instruments.
	/// </summary>
	public enum HealthState : short
	{
		/// <summary>
		/// Component and/or <see cref="IHealthItem"/> has not been initialized.
		/// </summary>
		Uninitialized = 0,

		/// <summary>
		/// Component is starting.
		/// </summary>
		Starting,

		/// <summary>
		/// Component is online and healthy.
		/// </summary>
		Healthy,

		/// <summary>
		/// Component is shutting down.
		/// </summary>
		Stopping,

		/// <summary>
		/// Component is in an error state and cannot function normally.
		/// </summary>
		Error,

		/// <summary>
		/// Component is functioning normally but status is abnormal.
		/// </summary>
		Warning,

		/// <summary>
		/// Component's health is not adequately described by any of the standard values.
		/// </summary>
		Other
	}

}