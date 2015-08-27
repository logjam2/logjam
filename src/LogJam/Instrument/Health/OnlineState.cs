// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineState.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Instrument.Health
{

	/// <summary>
	/// State that defines whether a <see cref="IHealthItem"/> is online or offline.
	/// </summary>
	public enum OnlineState : byte
	{
		/// <summary>
		/// The component is offline and not available to receive load.
		/// </summary>
		/// <remarks>If any components in a system have <c>OnlineState == OnlineState.Offline</c>, the load balancer
		/// page can be used to take the whole system offline/stop receiving requests until all components are online. 
		/// This is very useful for preventing traffic during system startup and shutdown; however it should not be 
		/// used for runtime errors except after careful consideration.</remarks>
		Offline = 0,

		/// <summary>
		/// The component is available to receive load.
		/// </summary>
		Online = 1
	}

}