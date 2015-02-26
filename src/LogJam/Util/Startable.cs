// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startable.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{

	/// <summary>
	/// Base class for <see cref="IStartable"/> implementations.  
	/// </summary>
	public abstract class Startable : IStartable
	{

		private bool _isStarted;

		protected Startable()
		{
			_isStarted = false;
		}

		/// @inheritdoc
		public virtual void Start()
		{
			lock (this)
			{
				if (! _isStarted)
				{
					InternalStart();
					_isStarted = true;
				}				
			}
		}

		/// <summary>
		/// Can be overridden to provide logic that runs when the object is started.
		/// </summary>
		protected virtual void InternalStart()
		{}

		/// @inheritdoc
		public void Stop()
		{
			lock (this)
			{
				if (_isStarted)
				{
					InternalStop();
					_isStarted = true;
				}
			}
		}

		/// <summary>
		/// Can be overridden to provide logic that runs when the object is stopped.
		/// </summary>
		protected virtual void InternalStop()
		{ }

		/// @inheritdoc
		public virtual bool IsStarted { get { return _isStarted; } }

	}

}