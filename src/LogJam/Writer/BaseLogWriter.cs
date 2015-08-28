// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using LogJam.Internal;
	using LogJam.Trace;
	using LogJam.Util;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;


	/// <summary>
	/// Common implementation for <see cref="ILogWriter"/>s.
	/// </summary>
	public abstract class BaseLogWriter : Startable, ILogWriter, IDisposable, ILogJamComponent
	{
		private readonly ITracerFactory _setupTracerFactory;
		private bool _disposed;

		private readonly Dictionary<Type, object> _entryWriters;

		/// <summary>
		/// Creates a new <see cref="BaseLogWriter"/>.
		/// </summary>
		protected BaseLogWriter(ITracerFactory setupTracerFactory)
		{
			Contract.Requires<ArgumentNullException>(setupTracerFactory != null);

			_setupTracerFactory = setupTracerFactory;

			_disposed = false;
			_entryWriters = new Dictionary<Type, object>();
		}

		/// <summary>
		/// Returns the <see cref="ITracerFactory"/> to use when logging setup operations.
		/// </summary>
		protected ITracerFactory SetupTracerFactory
		{
			get { return _setupTracerFactory; }
		}

		ITracerFactory ILogJamComponent.SetupTracerFactory
		{
			get { return _setupTracerFactory; }
		}

		protected internal void AddEntryWriter<TEntry>(IEntryWriter<TEntry> entryWriter)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(entryWriter != null);

			if (IsStarted)
			{
				throw new LogJamSetupException("New entry writers cannot be added after starting.", this);
			}

			lock (this)
			{
				try
				{
					_entryWriters.Add(typeof(TEntry), entryWriter);
				}
				catch (ArgumentException argExcp)
				{
					throw new LogJamSetupException("Cannot add 2nd writer for Entry type " + typeof(TEntry), argExcp, this);
				}
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
					GC.SuppressFinalize(this);
				}
			}
		}

		protected bool IsDisposed { get { return _disposed; } }

		protected virtual void Dispose(bool disposing)
		{ }

		protected void EnsureNotDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().GetCSharpName());
			}
		}

		#region ILogWriter

		public virtual bool IsSynchronized { get { return false; } }

		public virtual bool TryGetEntryWriter<TEntry>(out IEntryWriter<TEntry> entryWriter) where TEntry : ILogEntry
		{
			lock (this)
			{
				object untypedEntryWriter;
				if (_entryWriters.TryGetValue(typeof(TEntry), out untypedEntryWriter))
				{
					entryWriter = (IEntryWriter<TEntry>) untypedEntryWriter;
					return true;
				}
				else
				{
					entryWriter = null;
					return false;
				}
			}
		}

		public virtual IEnumerable<KeyValuePair<Type, object>> EntryWriters { get { return _entryWriters.ToArray(); } }

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
			EntryWriters.SafeStart(SetupTracerFactory);
		}

		protected override void InternalStop()
		{
			EntryWriters.SafeStop(SetupTracerFactory);
		}

		#endregion

	}

}