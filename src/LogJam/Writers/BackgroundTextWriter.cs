// // -----------------------------------------------------------------------
// <copyright file="BackgroundTextWriter.cs" company="Adap.tv">
// Copyright (c) 2015 Adap.tv.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using LogJam.Log.Api;

namespace LogJam.Log
{

	/// <summary>
	/// Supports all <see cref="TextWriter"/> operations by delegating to a background <c>TextWriter</c>
	/// on another thread.  This is needed because <see cref="TextWriter.Synchronized"/> turns all async operations
	/// into blocking ones.  This implementation is truly non-blocking, even for synchronous operations.
	/// </summary>
	public sealed class BackgroundTextWriter : TextWriter, ILogWriter<string>
	{

		private readonly TextWriter _innerTextWriter;

		public BackgroundTextWriter(TextWriter innerTextWriter)
		{
			Contract.Requires<ArgumentNullException>(innerTextWriter != null);

			_innerTextWriter = innerTextWriter;
		}

		#region TextWriter overrides

		public override Encoding Encoding
		{
			get { return _innerTextWriter.Encoding; }
		}

		#endregion

		/// <summary>
		/// Defines each of the supported background operations.
		/// </summary>
		private enum Op : byte
		{

			

		}
		/// <summary>
		/// Encapsulates all the info to run a TextWriter API call in the background thread.
		/// </summary>
		private struct Call
		{

			private string text;



		}



	}

}