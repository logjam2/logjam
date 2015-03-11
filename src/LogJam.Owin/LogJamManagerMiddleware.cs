// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamManagerMiddleware.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Threading.Tasks;

	using LogJam.Trace;

	using Microsoft.Owin;


	/// <summary>
	/// Handles startup and shutdown of logging; and stores a <see cref="LogManager"/> and <see cref="TraceManager"/> 
	/// instance with each request.
	/// </summary>
	internal sealed class LogJamManagerMiddleware : OwinMiddleware, IDisposable
	{

		private readonly LogManager _logManager;
		private readonly TraceManager _traceManager;

		public LogJamManagerMiddleware(OwinMiddleware next, LogManager logManager, TraceManager traceManager)
			: base(next)
		{
			Contract.Requires<ArgumentNullException>(next != null);
			Contract.Requires<ArgumentNullException>(logManager != null);
			Contract.Requires<ArgumentNullException>(traceManager != null);

			_logManager = logManager;
			_traceManager = traceManager;
			_traceManager.Start();
		}

		public override Task Invoke(IOwinContext owinContext)
		{
			owinContext.SetLogManager(_logManager);
			owinContext.SetTracerFactory(_traceManager);

			return Next.Invoke(owinContext);
		}

		public void Dispose()
		{
			_logManager.Stop();
			_traceManager.Stop();
		}

	}

}