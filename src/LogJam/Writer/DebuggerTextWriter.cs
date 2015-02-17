// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerLogWriter.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Writer
{
	using System;
	using System.IO;
	using System.Text;


	/// <summary>
	/// A <see cref="TextWriter"/> that writes output to a debugger window.
	/// </summary>
	internal class DebuggerTextWriter : TextWriter
	{

		public override Encoding Encoding { get { return Encoding.UTF8; } }

		public override void Write(string value)
		{
#if (PORTABLE)
			// REVIEW: This isn't reliable - it is conditionally compiled in debug builds; but it's all that's available in the portable profile.
			Debug.WriteLine(value);
#else
			System.Diagnostics.Trace.Write(value);
#endif
		}

		public override void WriteLine(string value)
		{
#if (PORTABLE)
			// REVIEW: This isn't reliable - it is conditionally compiled in debug builds; but it's all that's available in the portable profile.
			Debug.WriteLine(value);
#else
			System.Diagnostics.Trace.WriteLine(value);
#endif
		}

		public override void Write(char[] buffer, int index, int count)
		{
			Write(new string(buffer, index, count));
		}

		public override void Write(char value)
		{
			Write(value.ToString());
		}

	}

}
