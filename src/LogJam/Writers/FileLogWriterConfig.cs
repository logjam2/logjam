// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogWriterConfig.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Writers
{
	using LogJam.Config;
	using LogJam.Util;


	/// <summary>
	/// Configures a log writer that writes to the specified <see cref="File"/>.
	/// </summary>
	public sealed class FileLogWriterConfig<TEntry> : LogWriterConfig<TEntry> where TEntry : ILogEntry
	{
		public string Directory { get; set; }

		public string File { get; set; }

		public override ILogWriter<TEntry> CreateLogWriter()
		{
			throw new System.NotImplementedException();
		}

		public override bool Equals(ILogWriterConfig other)
		{
			if (! base.Equals(other))
			{
				return false;
			}
			var otherSameType = other as FileLogWriterConfig<TEntry>;
			if (otherSameType == null)
			{
				return false;
			}

			return string.Equals(Directory, otherSameType.Directory) && string.Equals(File, otherSameType.File);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ Directory.Hash(6) ^ File.Hash(3);
		}

	}

}