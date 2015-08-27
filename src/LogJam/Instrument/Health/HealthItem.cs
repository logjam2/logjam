// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthItem.cs">
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
	/// A simple <see cref="IHealthItem"/> that is updated by setting the <see cref="Status"/> property.
	/// </summary>
	public class HealthItem : IHealthItem
	{
		private readonly InstrumentKey _key;

		private HealthStatus _lastStatus;

		public HealthItem(InstrumentKey key)
		{
			_key = key;
		}

		/// <summary>
		/// Update the health status of this <see cref="HealthItem"/> by setting this property whenever the status may be changing. Or, obtain the
		/// last set health status value.
		/// </summary>
		public HealthStatus Status
		{
			get {  return _lastStatus; }
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);

				OnUpdateStatus(value);
			}
		}

		protected virtual void OnUpdateStatus(HealthStatus newStatus)
		{
			Contract.Requires<ArgumentNullException>(newStatus != null);

			if (! newStatus.Equals(_lastStatus))
			{
				HealthStatusChanging changingEvent = StatusChanging;
				if (changingEvent != null)
				{
					try
					{
						// Call the changing event
						var eventArgs = new HealthStatusChangingEventArgs(this, _lastStatus, newStatus);
						changingEvent(this, eventArgs);
					}
					catch (Exception excp)
					{
						OnHealthItemException(excp);
					} 
				}

				_lastStatus = newStatus;
			}
		}

		/// <summary>
		/// An <see cref="Exception"/> occurred while polling or publishing health status.
		/// </summary>
		/// <param name="exception"></param>
		protected virtual void OnHealthItemException(Exception exception)
		{
			// REVIEW: Do anything here?  SetupLog? Another HealthItem?
		}


		#region IHealthItem

		public InstrumentKey Key
		{ get { return _key; } }

		public virtual HealthStatus GetCurrentStatus()
		{
			return Status;
		}

		public event HealthStatusChanging StatusChanging;

		#endregion

	}

}