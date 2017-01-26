// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeLogFileLogWriter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Writer.Rotator
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;

	using LogJam.Config;
	using LogJam.Test.Shared.Writers;
	using LogJam.Trace;
	using LogJam.Writer;


	/// <summary>
	/// Instead of writing to a real log file, stores the log file name in memory and accumulates the logged entries in memory.
	/// </summary>
	public sealed class FakeLogFileLogWriter<TEntry> : TestLogWriter<TEntry>
		where TEntry : ILogEntry
	{

		public FakeLogFileLogWriter(ITracerFactory setupTracerFactory, FileInfo logFile, bool synchronize = false)
			: base(setupTracerFactory, synchronize)
		{
			LogFile = logFile;
		}

		public FileInfo LogFile { get; private set; }

		public long TimestampOpened { get; private set; }
		public long TimestampClosed { get; private set; }

		protected override void InternalStart()
		{
			TimestampOpened = Stopwatch.GetTimestamp();
			base.InternalStart();
		}

		protected override void InternalStop()
		{
			base.InternalStop();
			TimestampClosed = Stopwatch.GetTimestamp();
		}

		public override string ToString()
		{
			return string.Format("FakeLogFileWriter: {0}", LogFile.Name);
		}


		public class Config : LogWriterConfig, ILogFileWriterConfig
		{
			private readonly List<FakeLogFileLogWriter<TEntry>> _createdLogWriters = new List<FakeLogFileLogWriter<TEntry>>();

            public LogFileConfig LogFile { get; } = new LogFileConfig();

			/// <summary>
			/// Allows tests to attach whatever behavior they want to be notified when an entry is logged.
			/// </summary>
			public event EventHandler<TestEntryLoggedEventArgs<TEntry>> EntryLogged;

			public override ILogWriter CreateLogWriter(ITracerFactory setupTracerFactory)
			{
				var logWriter = new FakeLogFileLogWriter<TEntry>(setupTracerFactory, new FileInfo(LogFile.GetNewLogFileFullPath()));
				_createdLogWriters.Add(logWriter);
				logWriter.EntryLogged += EntryLogged;
				return logWriter;
			}

			/// <summary>
			/// Tracks all the <see cref="FakeLogFileLogWriter{TEntry}"/>s that have been created, just for testing.
			/// </summary>
			public IEnumerable<FakeLogFileLogWriter<TEntry>> CreatedLogWriters { get { return _createdLogWriters; } }
		}

	}

}