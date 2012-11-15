// ------------------------------------------------------------------------------------------------------------
// <copyright company="Crim Consulting" file="TracerUseCasesWithDefaultConfig.cs">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// ------------------------------------------------------------------------------------------------------------
namespace LogJam.UnitTests.Trace
{
	using System;

	using LogJam.Trace;

	using Xunit;

	/// <summary>
	/// Exercise common <see cref="Tracer"/> use-cases using default configuration.
	/// </summary>
	public class TracerUseCasesWithDefaultConfig
	{
		#region Fields

		private readonly Tracer _tracer;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TracerUseCasesWithDefaultConfig"/> class.
		/// </summary>
		public TracerUseCasesWithDefaultConfig()
		{
			_tracer = TraceManager.GetTracer(GetType());
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// The skip debug message by default.
		/// </summary>
		[Fact]
		public void SkipDebugMessageByDefault()
		{
			_tracer.Debug("Debug message at {0} should not display", DateTime.Now);
		}

		///// <summary>
		///// The trace begin and end activity.
		///// </summary>
		// [Fact]
		// public void TraceBeginAndEndActivity()
		// {
		// 	using (_tracer.StartActivity("Test activity using label"))
		// 	{
		// 		_tracer.Info("Trace message within test activity");
		// 	}
		// }

		/// <summary>
		/// Exercise tracing an exception.
		/// </summary>
		[Fact]
		public void TraceException()
		{
			try
			{
				throw new NotImplementedException("Test exception for exercising Tracer.");
			}
			catch (Exception excp)
			{
				_tracer.Warn(excp, "Caught expected exception at {0}", DateTime.Now);
			}
		}

		/// <summary>
		/// The trace info message.
		/// </summary>
		[Fact]
		public void TraceInfoMessage()
		{
			_tracer.Info("Info message at {0}", DateTime.Now);
		}

		#endregion
	}
}