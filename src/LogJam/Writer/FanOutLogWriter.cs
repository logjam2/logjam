// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FanOutLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;


	/// <summary>
	/// Base class for LogWriters that write each entry to multiple downstream <see cref="ILogWriter{TEntry}"/> instances.
	/// </summary>
	/// <seealso cref="ProxyLogWriter{TEntry}"/> for a similar logwriter that writes each entry to a single <see cref="ILogWriter{TEntry}"/> instance.
	public class FanOutLogWriter<TEntry> : ILogWriter<TEntry>, IDisposable
		where TEntry : ILogEntry
	{

		private readonly ILogWriter<TEntry>[] _innerLogWriters;
		private bool _disposed = false;

		/// <summary>
		/// Creates a new <see cref="FanOutLogWriter{TEntry}"/>.
		/// </summary>
		/// <param name="innerLogWriters">The inner <see cref="ILogWriter{TEntry}"/>s to delegate to.  May not be <c>null</c>.</param>
		public FanOutLogWriter(params ILogWriter<TEntry>[] innerLogWriters)
		{
			Contract.Requires<ArgumentNullException>(innerLogWriters != null);
			Contract.Requires<ArgumentException>(innerLogWriters.All(writer => writer != null));

			_innerLogWriters = innerLogWriters;
		}

		/// <summary>
		/// Creates a new <see cref="FanOutLogWriter{TEntry}"/>.
		/// </summary>
		/// <param name="innerLogWriters">The inner <see cref="ILogWriter{TEntry}"/>s to delegate to.  May not be <c>null</c>.</param>
		public FanOutLogWriter(IEnumerable<ILogWriter<TEntry>> innerLogWriters)
		{
			Contract.Requires<ArgumentNullException>(innerLogWriters != null);
			Contract.Requires<ArgumentException>(innerLogWriters.All(writer => writer != null));

			_innerLogWriters = innerLogWriters.ToArray();
		}
		
		public virtual void Dispose()
		{
			if (! _disposed)
			{
				foreach (var writer in _innerLogWriters)
				{
					var disposable = writer as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				_disposed = true;
			}
		}

		/// <summary>
		/// Returns the inner <see cref="ILogWriter{TEntry}"/>s that this <c>FanOutLogWriter</c>
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
				writer.Write(ref entry);
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