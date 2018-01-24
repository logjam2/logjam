// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startable.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Util
{
	using System;


	/// <summary>
	/// Base class for <see cref="IStartable"/> implementations.  
	/// </summary>
	public abstract class Startable : IStartable
	{

		private StartableState _startableState;

		protected Startable()
		{
			_startableState = StartableState.Unstarted;
		}

		/// @inheritdoc
		public StartableState State
		{
			get { return _startableState; }
		}

		/// @inheritdoc
		public bool ReadyToStart
		{
			get
			{
				var state = _startableState;
				return (state == StartableState.Unstarted) || (state == StartableState.Stopped);
			}
		}

		/// @inheritdoc
		public virtual bool IsStarted
		{
			get { return _startableState == StartableState.Started; }
		}

		/// @inheritdoc
		public virtual void Start()
		{
			lock (this)
			{
				if (ReadyToStart)
				{
					_startableState = StartableState.Starting;
					try
					{
						InternalStart();
						_startableState = StartableState.Started;
					}
					catch
					{
						_startableState = StartableState.FailedToStart;
						throw;
					}
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
				if ((_startableState != StartableState.Stopped) && (_startableState != StartableState.Stopping) && (_startableState != StartableState.Unstarted))
				{
					_startableState = StartableState.Stopping;
					try
					{
						InternalStop();
						_startableState = StartableState.Stopped;
					}
					catch
					{
						_startableState = StartableState.FailedToStop;
						throw;
					}
				}
			}
		}


		/// <summary>
		/// Can be overridden to provide logic that runs when the object is stopped.  <c>InternalStop</c> methods must do any
		/// cleanup necessary, and must be reliable even after startup failed.
		/// </summary>
		protected virtual void InternalStop()
		{ }

		/// @inheritdoc
		/// <summary>
		/// Override ToString() to provide more descriptive start/stop logging.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			// This makes Start/Stop logging friendlier, but subclasses are welcome to provider a better ToString()
			return GetType().GetCSharpName();
		}

	}

}