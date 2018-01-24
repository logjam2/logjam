// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntryCountLogFileRotator.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Writer.Rotator
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Threading;

	using LogJam.Config;
	using LogJam.Writer;
	using LogJam.Writer.Rotator;


	/// <summary>
	/// An <see cref="ILogFileRotator"/> for unit testing - creates a new log file after every X entries are written.
	/// </summary>
	internal sealed class EntryCountLogFileRotator : ILogFileRotator
	{

		private readonly string _filenamePattern;
		private readonly int _entriesPerFile;
		private int _count;

		private FileInfo _currentLogFile;
		private readonly List<FileInfo> _allLogFiles;

		public EntryCountLogFileRotator(string filenamePattern, int entriesPerFile)
		{
			Contract.Requires<ArgumentException>(entriesPerFile > 0);

			_filenamePattern = filenamePattern;
			_entriesPerFile = entriesPerFile;
			_count = 0;

			// Parameter validation; must not throw
			_currentLogFile = new FileInfo(string.Format(filenamePattern, 0));
			_allLogFiles = new List<FileInfo>();
			_allLogFiles.Add(_currentLogFile);
		}

		public void IncrementCount()
		{
			int incremented = Interlocked.Increment(ref _count);

			if (0 == incremented % _entriesPerFile)
			{	// Time to trigger rotation
				int nextFileNumber = incremented / _entriesPerFile;
				FileInfo nextLogFile = new FileInfo(string.Format(_filenamePattern, nextFileNumber));
				var eventDelegate = TriggerRotate;
				if (eventDelegate != null)
				{
					eventDelegate(this, new RotateLogFileEventArgs(this, CurrentLogFile, nextLogFile));
				}
			}
		}

		public int Count { get { return _count; } }

		public IEntryWriter<TEntry> ProxyEntryWriter<TEntry>(IEntryWriter<TEntry> innerEntryWriter) where TEntry : ILogEntry
		{
			return new CountingProxyEntryWriter<TEntry>(innerEntryWriter, this);
		}

#region ILogFileRotator

		public event EventHandler<RotateLogFileEventArgs> TriggerRotate;

		public Action Rotate(RotatingLogFileWriter rotatingLogFileWriter, RotateLogFileEventArgs rotateEventArgs)
		{
			Action cleanupAction = rotatingLogFileWriter.SwitchLogFileWriterTo(rotateEventArgs.NextLogFile);
			_currentLogFile = rotateEventArgs.NextLogFile;
			_allLogFiles.Add(CurrentLogFile);
			return cleanupAction;
		}

		public FileInfo CurrentLogFile { get { return _currentLogFile; } }

		public IEnumerable<FileInfo> EnumerateLogFiles { get { return _allLogFiles; } }

#endregion

		internal class Config : LogFileRotatorConfig
		{

			public Config(string filenamePattern, int entriesPerFile)
			{
				FileNamePattern = filenamePattern;
				EntriesPerFile = entriesPerFile;
			}

			public string FileNamePattern { get; set; }

			public int EntriesPerFile { get; set; }

			public override ILogFileRotator CreateLogFileRotator()
			{
				return new EntryCountLogFileRotator(FileNamePattern, EntriesPerFile);
			}

		}

		/// <summary>
		/// A proxy EntryWriter that triggers the rotator
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		internal class CountingProxyEntryWriter<TEntry> : ProxyEntryWriter<TEntry>
			where TEntry : ILogEntry
		{

			private readonly EntryCountLogFileRotator _rotator;

			public CountingProxyEntryWriter(IEntryWriter<TEntry> innerEntryWriter, EntryCountLogFileRotator rotator)
				: base(innerEntryWriter)
			{
				_rotator = rotator;
			}

			public override void Write(ref TEntry entry)
			{
				_rotator.IncrementCount();
				base.Write(ref entry);
			}

		}
	}

}