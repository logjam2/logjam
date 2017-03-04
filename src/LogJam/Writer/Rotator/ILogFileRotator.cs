// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogFileRotator.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writer.Rotator
{
	using System;
	using System.Collections.Generic;
	using System.IO;


	/// <summary>
	/// An object that implements file rotation logic.
	/// </summary>
	public interface ILogFileRotator
	{
		/// <summary>
		/// Event raised by the log file rotator when rotation should begin.
		/// </summary>
		event EventHandler<RotateLogFileEventArgs> TriggerRotate;

		/// <summary>
		/// Called by <see cref="RotatingLogFileWriter"/> to rotate the log files after the previous log file is closed,
		/// and before the next log file is opened.  This method is called in a context that is synchronized with log writing and other operations like flushing.
		/// </summary>
		/// <param name="rotateEventArgs"></param>
		/// <returns>An optional action containing post-rotation cleanup logic that does not need to run as part of the synchronized rotation - eg flush and close 
		/// of the previous log file.</returns>
		Action Rotate(RotatingLogFileWriter rotatingLogFileWriter, RotateLogFileEventArgs rotateEventArgs);

		/// <summary>
		/// Returns the current log file.  Must not return <c>null</c> before the <see cref="TriggerRotate"/> event is raised.
		/// </summary>
		FileInfo CurrentLogFile { get; }

		/// <summary>
		/// Enumerates the log files that exist in the current location.
		/// </summary>
		IEnumerable<FileInfo> EnumerateLogFiles { get; } 
	}

}