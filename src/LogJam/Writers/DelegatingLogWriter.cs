// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegatingLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writers
{
	using System;
	using System.Diagnostics.Contracts;


	/// <summary>
	/// Base class for LogWriters that delegate to a downstream <see cref="ILogWriter{TEntry}"/> instance.
	/// </summary>
	/// <seealso cref="MultiLogWriter{TEntry}"/> for a delegating logwriter that writes to multiple <see cref="ILogWriter{TEntry}"/> instances.
	public abstract class DelegatingLogWriter<TEntry> : ILogWriter<TEntry> where TEntry : ILogEntry
	{

		private readonly ILogWriter<TEntry> _innerLogWriter;
		private bool _disposed = false;

		/// <summary>
		/// Creates a new <see cref="DelegatingLogWriter{TEntry}"/>.
		/// </summary>
		/// <param name="innerLogWriter">The inner <see cref="ILogWriter{TEntry}"/> to delegate to.  May not be <c>null</c>.</param>
		protected DelegatingLogWriter(ILogWriter<TEntry> innerLogWriter)
		{
			Contract.Requires<ArgumentNullException>(innerLogWriter != null);

			_innerLogWriter = innerLogWriter;
		}

		public virtual void Dispose()
		{
			if (! _disposed)
			{
				IDisposable innerDisposable = _innerLogWriter as IDisposable;
				if (innerDisposable != null)
				{
					innerDisposable.Dispose();					
				}
				_disposed = true;
			}
		}

		/// <summary>
		/// Returns the inner <see cref="ILogWriter{TEntry}"/> that this <c>DelegatingLogWriter</c>
		/// forwards to.
		/// </summary>
		public ILogWriter<TEntry> InnerLogWriter
		{
			get { return _innerLogWriter; }
		}

		public virtual bool Enabled { get { return InnerLogWriter.Enabled; } }

		public virtual bool IsSynchronized { get { return InnerLogWriter.IsSynchronized; } }

		public virtual void Write(ref TEntry entry)
		{
			InnerLogWriter.Write(ref entry);
		}

	}

}