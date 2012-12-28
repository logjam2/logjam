// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamTraceListener.cs" company="Crim Consulting">
// Copyright (c) 2011-2012 Crim Consulting.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Interop
{
	using System;
	using System.Diagnostics;

	/// <summary>
	/// A <see cref="TraceListener" /> implementation that allows <see cref="Trace"/> and <see cref="TraceSource"/> methods to write to <see cref="LogJam.Trace"/> classes.
	/// </summary>
	public class LogJamTraceListener : TraceListener
	{
		/// <summary>
		/// Initializes a new instance of <see cref='LogJamTraceListener'/>.
		/// </summary>
		public LogJamTraceListener()
		{}

		/// <summary>
		/// Initializes a new instance of <see cref='LogJamTraceListener'/> class using the specified name for the listener.
		/// </summary>
		/// <param name="name"></param>
		public LogJamTraceListener(string name)
			: base(name)
		{}

		/// <summary>
		/// <see cref="LogJamTraceListener"/> is always threadsafe.
		/// </summary>
		/// <returns><c>true</c></returns>
		public override bool IsThreadSafe
		{
			get { return true; }
		}

		public override void Flush()
		{
			base.Flush();
		}

		/// <summary>
		/// Displays a failure message.
		/// </summary>
		/// <param name="message">Failure message.</param>
		/// <param name="detailMessage">Detailed message describing the failure. May be <c>null</c>.</param>
		public override void Fail(string message, string detailMessage)
		{
			// TODO
		}

		/// <summary>
		/// Writes the specified message to <see cref="LogJam.Trace"/>.
		/// </summary>
		/// <param name="message"></param>
		public override void Write(string message)
		{
			// TODO
		}

		/// <summary>
		/// Writes <paramref name="o"/>'s <see cref="Object.ToString"/> to <see cref="LogJam.Trace"/>
		/// </summary>
		/// <param name="o"></param>
		public override void Write(object o)
		{
			// REVIEW: Use Filter or not?
			if (Filter != null && !Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, null, null, o, null))
				return;

			if (o == null) return;
			Write(o.ToString());
		}

		/// <summary>
		/// Writes a category and a message to <see cref="LogJam.Trace"/>
		/// </summary>
		/// <param name="message">The message to trace.</param>
		/// <param name="category">Name of the <see cref="Tracer"/>.</param>
		public override void Write(string message, string category)
		{
			// REVIEW: Use Filter or not?
			if (Filter != null && !Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, message, null, null, null))
				return;

			// TODO
		}

		/// <devdoc>
		/// <para>Writes a category name and the name of the <paramref name="o"/> parameter to the listener you
		///    specify when you inherit from the <see cref='System.Diagnostics.TraceListener'/>
		///    class.</para> 
		/// </devdoc>
		public override void Write(object o, string category)
		{
			// REVIEW: Use Filter or not?
			if (Filter != null && !Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, category, null, o, null))
				return;

			// TODO
		}

		/// <devdoc> 
		///    <para>When overridden in a derived class, writes a message to the listener you specify in
		///       the derived class, followed by a line terminator. The default line terminator is a carriage return followed 
		///       by a line feed (\r\n).</para>
		/// </devdoc>
		public abstract void WriteLine(string message);

		/// <devdoc>
		/// <para>Writes the name of the <paramref name="o"/> parameter to the listener you specify when you inherit from the <see cref='System.Diagnostics.TraceListener'/> class, followed by a line terminator. The default line terminator is a 
		///    carriage return followed by a line feed 
		///    (\r\n).</para>
		/// </devdoc> 
		public virtual void WriteLine(object o)
		{
			if (Filter != null && !Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, null, null, o))
				return;

			WriteLine(o == null ? "" : o.ToString());
		}

		/// <devdoc>
		///    <para>Writes a category name and a message to the listener you specify when you 
		///       inherit from the <see cref='System.Diagnostics.TraceListener'/> class,
		///       followed by a line terminator. The default line terminator is a carriage return followed by a line feed (\r\n).</para>
		/// </devdoc>
		public virtual void WriteLine(string message, string category)
		{
			if (Filter != null && !Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, message))
				return;
			if (category == null)
				WriteLine(message);
			else
				WriteLine(category + ": " + ((message == null) ? string.Empty : message));
		}

		/// <devdoc> 
		///    <para>Writes a category
		///       name and the name of the <paramref name="o"/>parameter to the listener you 
		///       specify when you inherit from the <see cref='System.Diagnostics.TraceListener'/> 
		///       class, followed by a line terminator. The default line terminator is a carriage
		///       return followed by a line feed (\r\n).</para> 
		/// </devdoc>
		public virtual void WriteLine(object o, string category)
		{
			if (Filter != null && !Filter.ShouldTrace(null, "", TraceEventType.Verbose, 0, category, null, o))
				return;

			WriteLine(o == null ? "" : o.ToString(), category);
		}


		// new write methods used by TraceSource

		[
		ComVisible(false)
		]
		public virtual void TraceData(TraceEventCache eventCache, String source, TraceEventType eventType, int id, object data)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data))
				return;

			WriteHeader(source, eventType, id);
			string datastring = String.Empty;
			if (data != null)
				datastring = data.ToString();

			WriteLine(datastring);
			WriteFooter(eventCache);
		}

		[
		ComVisible(false)
		]
		public virtual void TraceData(TraceEventCache eventCache, String source, TraceEventType eventType, int id, params object[] data)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
				return;

			WriteHeader(source, eventType, id);

			StringBuilder sb = new StringBuilder();
			if (data != null)
			{
				for (int i = 0; i < data.Length; i++)
				{
					if (i != 0)
						sb.Append(", ");

					if (data[i] != null)
						sb.Append(data[i].ToString());
				}
			}
			WriteLine(sb.ToString());

			WriteFooter(eventCache);
		}

		[
		ComVisible(false)
		]
		public virtual void TraceEvent(TraceEventCache eventCache, String source, TraceEventType eventType, int id)
		{
			TraceEvent(eventCache, source, eventType, id, String.Empty);
		}

		// All other TraceEvent methods come through this one.
		[
		ComVisible(false)
		]
		public virtual void TraceEvent(TraceEventCache eventCache, String source, TraceEventType eventType, int id, string message)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message))
				return;

			WriteHeader(source, eventType, id);
			WriteLine(message);

			WriteFooter(eventCache);
		}

		[
		ComVisible(false)
		]
		public virtual void TraceEvent(TraceEventCache eventCache, String source, TraceEventType eventType, int id, string format, params object[] args)
		{
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args))
				return;

			WriteHeader(source, eventType, id);
			if (args != null)
				WriteLine(String.Format(CultureInfo.InvariantCulture, format, args));
			else
				WriteLine(format);

			WriteFooter(eventCache);
		}

		[
		ComVisible(false)
		]
		public virtual void TraceTransfer(TraceEventCache eventCache, String source, int id, string message, Guid relatedActivityId)
		{
			TraceEvent(eventCache, source, TraceEventType.Transfer, id, message + ", relatedActivityId=" + relatedActivityId.ToString());
		}

		private void WriteHeader(String source, TraceEventType eventType, int id)
		{
			Write(String.Format(CultureInfo.InvariantCulture, "{0} {1}: {2} : ", source, eventType.ToString(), id.ToString(CultureInfo.InvariantCulture)));
		}

		[ResourceExposure(ResourceScope.None)]
		[ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
		private void WriteFooter(TraceEventCache eventCache)
		{
			if (eventCache == null)
				return;

			indentLevel++;
			if (IsEnabled(TraceOptions.ProcessId))
				WriteLine("ProcessId=" + eventCache.ProcessId);

			if (IsEnabled(TraceOptions.LogicalOperationStack))
			{
				Write("LogicalOperationStack=");
				Stack operationStack = eventCache.LogicalOperationStack;
				bool first = true;
				foreach (Object obj in operationStack)
				{
					if (!first)
						Write(", ");
					else
						first = false;

					Write(obj.ToString());
				}
				WriteLine(String.Empty);
			}

			if (IsEnabled(TraceOptions.ThreadId))
				WriteLine("ThreadId=" + eventCache.ThreadId);

			if (IsEnabled(TraceOptions.DateTime))
				WriteLine("DateTime=" + eventCache.DateTime.ToString("o", CultureInfo.InvariantCulture));

			if (IsEnabled(TraceOptions.Timestamp))
				WriteLine("Timestamp=" + eventCache.Timestamp);

			if (IsEnabled(TraceOptions.Callstack))
				WriteLine("Callstack=" + eventCache.Callstack);
			indentLevel--;
		}
	}
}