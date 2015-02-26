// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using LogJam.Trace;
	using LogJam.Util;


	/// <summary>
	/// A collection of typed log writers.  For any given entry type, there can be at most one corresponding <see cref="ILogWriter{TEntry}"/>.
	/// </summary>
	/// <seealso cref="TextWriterMultiLogWriter"/>
	public class MultiLogWriter : Startable, IMultiLogWriter, IDisposable
	{
		private readonly bool _isSynchronized;
		private readonly ITracerFactory _setupTracerFactory;
		private bool _disposed;

		private readonly Dictionary<Type, ILogWriter> _typedLogWriters;

		/// <summary>
		/// Creates a new <see cref="MultiLogWriter"/>.
		/// </summary>
		internal MultiLogWriter(bool isSychronized, ITracerFactory setupTracerFactory)
		{
			Contract.Requires<ArgumentNullException>(setupTracerFactory != null);

			_isSynchronized = isSychronized;
			_setupTracerFactory = setupTracerFactory;

			_disposed = false;
			_typedLogWriters = new Dictionary<Type, ILogWriter>();
		}

		/// <summary>
		/// Returns the <see cref="ITracerFactory"/> to use when logging setup operations.
		/// </summary>
		protected ITracerFactory SetupTracerFactory
		{
			get { return _setupTracerFactory; }
		}

		protected internal void AddLogWriter<TEntry>(ILogWriter<TEntry> logWriter)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(logWriter != null);
			if (IsStarted)
			{
				throw new LogJamSetupException("New log writers cannot be added after starting.", this);
			}

			lock (this)
			{
				_typedLogWriters.Add(typeof(TEntry), logWriter);
			}
		}

		public void Dispose()
		{
			Stop();
			lock (this)
			{
				if (! _disposed)
				{
					_disposed = true;

					Dispose(true);
				}
			}
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			InnerLogWriters.SafeDispose(SetupTracerFactory);
		}

		internal IEnumerable<ILogWriter> InnerLogWriters
		{
			get { return (this as IEnumerable<ILogWriter>); }
		}

		#region IMultiLogWriter

		public bool Enabled { get { return IsStarted; } }

		public virtual bool IsSynchronized { get { return _isSynchronized; } }

		public bool GetLogWriter<TEntry>(out ILogWriter<TEntry> logWriter) where TEntry : ILogEntry
		{
			lock (this)
			{
				ILogWriter untypedLogWriter;
				if (_typedLogWriters.TryGetValue(typeof(TEntry), out untypedLogWriter))
				{
					logWriter = (ILogWriter<TEntry>) untypedLogWriter;
					return true;
				}
				else
				{
					logWriter = new NoOpLogWriter<TEntry>();
					return false;
				}
			}
		}

		public IEnumerator<ILogWriter> GetEnumerator()
		{
			return _typedLogWriters.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
		#region Startable overrides

		public override void Start()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().GetCSharpName());
			}

			base.Start();
		}

		protected override void InternalStart()
		{
			InnerLogWriters.SafeStart(SetupTracerFactory);
		}

		protected override void InternalStop()
		{
			InnerLogWriters.SafeStop(SetupTracerFactory);
		}

		#endregion
	}

}