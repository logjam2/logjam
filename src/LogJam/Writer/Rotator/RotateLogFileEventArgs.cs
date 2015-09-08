// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotateLogFileEventArgs.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Rotator
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;


	/// <summary>
	/// Event args for <see cref="ILogFileRotator.TriggerRotate"/>.
	/// </summary>
	public class RotateLogFileEventArgs : EventArgs
	{

		private readonly ILogFileRotator _logFileRotator;
		private readonly FileInfo _currentLogFile;
		private readonly FileInfo _nextLogFile;

		public RotateLogFileEventArgs(ILogFileRotator logFileRotator, FileInfo currentLogFile, FileInfo nextLogFile)
		{
			Contract.Requires<ArgumentNullException>(logFileRotator != null);
			Contract.Requires<ArgumentNullException>(nextLogFile != null);

			_logFileRotator = logFileRotator;
			_currentLogFile = currentLogFile;
			_nextLogFile = nextLogFile;
		}

		public ILogFileRotator LogFileRotator { get { return _logFileRotator; } }

		public FileInfo CurrentLogFile { get { return _currentLogFile; } }

		public FileInfo NextLogFile { get { return _nextLogFile; } }

	}
}
