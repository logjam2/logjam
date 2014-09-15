// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TracerConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Trace.Config
{
	using LogJam.Config;
	using LogJam.Util.Collections;
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq;


	/// <summary>
	/// A configuration element used to configure all <see cref="Tracer"/>s that match this <see cref="NamePrefix"/>.
	/// </summary>
	/// <remarks>
	/// <c>TracerConfig</c> instances are immutable.
	/// </remarks>
	public class TracerConfig : NamePrefixTreeNode<TracerConfig>
	{

		private TraceWriter[] _traceWriters;

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TracerConfig"/> class. 
		/// Creates a new <see cref="TracerConfig"/>.
		/// </summary>
		/// <param name="namePrefix">
		/// Matched against <see cref="Tracer.Name"/>s.  The <see cref="TracerConfig"/> that best matches each 
		/// <see cref="Tracer.Name"/> is used to configure that <see cref="Tracer"/>.
		/// </param>
		/// <param name="traceSwitch">
		/// The <see cref="ITraceSwitch"/> instance used for matching <see cref="Tracer"/>s.
		/// </param>
		/// <param name="traceLogWriter">
		/// The <see cref="ILogWriter<TraceEntry>"/> instance used for matching <see cref="Tracer"/>s.
		/// </param>
		public TracerConfig(string namePrefix, ITraceSwitch traceSwitch, ILogWriter<TraceEntry> traceLogWriter)
			: base(NormalizeTraceConfigNamePrefix(namePrefix))
		{
			Contract.Requires<ArgumentNullException>(traceSwitch != null);
			Contract.Requires<ArgumentNullException>(traceLogWriter != null);

			_traceWriters = new[] { new TraceWriter(traceSwitch, traceLogWriter) };
		}

		//public TracerConfig(string namePrefix, params TraceWriter[] traceWriters)
		//	: base(NormalizeTraceConfigNamePrefix(namePrefix))
		//{
		//	Contract.Requires<ArgumentNullException>(traceWriters != null);

		//	_traceWriters = traceWriters;
		//}

		private static string NormalizeTraceConfigNamePrefix(string namePrefix)
		{
			// namePrefix cannot end with a '.' - strip them if present
			return namePrefix.TrimEnd('.', ' ');
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the <see cref="TraceWriter"/> instances used for matching <see cref="Tracer"/>s.
		/// </summary>
		/// <value>
		/// The message collector.
		/// </value>
		internal TraceWriter[] TraceWriters { get { return _traceWriters; } }

		#endregion

		/// <summary>
		/// Replaces the specified <paramref name="traceSwitch"/> and/or <paramref name="traceLogWriter"/> in this <see cref="TracerConfig"/>.
		/// </summary>
		/// <param name="traceSwitch">The new <see cref="ITraceSwitch"/>, or <c>null</c> to use the current single traceswitch.</param>
		/// <param name="traceLogWriter">The new <see cref="ILogWriter{TEntry}"/>, or <c>null</c> to use the current single tracelog writer.</param>
		public void Replace(ITraceSwitch traceSwitch, ILogWriter<TraceEntry> traceLogWriter)
		{
			lock (this)
			{
				TraceWriter replacementTraceWriter;
				if ((traceSwitch == null) || (traceLogWriter == null))
				{
					// Support replacing whichever of traceSwitch, traceLogWriter, are not null.
					if (_traceWriters.Length != 1)
					{
						throw new InvalidOperationException(string.Format("Cannot partially replace TraceWriters for {0} unless a single TraceWriter is attached.", this));
					}
					var currentTraceWriter = _traceWriters[0];

					if (traceSwitch != null)
					{
						replacementTraceWriter = new TraceWriter(traceSwitch, currentTraceWriter.InnerLogWriter);
					}
					else
					{
						replacementTraceWriter = new TraceWriter(currentTraceWriter.TraceSwitch, traceLogWriter);
					}
				}
				else
				{
					replacementTraceWriter = new TraceWriter(traceSwitch, traceLogWriter);
				}

				//Replace(replacementTraceWriter);
			}
		}

		///// <summary>
		///// Replaces <see cref="TraceWriters"/> with the specified <paramref name="traceWriters"/>.
		///// </summary>
		///// <param name="traceWriters">The replacement <see cref="TraceWriter"/>s to use with this <see cref="TracerConfig"/>.</param>
		//public void Replace(params TraceWriter[] traceWriters)
		//{
		//	Contract.Requires<ArgumentNullException>(traceWriters != null);

		//	lock (this)
		//	{
		//		_traceWriters = traceWriters;
		//	}

		//	OnChanged();
		//}

		/// <summary>
		/// Adds the specified <paramref name="traceSwitch"/> and/or <paramref name="traceLogWriter"/> to this <see cref="TracerConfig"/>.
		/// </summary>
		/// <param name="traceSwitch">The new <see cref="ITraceSwitch"/>.</param>
		/// <param name="traceLogWriter">The new trace <see cref="ILogWriter{TEntry}"/>.</param>
		public void Add(ITraceSwitch traceSwitch, ILogWriter<TraceEntry> traceLogWriter)
		{
			Contract.Requires<ArgumentNullException>(traceSwitch != null);
			Contract.Requires<ArgumentNullException>(traceLogWriter != null);

			//Add(new TraceWriter(traceSwitch, traceLogWriter));
		}

		/// <summary>
		///// Adds the specified <paramref name="traceWriters"/> to <see cref="TraceWriters"/>.
		///// </summary>
		///// <param name="traceWriters">The replacement <see cref="TraceWriter"/>s to use with this <see cref="TracerConfig"/>.</param>
		//public void Add(params TraceWriter[] traceWriters)
		//{
		//	Contract.Requires<ArgumentNullException>(traceWriters != null);

		//	lock (this)
		//	{
		//		_traceWriters = _traceWriters.Concat(traceWriters).ToArray();
		//	}

		//	OnChanged();
		//}

		/// <summary>
		/// An event that is raised when this <see cref="TracerConfig"/> is changed.
		/// </summary>
		public event EventHandler<ConfigChangedEventArgs<TracerConfig>> Changed;

		private void OnChanged()
		{
			var handlers = Changed;
			if (handlers != null)
			{
				handlers(this, new ConfigChangedEventArgs<TracerConfig>(this));
			}
		}

		public override string ToString()
		{
			return string.Format("TracerConfig(\"{0}\")", NamePrefix);
		}

	}
}
