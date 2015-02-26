// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferingLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Util;


	/// <summary>
	/// Base class for log writers that write to buffers.
	/// </summary>
	public abstract class BufferingLogWriter : Startable, IBufferingLogWriter
	{

		/// <summary>
		/// Predicate that causes an <see cref="IBufferingLogWriter"/> to flush after every log entry is written.
		/// </summary>
		public static readonly Func<bool> AlwaysFlush = () => true;

		/// <summary>
		/// Predicate that causes an <see cref="IBufferingLogWriter"/> to never automatically flush.  In most cases this means
		/// that flushing only occurs when the buffer is filled.
		/// </summary>
		public static readonly Func<bool> NeverFlush = () => false;



		/// <summary>
		/// Holds the function that is used to determine whether to flush the buffers or not.
		/// </summary>
		private Func<bool> _flushPredicate;
 

		/// <summary>
		/// Initializes the <see cref="BufferingLogWriter"/> state.
		/// </summary>
		/// <param name="flushPredicate">A function that is used to determine whether to flush the buffers or not.  If <c>null</c>,
		/// a predicate is used to cause buffers to be flushed after every write.</param>
		protected BufferingLogWriter(Func<bool> flushPredicate = null)
		{
			// Default to "always flush" if not specified.
			_flushPredicate = flushPredicate ?? AlwaysFlush;
		}

		/// @inheritdoc
		public Func<bool> FlushPredicate
		{
			get { return _flushPredicate; }
			set
			{
				if (IsStarted)
				{
					throw new LogJamSetupException("FlushPredicate cannot be set after logwriter is started.", this);
				}
				_flushPredicate = value;
			}
		}

		/// @inheritdoc
		public virtual bool Enabled { get { return IsStarted; } }

		/// <summary>
		/// Must be overridden by subclasses.
		/// </summary>
		public abstract bool IsSynchronized { get; }

	}

}