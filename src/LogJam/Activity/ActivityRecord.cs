// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityRecord.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace
{
	using System;
	using System.Diagnostics.Contracts;


	/// <summary>
	/// Base class for all activity records.
	/// </summary>
	/// <remarks>
	/// An activity record tracks the beginning and ending of an activity, along with any activity-specific
	/// metadata.  It also can be correlated with trace records that occur within the activity.
	/// </remarks>
	public abstract class ActivityRecord
	{

		private string _id;

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityRecord"/> class.
		/// </summary>
		protected ActivityRecord()
		{
			SourceName = GetType().FullName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityRecord"/> class.
		/// </summary>
		/// <param name="sourceName">
		/// TODO The source name.
		/// </param>
		protected ActivityRecord(string sourceName)
		{
			Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(sourceName));
			SourceName = sourceName;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		public virtual string Id
		{
			get
			{
				lock (this)
				{
					if (_id == null)
					{
						_id = CreateNewId();
					}
					return _id;
				}
			}
		}

		/// <summary>
		/// Gets the source name.
		/// </summary>
		/// <value>
		/// TODO The source name.
		/// </value>
		public string SourceName { get; private set; }

		/// <summary>
		/// Gets the time elapsed.
		/// </summary>
		/// <value>
		/// TODO The time elapsed.
		/// </value>
		public abstract TimeSpan TimeElapsed { get; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Marks the activity as beginning in the current thread at the current time.
		/// </summary>
		/// <remarks>
		/// Implementations should gracefully handle multiple calls to <c>Begin()</c> - if
		/// this method was previously called for this instance, no changes should be made to
		/// the start time of the activity.
		/// </remarks>
		public abstract void Begin();

		/// <summary>
		/// TODO The end with failure.
		/// </summary>
		/// <param name="exception">
		/// TODO The exception.
		/// </param>
		public abstract void EndWithFailure(Exception exception = null);

		/// <summary>
		/// The end.
		/// </summary>
		public abstract void EndWithSuccess();

		#endregion

		/// <summary>
		/// Create and returns a new activity ID.  May be overridden.
		/// </summary>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public virtual string CreateNewId()
		{
			Contract.Ensures(! string.IsNullOrWhiteSpace(Contract.Result<string>()));

			return Guid.NewGuid().ToString();
		}

	}
}
