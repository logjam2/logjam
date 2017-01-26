// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProxyLogWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using LogJam.Internal;
	using LogJam.Trace;
	using LogJam.Util;


	/// <summary>
	/// An <see cref="ILogWriter"/> that delegates to an inner <see cref="ILogWriter"/>.
	/// </summary>
	public abstract class ProxyLogWriter : BaseLogWriter
	{
		private ILogWriter _innerLogWriter;

		/// <summary>
		/// Creates a new <see cref="ProxyLogWriter"/>.
		/// </summary>
		/// <param name="setupTracerFactory">The <see cref="ITracerFactory"/> tracing setup operations.</param>
		/// <param name="innerLogWriter">The inner <see cref="ILogWriter"/> to delegate to.  Must not be <c>null</c>.</param>
		protected ProxyLogWriter(ITracerFactory setupTracerFactory, ILogWriter innerLogWriter)
			: base(setupTracerFactory)
		{
			Contract.Requires<ArgumentNullException>(innerLogWriter != null);

			_innerLogWriter = innerLogWriter;
		}

		/// <summary>
		/// Returns the inner <see cref="ILogWriter"/> that this <c>ProxyLogWriter</c>
		/// forwards to. 
		/// </summary>
		/// <remarks>
		/// Subclasses may set the <c>InnerLogWriter</c> property; however they need to be careful about synchronization issues,
		/// and also take care to switch all the contained entry writers to match the semantics of switching the proxied logwriter.
		/// </remarks>
		public ILogWriter InnerLogWriter
		{
			get { return _innerLogWriter; }
			protected set
			{
				Contract.Requires<ArgumentNullException>(value != null);
				_innerLogWriter = value;
			}
		}

		#region ILogWriter

		public override bool IsSynchronized { get { return InnerLogWriter.IsSynchronized; } }

		#endregion
		#region Startable overrides

		protected override void InternalStart()
		{
			(InnerLogWriter as IStartable).SafeStart(SetupTracerFactory);

			base.InternalStart();
		}

		protected override void InternalStop()
		{
			base.InternalStop();

			(InnerLogWriter as IStartable).SafeStop(SetupTracerFactory);
		}

		#endregion
		#region IDisposable

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				(_innerLogWriter as IDisposable).SafeDispose(SetupTracerFactory);
			}
		}

		#endregion
	}

}
