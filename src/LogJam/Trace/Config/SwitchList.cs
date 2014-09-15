// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchList.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Config
{
	using LogJam.Util;


	/// <summary>
	/// Holds the switches for a <see cref="TraceWriterConfig"/> as a set of key-value pairs,
	/// where the key is a name prefix, and the value is a <see cref="ITraceSwitch"/>.
	/// </summary>
	internal sealed class SwitchList : ListDictionary<string, ITraceSwitch>
	{

		/// <summary>
		/// Finds the <see cref="ITraceSwitch"/> in this list that containsprefix matches <paramref name="tracerName"/>.
		/// No matching switch may be found, in which case <c>false</c> is returned.
		/// </summary>
		/// <param name="tracerName"></param>
		/// <param name="traceSwitch"></param>
		/// <returns></returns>
		public bool FindBestMatchingSwitch(string tracerName, out ITraceSwitch traceSwitch)
		{
			int bestMatchLength = -1;
			traceSwitch = null;

			foreach (var kvp in this)
			{
				if (kvp.Key.Length > bestMatchLength)
				{
					if (tracerName.StartsWith(kvp.Key))
					{
						bestMatchLength = kvp.Key.Length;
						traceSwitch = kvp.Value;
					}
				}
			}

			return bestMatchLength >= 0;
		}

	}

}