// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthStatus.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument.Health
{
	using System;

	using LogJam.Trace;


	/// <summary>
	/// Immutable representation of an <see cref="IHealthItem"/>'s status at a point in time.
	/// </summary>
	/// <remarks>
	/// Note that this representation of status has the following key elements:
	/// <list type="number">
	/// <item><term><see cref="OnlineState"/></term><description>Whether the component is online or not. If any components in a system have <c>OnlineState == OnlineState.Offline</c>, the load balancer
	/// page can be used to take the whole system offline/stop receiving requests until all components are online. This is very useful for preventing traffic during system startup and shutdown; however it should not be used for runtime errors except after careful consideration.</description></item>
	/// <item><term><see cref="HealthState"/></term><description>Whether the component is starting, stopping, online, in error state, etc (this is related to, but independent of whether the component is online). Note that error and warning states can be used to trigger monitoring alerts.</description></item>
	/// <item><term><see cref="Description"/>, <see cref="Exception"/></term><description>Other information that can be read by a human to better understand the status.</description></item>
	/// </list>
	/// </remarks>
	public sealed class HealthStatus : IEquatable<HealthStatus>
	{
		private readonly OnlineState _onlineState;
		private readonly HealthState _healthState;
		private readonly string _description;
		private readonly Exception _exception;

		public HealthStatus(OnlineState onlineState, HealthState healthState, DateTimeOffset? timestamp, string description, Exception exception = null)
		{
			_onlineState = onlineState;
			_healthState = healthState;
			Timestamp = timestamp;
			_description = description;
			_exception = exception;
		}

		public HealthStatus(OnlineState onlineState, HealthState healthState, string description, Exception exception = null)
		{
			_onlineState = onlineState;
			_healthState = healthState;
			Timestamp = DateTimeOffset.UtcNow;
			_description = description;
			_exception = exception;
		}

		/// <summary>
		/// Should be <see cref="OnlineState.Online"/> when the component is initialized and fully operational. Should be <see cref="OnlineState.Offline"/>  when 
		/// initializing, shutting down, or in an operational error state.
		/// </summary>
		public OnlineState OnlineState { get { return _onlineState; } }

		/// <summary>
		/// <see cref="HealthState"/> that describes the state of the component.
		/// </summary>
		public HealthState HealthState { get { return _healthState; } }

		/// <summary>
		/// Optional timestamp for when this status became effective.
		/// </summary>
		public DateTimeOffset? Timestamp
		{ get; private set; }

		/// <summary>
		/// Human-readable english description of the status.
		/// </summary>
		public string Description { get { return _description; } }

		/// <summary>
		/// Optional <see cref="Exception"/> that provides detailed information on the cause of an error state.
		/// </summary>
		public Exception Exception { get { return _exception; } }

		#region Equality

		public bool Equals(HealthStatus other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return _onlineState == other._onlineState && _healthState == other._healthState && string.Equals(_description, other._description) && Equals(_exception, other._exception);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return obj is HealthStatus && Equals((HealthStatus) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = ((int) _onlineState << 4) ^ (int) _healthState;
				hashCode = (hashCode * 397) ^ (_description != null ? _description.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (_exception != null ? _exception.GetHashCode() : 0);
				return hashCode;
			}
		}

		#endregion

		#region Static helpers

		/// <summary>
		/// Standard health status value for "health status has not yet been set".
		/// </summary>
		public static readonly HealthStatus Uninitialized = new HealthStatus(OnlineState.Offline, HealthState.Uninitialized, null, "Uninitialized");

		/// <summary>
		/// Returns a new <see cref="HealthStatus"/> with standard values for "component is starting".
		/// </summary>
		/// <returns></returns>
		public static HealthStatus Starting()
		{
			return new HealthStatus(OnlineState.Offline, HealthState.Starting, "Starting");
		}

		/// <summary>
		/// Returns a new <see cref="HealthStatus"/> with standard values for "component is online and healthy".
		/// </summary>
		/// <returns></returns>
		public static HealthStatus Online()
		{
			return new HealthStatus(OnlineState.Online, HealthState.Healthy, "Online");
		}

		/// <summary>
		/// Returns a new <see cref="HealthStatus"/> with standard values for "component is online but warning should be noted".
		/// </summary>
		/// <returns></returns>
		public static HealthStatus OnlineWithWarning(string warning = null, Exception exception = null)
		{
			warning = warning ?? "Online with warning";
            return new HealthStatus(OnlineState.Online, HealthState.Warning, warning, exception);
		}

		/// <summary>
		/// Returns a new <see cref="HealthStatus"/> with standard values for "component is stopping".
		/// </summary>
		/// <returns></returns>
		public static HealthStatus Stopping()
		{
			return new HealthStatus(OnlineState.Offline, HealthState.Stopping, "Stopping");
		}

		/// <summary>
		/// Returns a new <see cref="HealthStatus"/> with standard values for "component is offline due to an exception".
		/// </summary>
		/// <returns></returns>
		public static HealthStatus OfflineDueToError(Exception exception)
		{
			return new HealthStatus(OnlineState.Offline, HealthState.Error, "Offline due to exception", exception);
		}

		/// <summary>
		/// Returns a new <see cref="HealthStatus"/> with standard values for "component is offline due to an error".
		/// </summary>
		/// <returns></returns>
		public static HealthStatus OfflineDueToError(string errorDescription, Exception exception = null)
		{
			return new HealthStatus(OnlineState.Offline, HealthState.Error, errorDescription, exception);
		}

		#endregion
	}

}