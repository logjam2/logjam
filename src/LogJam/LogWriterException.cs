// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWriterException.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
	using System;


	/// <summary>
	/// Thrown when an <see cref="ILogWriter{TEntry}"/> failed to write to a log.
	/// </summary>
	public sealed class LogWriterException : Exception
	{

		private readonly ILogWriter _logWriter;

		public LogWriterException(string message, Exception innerException, ILogWriter logWriter)
		{
			
		}

	}

}