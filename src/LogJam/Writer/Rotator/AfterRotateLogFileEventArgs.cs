// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AfterRotateLogFileEventArgs.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Rotator
{
	using System;
	using System.IO;


	/// <summary>
	/// Event args for <see cref="RotatingLogFileWriter.AfterRotate"/>.
	/// </summary>
	public class AfterRotateLogFileEventArgs : EventArgs
	{

		private readonly RotatingLogFileWriter _rotatingLogFileWriter;
		private readonly FileInfo _previousLogFile;

		internal AfterRotateLogFileEventArgs(RotatingLogFileWriter rotatingLogFileWriter, FileInfo previousLogFile)
		{
			_rotatingLogFileWriter = rotatingLogFileWriter;
			_previousLogFile = previousLogFile;
		}

		public FileInfo PreviousLogFile { get { return _previousLogFile; } }

		public RotatingLogFileWriter RotatingLogFileWriter { get { return _rotatingLogFileWriter; } }

	}

}
