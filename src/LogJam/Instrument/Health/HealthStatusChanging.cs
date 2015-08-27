// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthStatusChanging.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument.Health
{
	using System;
	using System.Diagnostics.Contracts;


	/// <summary>
	/// Event signature for <see cref="IHealthItem.StatusChanging"/>.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public delegate void HealthStatusChanging(object sender, HealthStatusChangingEventArgs args);


	public class HealthStatusChangingEventArgs : EventArgs
	{

		private readonly IHealthItem _healthItem;
		private readonly HealthStatus _previousStatus;
		private readonly HealthStatus _newStatus;

		public HealthStatusChangingEventArgs(IHealthItem healthItem, HealthStatus previousStatus, HealthStatus newStatus)
		{
			Contract.Requires<ArgumentNullException>(healthItem != null);
			Contract.Requires<ArgumentNullException>(newStatus != null);

			_healthItem = healthItem;
			_previousStatus = previousStatus;
			_newStatus = newStatus;
		}

		/// <summary>
		/// The <see cref="IHealthItem"/> that is changing.
		/// </summary>
		public IHealthItem HealthItem { get { return _healthItem; } }

		/// <summary>
		/// The previous status for <see cref="HealthItem"/>.  May be <c>null</c> when status is initially set.
		/// </summary>
		public HealthStatus PreviousStatus { get { return _previousStatus; } }

		/// <summary>
		/// The new status for <see cref="HealthItem"/>.  Is never <c>null</c>.
		/// </summary>
		public HealthStatus NewStatus { get { return _newStatus; } }

	}
}
