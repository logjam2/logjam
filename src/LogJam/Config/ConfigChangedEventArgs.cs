// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigChangedEventArgs.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Config
{
	using System;


	/// <summary>
	/// <see cref="EventArgs"/> derived type for <see cref="ConfigChangeHandler"/> events.
	/// </summary>
	/// <typeparam name="T">
	/// </typeparam>
	public class ConfigChangedEventArgs<T> : EventArgs
	{
		#region Fields

		private readonly T _configChanged;

		#endregion

		#region Constructors and Destructors

		internal ConfigChangedEventArgs(T configChanged)
		{
			_configChanged = configChanged;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the config changed.
		/// </summary>
		/// <value>
		/// The config changed.
		/// </value>
		public T ConfigChanged { get { return _configChanged; } }

		#endregion
	}
}
