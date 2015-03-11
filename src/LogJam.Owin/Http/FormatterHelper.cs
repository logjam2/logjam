// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatterHelper.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Owin.Http
{
	using System.IO;

	using Microsoft.Owin;


	/// <summary>
	/// Helper methods for log formatting.
	/// </summary>
	internal static class FormatterHelper
	{

		internal static void FormatHeaders(TextWriter textWriter, IReadableStringCollection headerDictionary)
		{
			foreach (var header in headerDictionary)
			{
				foreach (string value in header.Value)
				{
					textWriter.Write(header.Key);
					textWriter.Write(": ");
					textWriter.WriteLine(value);
				}
			}
		}
	}

}