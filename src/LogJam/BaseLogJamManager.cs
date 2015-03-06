// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseLogJamManager.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using System;
	using System.Collections.Generic;

	using LogJam.Trace;


	/// <summary>
	/// Common LogJam *Manager functionality.
	/// </summary>
	public abstract class BaseLogJamManager : IStartable, IDisposable
	{

		#region Instance fields

		private bool _isStarted;
		private Exception _startException;
		private bool _isStopped;

		private readonly List<WeakReference> _disposables = new List<WeakReference>();

		#endregion

		/// <summary>
		/// A special <see cref="ITracerFactory"/> that returns <see cref="Tracer"/>s for managing configuration,
		/// setup, shutdown, and exceptions during logging.
		/// </summary>
		public abstract SetupTracerFactory SetupTracerFactory { get; }

		/// <summary>
		/// Returns the collection of <see cref="TraceEntry"/>s logged through <see cref="SetupTracerFactory"/>.
		/// </summary>
		public abstract IEnumerable<TraceEntry> SetupTraces { get; }

		/// <summary>
		/// Ensures that <see cref="Start"/> is automatically called once, but <see cref="Start"/> is not automatically called
		/// again if an exception occurred during the initial start, or if <see cref="Stop"/> was called.
		/// </summary>
		/// <returns><c>true</c> if this instance has been successfully started; returns <c>false</c> if the instance was <see cref="Stop"/>ped 
		/// or if the initial call to <see cref="Start"/> was not completely successful.</returns>
		public bool EnsureStarted()
		{
			if (_isStarted)
			{
				return true;
			}
			if (_isStopped)
			{
				return false;
			}
			if (_startException != null)
			{
				return false;
			}

			Start();
			return _isStarted;
		}

		/// <summary>
		/// Starts the manager whether or not <c>Start()</c> has already been called.
		/// </summary>
		/// <remarks>To avoid starting more than once, use <see cref="EnsureStarted"/>.</remarks>
		public void Start()
		{
			var tracer = SetupTracerFactory.TracerFor(this);
			string className = GetType().Name;
			tracer.Debug("Starting " + className + "...");

			lock (this)
			{
				try
				{
					InternalStart();
					_isStarted = true;
					tracer.Info(className + " started.");
				}
				catch (Exception startException)
				{
					_startException = startException;
					tracer.Error(startException, "Start failed: Exception occurred.");
				}
			}
		}

		/// <summary>
		/// Stops this instance; closes all disposables.
		/// </summary>
		public void Stop()
		{
			lock (this)
			{
				if (_isStopped)
				{
					return;
				}
				_isStopped = true;
			}

			var tracer = SetupTracerFactory.TracerFor(this);
			string className = GetType().Name;
			tracer.Info("Stopping " + className + "...");

			InternalStop();

			foreach (var disposableRef in _disposables)
			{
				IDisposable disposable = disposableRef.Target as IDisposable;
				if (disposable != null)
				{
					try
					{
						disposable.Dispose();
					}
					catch (ObjectDisposedException)
					{ } // No need to log this
					catch (Exception exception)
					{
						tracer.Error(exception, "Exception while shutting down " + className + ".");
					}
				}
			}

			tracer.Info(className + " stopped.");
			_isStarted = false;
		}

		/// <summary>
		/// The start implementation, to be overridden by derived classes.
		/// </summary>
		protected virtual void InternalStart()
		{ }

		/// <summary>
		/// The stop implementation, to be overridden by derived classes.
		/// </summary>
		protected virtual void InternalStop()
		{ }

		/// <summary>
		/// Returns <c>true</c> if <see cref="Start()"/> was called and succeeded.
		/// </summary>
		public bool IsStarted { get { return _isStarted; } }

		/// <summary>
		/// Returns <c>true</c> if <see cref="Stop"/> or <see cref="Dispose"/> has been called.
		/// 
		/// </summary>
		public bool IsStopped { get { return _isStopped; } }

		/// <summary>
		/// Registers <paramref name="objectToDispose"/> for cleanup when <see cref="Stop"/> is called.
		/// </summary>
		/// <param name="objectToDispose"></param>
		public void DisposeOnStop(object objectToDispose)
		{
			if (objectToDispose is IDisposable)
			{
				_disposables.Add(new WeakReference(objectToDispose));
			}
		}

		public void Dispose()
		{
			Stop();
		}

	}

}