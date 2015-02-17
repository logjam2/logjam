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
	public class MultiLogWriter : IMultiLogWriter, IStartable, IDisposable
	{
		private readonly bool _isSynchronized;
		private readonly ITracerFactory _setupTracerFactory;
		private bool _disposed;
		private bool _isStarted;

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
			_isStarted = true;
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

			lock (this)
			{
				_typedLogWriters.Add(typeof(TEntry), logWriter);
			}
		}

		public void Dispose()
		{
			lock (this)
			{
				_isStarted = false;
				if (! _disposed)
				{
					_disposed = true;

					Dispose(true);
				}
			}
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

		public bool Enabled { get { return _isStarted; } }

		public virtual bool IsSynchronized { get { return _isSynchronized; } }

		public bool GetLogWriter<TEntry>(out ILogWriter<TEntry> logWriter) where TEntry : ILogEntry
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

		public IEnumerator<ILogWriter> GetEnumerator()
		{
			return _typedLogWriters.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
		#region IStartable

		public virtual void Start()
		{
			lock (this)
			{
				if (_disposed)
				{
					throw new ObjectDisposedException(GetType().GetCSharpName());
				}

				_isStarted = true;

				InnerLogWriters.SafeStart(SetupTracerFactory);
			}
		}

		public virtual void Stop()
		{
			lock (this)
			{
				_isStarted = false;

				InnerLogWriters.SafeStop(SetupTracerFactory);
			}
		}

		public bool IsStarted { get { return _isStarted; } }

		#endregion
	}

}