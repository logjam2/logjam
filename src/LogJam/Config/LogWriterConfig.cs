// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Config
{
	using System;

	using LogJam.Trace;
	using LogJam.Trace.Config;
	using LogJam.Writer;


	/// <summary>
	/// Base class for holding configuration of logwriters that write a single entry type, <typeparamref name="TEntry"/>.
	/// </summary>
	/// <see cref="TraceWriterConfig.LogWriterConfig"/>
	public abstract class LogWriterConfig<TEntry> : ILogWriterConfig where TEntry : ILogEntry
	{

		private bool _synchronized = true;

		/// <summary>
		/// Sets or gets whether the <see cref="ILogWriter{TEntry}"/> returned from <see cref="CreateLogWriter"/> should have its
		/// writes synchronized or not.
		/// </summary>
		public virtual bool Synchronized { get { return _synchronized; } set { _synchronized = value; } }

		/// <inheritdoc />
		public ILogWriter CreateILogWriter(ITracerFactory setupTracerFactory)
		{
			return CreateLogWriter(setupTracerFactory);
		}

		/// <summary>
		/// Creates and returns a new <see cref="ILogWriter{TEntry}"/> using the configured settings.
		/// </summary>
		/// <param name="setupTracerFactory">An <see cref="ITracerFactory"/> for tracing information about logging setup.</param>
		/// <returns>A new <see cref="ILogWriter{TEntry}"/> using the configured settings.</returns>
		public abstract ILogWriter<TEntry> CreateLogWriter(ITracerFactory setupTracerFactory);

		public virtual bool Equals(ILogWriterConfig other)
		{
			if (other == null)
			{
				return false;
			}
			if (! ReferenceEquals(GetType(), other.GetType()))
			{
				return false;
			}
			return Synchronized == other.Synchronized;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ILogWriterConfig);
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode();
		}

	}

}