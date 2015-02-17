// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivityCollector.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Activity.Collectors
{
	using System;

	using LogJam.Trace;
	using LogJam.Writer;


	/// <summary>
	/// Interface for activity collectors, which are notified when activities are started and ended.
	/// </summary>
	/// <typeparam name="TActivityRecord">
	/// The activity type that this <c>IActivityCollector</c> receives.
	/// </typeparam>
	public interface IActivityCollector<in TActivityRecord>
		where TActivityRecord : ActivityRecord
	{
		#region Public Properties

		/// <summary>
		/// Gets a value indicating whether this <see cref="ILogWriter{TEntry}"/> should receive <see cref="Tracer"/> messages.
		/// </summary>
		/// <value>
		/// If <c>true</c>, this <c>ITraceCollector</c> should receive messages.  If <c>false</c>, none of the other 
		/// <c>ITraceCollector</c> methods should be called.
		/// </value>
		bool IsActive { get; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The begin activity.
		/// </summary>
		/// <param name="activity">
		/// The activity.
		/// </param>
		void BeginActivity(TActivityRecord activity);

		/// <summary>
		/// The end activity.
		/// </summary>
		/// <param name="activity">
		/// The activity.
		/// </param>
		/// <param name="success">
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		void EndActivity(TActivityRecord activity, bool success, Exception exception);

		#endregion
	}
}
