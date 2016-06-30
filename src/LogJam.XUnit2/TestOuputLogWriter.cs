// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOuputLogWriter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2
{
	using System;
	using System.Diagnostics.Contracts;

	using LogJam.Trace;
	using LogJam.Writer;

	using Util;

	using Xunit.Abstractions;

	/// <summary>
	/// A <see cref="ILogWriter" />s that writes log entries to a to a test's xunit 2.0 test output.
	/// Log entries are written to an <see cref="ITestOutputHelper" /> 
	/// Allows changing the <see cref="ITestOutputHelper" /> which enables re-using the writer for tests in a test collection.
	/// </summary>
	public sealed class TestOuputLogWriter : BaseLogWriter
	{
		private readonly ITestOutputLogWriterConfig _logWriterConfig;
		private ILogWriter _innerLogWriter;

		public TestOuputLogWriter(
			ITracerFactory tracerFactory,
			ITestOutputLogWriterConfig logWriterConfig)
			: base(tracerFactory)
		{
			Contract.Requires<ArgumentNullException>(logWriterConfig != null);

			_logWriterConfig = logWriterConfig;
			CreateLogWriterAndEntries();
		}

		public void SetTestOutput(ITestOutputHelper testOutput)
		{
			Contract.Requires<ArgumentNullException>(testOutput != null);

			var cleanupAction = SwitchTestOutput(testOutput);
			cleanupAction();
		}

		#region ILogWriter

		public override bool IsSynchronized => true;

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				(_innerLogWriter as IDisposable)?.Dispose();
			}
		}

		protected override void InternalStart()
		{
			lock (this)
			{
				(_innerLogWriter as IStartable)?.Start();
				base.InternalStart();
			}
		}

		protected override void InternalStop()
		{
			lock (this)
			{
				(_innerLogWriter as IStartable)?.Stop();
				base.InternalStop();
			}
		}

		#region private 

		private Action SwitchTestOutput(ITestOutputHelper testOutput)
		{
			lock (this)
			{
				var previousLogWriter = _innerLogWriter;
				_logWriterConfig.TestOutput = testOutput;
				_innerLogWriter = _logWriterConfig.CreateLogWriter(SetupTracerFactory);
				(_innerLogWriter as IStartable)?.Start();
				foreach (var entryWriter in EntryWriters)
				{
					(entryWriter.Value as ITestOutputEntryWriter)?.SwitchLogWriter(_innerLogWriter);
				}
				var cleanupAction = new Action(() =>
										  {
											  (previousLogWriter as IStartable)?.Stop();
											  (previousLogWriter as IDisposable)?.Dispose();
										  });
				return cleanupAction;
			}
		}

		private void CreateLogWriterAndEntries()
		{
			_innerLogWriter = _logWriterConfig.CreateLogWriter(SetupTracerFactory);
			AddEntryWriters(); 
		}

		private void AddEntryWriters()
		{
			foreach (var kvp in _innerLogWriter.EntryWriters)
			{
				Type entryWriterEntryType = kvp.Key;
				object innerEntryWriter = kvp.Value;

				AddEntryWriter(innerEntryWriter, entryWriterEntryType);
			}
		}

		private void AddEntryWriter(object innerEntryWriter, Type entryWriterEntryType)
		{
			var entryTypeArgs = new[] { entryWriterEntryType };
			var newEntryWriter = this.InvokeGenericMethod(
														  entryTypeArgs,
														  "CreateTestOutputWriterEntry",
														  innerEntryWriter);
			this.InvokeGenericMethod(entryTypeArgs, "AddEntryWriter", newEntryWriter);
		}

		private IEntryWriter<TEntry> CreateTestOutputWriterEntry<TEntry>(IEntryWriter<TEntry> innerEntryWriter)
			where TEntry : ILogEntry
		{
			return new TestOutputEntryWriter<TEntry>(SetupTracerFactory, innerEntryWriter);
		}


		private interface ITestOutputEntryWriter
		{

			void SwitchLogWriter(ILogWriter newLogWriter);

		}


		private class TestOutputEntryWriter<TEntry> : ProxyEntryWriter<TEntry>, ITestOutputEntryWriter
			where TEntry : ILogEntry
		{
			private readonly ITracerFactory _setupTracerFactory;

			public TestOutputEntryWriter(
				ITracerFactory setupTracerFactory,
				IEntryWriter<TEntry> innerEntryWriter)
				: base(innerEntryWriter)
			{
				_setupTracerFactory = setupTracerFactory;
			}

			public void SwitchLogWriter(ILogWriter newLogWriter)
			{
				IEntryWriter<TEntry> newInnerEntryWriter;
				if (! newLogWriter.TryGetEntryWriter<TEntry>(out newInnerEntryWriter))
				{
					var tracer = _setupTracerFactory.TracerFor(this);
					tracer.Error($"Unable to get EntryWriter of type {typeof(TEntry).GetCSharpName()} from log writer {newLogWriter}; disabling logging of {typeof(TEntry).GetCSharpName()}");
					newInnerEntryWriter = new NoOpEntryWriter<TEntry>();
				}

				(InnerEntryWriter as IStartable)?.Stop();
				InnerEntryWriter = newInnerEntryWriter;
				(InnerEntryWriter as IStartable)?.Start();
			}

		}

		#endregion
	}
}
