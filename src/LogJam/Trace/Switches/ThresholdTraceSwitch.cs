// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThresholdTraceSwitch.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace.TraceSwitch
{
	/// <summary>
	/// A <see cref="ITraceSwitch"/> that allows <see cref="Tracer"/> events that equal or exceed a fixed threshold.
	/// </summary>
	public sealed class ThresholdTraceSwitch : ITraceSwitch
	{
		#region Fields

		private readonly TraceLevel _threshold;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ThresholdTraceSwitch"/> class.
		/// </summary>
		/// <param name="threshold">
		/// The threshold.
		/// </param>
		public ThresholdTraceSwitch(TraceLevel threshold)
		{
			_threshold = threshold;
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The is message enabled.
		/// </summary>
		/// <param name="tracer">
		/// The tracer.
		/// </param>
		/// <param name="traceLevel">
		/// The trace level.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool IsEnabled(Tracer tracer, TraceLevel traceLevel)
		{
			return traceLevel >= _threshold;
		}

		#endregion
	}
}
