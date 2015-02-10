// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterMultiLogWriter.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Threading;

	using LogJam.Format;


	/// <summary>
	/// Supports writing multiple log entry types to a <see cref="TextWriter"/>.
	/// </summary>
	/// <seealso cref="TextWriterLogWriter{TEntry}"/>
	public class TextWriterMultiLogWriter : IMultiLogWriter, IStartable, IDisposable
	{

		private bool _disposed;
		private bool _isStarted;
		private readonly TextWriter _textWriter;
		private readonly bool _isSynchronized;
		private readonly string _newLine;

		private readonly Dictionary<Type, ILogWriter> _typedLogWriters;

		/// <summary>
		/// Creates a new <see cref="TextWriterMultiLogWriter"/>.
		/// </summary>
		/// <param name="textWriter">The <see cref="TextWriter"/> to write formatted log output to.</param>
		/// <param name="synchronize">Set to <c>true</c> if all logging operations should be synchronized.</param>
		public TextWriterMultiLogWriter(TextWriter textWriter, bool synchronize)
		{
			Contract.Requires<ArgumentNullException>(textWriter != null);

			_disposed = false;
			_textWriter = textWriter;
			_isSynchronized = synchronize;
			_isStarted = true;
			_newLine = textWriter.NewLine;

			_typedLogWriters = new Dictionary<Type, ILogWriter>();
		}

		public void Dispose()
		{
			lock(this)
			{
				_isStarted = false;
				if (!_disposed)
				{
					_disposed = true;
					_textWriter.Dispose();
				}
			}
		}

		/// <summary>
		/// Adds the specified <typeparamref name="TEntry"/> to this <see cref="TextWriterMultiLogWriter"/>, using <paramref name="entryFormatter"/> to
		/// format entries of type <c>TEntry</c>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="entryFormatter"></param>
		/// <returns>this, for chaining calls in fluent style.</returns>
		public TextWriterMultiLogWriter AddFormat<TEntry>(LogFormatter<TEntry> entryFormatter)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(entryFormatter != null);

			lock (this)
			{
				// Throws if a formatter has already been added for the same TEntry
				_typedLogWriters.Add(typeof(TEntry), new InnerLogWriter<TEntry>(this, entryFormatter));
				return this;
			}
		}

		/// <summary>
		/// Adds the specified <typeparamref name="TEntry"/> to this <see cref="TextWriterMultiLogWriter"/>, using <paramref name="formatAction"/> to
		/// format entries of type <c>TEntry</c>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="formatAction"></param>
		/// <returns>this, for chaining calls in fluent style.</returns>
		public TextWriterMultiLogWriter AddFormat<TEntry>(FormatAction<TEntry> formatAction)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(formatAction != null);

			return AddFormat((LogFormatter<TEntry>) formatAction);
		}

		#region IMultiLogWriter

		public bool Enabled { get { return _isStarted; } }

		public bool IsSynchronized { get { return _isSynchronized; } }

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

		public void Start()
		{
			lock (this)
			{
				if (! _disposed)
				{
					_isStarted = true;
				}
			}
		}

		public void Stop()
		{
			_isStarted = false;
		}

		public bool IsStarted { get { return _isStarted; } }

		#endregion

		private void WriteFormattedEntry(string formattedEntry)
		{
			bool lockTaken = false;
			bool includeNewLine = ! formattedEntry.EndsWith(_newLine);
			if (_isSynchronized)
			{
				Monitor.Enter(this, ref lockTaken);
			}

			try
			{
				if (_isStarted)
				{
					if (includeNewLine)
					{
						_textWriter.WriteLine(formattedEntry);
					}
					else
					{
						_textWriter.Write(formattedEntry);
					}
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(this);
				}
			}
		}

		/// <summary>
		/// Provides log writing to the <see cref="TextWriterMultiLogWriter"/> for entry type <typeparamref name="TEntry"/>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		private class InnerLogWriter<TEntry> : ILogWriter<TEntry>
			where TEntry : ILogEntry
		{

			private readonly TextWriterMultiLogWriter _parent;
			private readonly LogFormatter<TEntry> _formatter;

			public InnerLogWriter(TextWriterMultiLogWriter parent, LogFormatter<TEntry> entryFormatter)
			{
				_parent = parent;
				_formatter = entryFormatter;
			}

			public void Write(ref TEntry entry)
			{
				if (_parent.Enabled)
				{
					_parent.WriteFormattedEntry(_formatter.Format(ref entry));
				}
			}

			public bool Enabled { get { return _parent.Enabled; } }

			public bool IsSynchronized { get { return _parent.IsSynchronized; } }

		}
	}

}