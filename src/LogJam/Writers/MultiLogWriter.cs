// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writers
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;


	/// <summary>
	/// Base class for LogWriters that delegate to multiple downstream <see cref="ILogWriter{TEntry}"/> instances.
	/// </summary>
	/// <seealso cref="DelegatingLogWriter{TEntry}"/> for a delegating logwriter that writes to a single <see cref="ILogWriter{TEntry}"/> instance.
	public class MultiLogWriter<TEntry> : ILogWriter<TEntry> where TEntry : ILogEntry
	{

		private readonly ILogWriter<TEntry>[] _innerLogWriters;
		private bool _disposed = false;

		/// <summary>
		/// Creates a new <see cref="MultiLogWriter{TEntry}"/>.
		/// </summary>
		/// <param name="innerLogWriters">The inner <see cref="ILogWriter{TEntry}"/>s to delegate to.  May not be <c>null</c>.</param>
		public MultiLogWriter(params ILogWriter<TEntry>[] innerLogWriters)
		{
			Contract.Requires<ArgumentNullException>(innerLogWriters != null);
			Contract.Requires<ArgumentException>(innerLogWriters.All(writer => writer != null));

			_innerLogWriters = innerLogWriters;
		}

		public virtual void Dispose()
		{
			if (! _disposed)
			{
				foreach (var writer in _innerLogWriters)
				{
					writer.Dispose();
				}
				_disposed = true;
			}
		}

		/// <summary>
		/// Returns the inner <see cref="ILogWriter{TEntry}"/>s that this <c>MultiLogWriter</c>
		/// forwards to.
		/// </summary>
		public IEnumerable<ILogWriter<TEntry>> InnerLogWriters
		{
			get { return _innerLogWriters; }
		}


		/// <inheritdoc />
		public virtual void Write(ref TEntry entry)
		{
			foreach (var writer in _innerLogWriters)
			{
				writer.Dispose();
			}
		}

		/// <inheritdoc />
		public bool Enabled
		{
			get
			{
				// TODO: Perf test this - is it called for normal log operations? should we cache?
				return _innerLogWriters.Any(writer => writer.Enabled);
			}
		}

		/// <inheritdoc />
		public bool IsSynchronized
		{
			get
			{
				// TODO: Perf test this - is it called for normal log operations? should we cache or always return false?
				return _innerLogWriters.All(writer => writer.IsSynchronized);
			}
		}

	}

}