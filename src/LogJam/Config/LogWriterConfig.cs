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
	/// Base class for holding logwriter configuration.
	/// </summary>
	/// <see cref="TraceWriterConfig.LogWriterConfig"/>
	public abstract class LogWriterConfig : ILogWriterConfig
	{

		private bool _synchronized = true;
		private bool _disposeOnStop = true;

		/// <summary>
		/// Sets or gets whether the <see cref="IEntryWriter{TEntry}"/> returned from <see cref="CreateLogWriter"/> should have its
		/// writes synchronized or not.  Default is <c>true</c>; however <c>false</c> is returned if <see cref="BackgroundLogging"/>
		/// is <c>true</c>, to avoid synchronization overhead when logging on a single background thread.
		/// </summary>
		public virtual bool Synchronized { get { return _synchronized && ! BackgroundLogging; } set { _synchronized = value; } }

		/// <summary>
		/// Sets or gets whether log writes should be queued from the logging thread, and written on a single background thread.
		/// Default is <c>false</c>.
		/// </summary>
		public virtual bool BackgroundLogging { get; set; }

		/// <summary>
		/// Sets or gets whether the <see cref="ILogWriter"/> created by <see cref="CreateLogWriter"/> should be disposed
		/// when the <see cref="LogManager"/> is stopped.  Default is <c>true</c>.
		/// </summary>
		public virtual bool DisposeOnStop { get { return _disposeOnStop; } set { _disposeOnStop = value; } }

		/// <inheritdoc />
		public abstract ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory);

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
			return (Synchronized == other.Synchronized)
				&& (BackgroundLogging = other.BackgroundLogging)
				&& (DisposeOnStop = other.DisposeOnStop);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(obj, this) || Equals(obj as ILogWriterConfig);
		}

	}

}