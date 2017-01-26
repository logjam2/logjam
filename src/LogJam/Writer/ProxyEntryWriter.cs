// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyEntryWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer
{
    using System;
    using System.Diagnostics.Contracts;


    /// <summary>
    /// Base class for LogWriters that write each entry to a downstream <see cref="IEntryWriter{TEntry}" /> instance.
    /// </summary>
    /// <seealso cref="FanOutEntryWriter{TEntry}" />
    /// for a logwriter that writes to multiple downstream
    /// <see cref="IEntryWriter{TEntry}" />
    /// instances.
    public abstract class ProxyEntryWriter<TEntry> : IEntryWriter<TEntry>, IDisposable
        where TEntry : ILogEntry
    {

		private IEntryWriter<TEntry> _innerEntryWriter;
        private bool _disposed = false;

        /// <summary>
        /// Creates a new <see cref="ProxyEntryWriter{TEntry}" />.
        /// </summary>
        /// <param name="innerEntryWriter">The inner <see cref="IEntryWriter{TEntry}" /> to delegate to. Must not be <c>null</c>.</param>
        protected ProxyEntryWriter(IEntryWriter<TEntry> innerEntryWriter)
        {
            Contract.Requires<ArgumentNullException>(innerEntryWriter != null);

            _innerEntryWriter = innerEntryWriter;
        }

        public virtual void Dispose()
        {
            if (! _disposed)
            {
                IDisposable innerDisposable = _innerEntryWriter as IDisposable;
                if (innerDisposable != null)
                {
                    innerDisposable.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Returns the inner <see cref="IEntryWriter{TEntry}" /> that this <c>ProxyEntryWriter</c>
        /// forwards to.
        /// </summary>
		public IEntryWriter<TEntry> InnerEntryWriter
		{
			get { return _innerEntryWriter; }
			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
				_innerEntryWriter = value;
			}
		}

		public virtual bool IsEnabled { get { return ! _disposed && InnerEntryWriter.IsEnabled; } }

		public Type LogEntryType { get { return typeof(TEntry); } }

        public virtual void Write(ref TEntry entry)
        {
            InnerEntryWriter.Write(ref entry);
        }

    }

}
