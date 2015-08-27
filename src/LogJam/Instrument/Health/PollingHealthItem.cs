// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="PollingHealthItem.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument.Health
{
	using System;


	/// <summary>
	/// An <see cref="IHealthItem"/> that calls a function (<see cref="HealthCheckFunc"/>) to obtain health information. In addition, <see cref="HealthItem.Status"/> can be
	/// set directly for non-polled status changes.
	/// </summary>
	/// <remarks>
	/// Note that the <see cref="HealthStatus"/> value is not updated until <see cref="IHealthItem.GetCurrentStatus()"/> is called.
	/// </remarks>
	public sealed class PollingHealthItem : HealthItem
	{

		private DateTime _lastHealthCheck;

		public PollingHealthItem(InstrumentKey key, Func<HealthStatus> healthCheckFunc)
			: this(key, healthCheckFunc, TimeSpan.Zero)
		{}

		public PollingHealthItem(InstrumentKey key, Func<HealthStatus> healthCheckFunc, TimeSpan minPollInterval)
			: base(key)
		{
			HealthCheckFunc = healthCheckFunc;
			MinPollInterval = minPollInterval;
		}

		/// <summary>
		/// The function that is called to obtain health status.
		/// </summary>
		public Func<HealthStatus> HealthCheckFunc { get; set; }

		/// <summary>
		/// The minimum interval between calls to <see cref="HealthCheckFunc"/>.  If status is requested more frequently than
		/// <c>MinPollInterval</c>, a cached value is returned.
		/// </summary>
		public TimeSpan MinPollInterval { get; set; }

		protected override void OnUpdateStatus(HealthStatus newStatus)
		{
			base.OnUpdateStatus(newStatus);

			_lastHealthCheck = DateTime.Now;
		}

		#region IHealthItem

		public override HealthStatus GetCurrentStatus()
		{
			if (Status == null)
			{
				CheckUpdateHealth();
			}
			else
			{
				if (_lastHealthCheck < DateTime.Now.Subtract(MinPollInterval))
				{
					CheckUpdateHealth();
				}
			}

			return Status;
		}

		#endregion

		private void CheckUpdateHealth()
		{
			var healthCheckFunc = HealthCheckFunc;
			HealthStatus status;
			if (healthCheckFunc == null)
			{
				status = new HealthStatus(OnlineState.Online, HealthState.Warning, "PollingHealthItem.HealthCheckFunc is null");
			}
			else
			{
				try
				{
					status = healthCheckFunc();
				}
				catch (Exception excp)
				{
					OnHealthItemException(excp);
					status = new HealthStatus(OnlineState.Online, HealthState.Error, "Exception while calling HealthCheckFunc", excp);
				}
			}
		}

	}

}