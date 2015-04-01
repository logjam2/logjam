// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextLogWriterConfig.cs">
// Copyright (c) 2011-2015 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------

namespace LogJam.Config
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using LogJam.Format;
	using LogJam.Util;
	using LogJam.Writer;


	/// <summary>
	/// Configures use of a <see cref="TextLogWriter"/>.
	/// </summary>
	public abstract class TextLogWriterConfig : LogWriterConfig
	{

		// Configured formatters are stored as a list of logentry Type, logentry formatter, configure action
		private readonly List<Tuple<Type, object, Action<TextLogWriter>>> _formatters;

		/// <summary>
		/// Initializes a new <see cref="TextLogWriterConfig"/>.
		/// </summary>
		protected TextLogWriterConfig()
		{
			_formatters = new List<Tuple<Type, object, Action<TextLogWriter>>>();
		}

		/// <summary>
		/// Adds formatting for entry types <typeparamref name="TEntry"/> using <paramref name="formatter"/>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="formatter"></param>
		/// <returns></returns>
		public TextLogWriterConfig Format<TEntry>(LogFormatter<TEntry> formatter)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(formatter != null);

			Action<TextLogWriter> configureAction = (mw) => mw.AddFormat(formatter);

			_formatters.Add(new Tuple<Type, object, Action<TextLogWriter>>(typeof(TEntry), formatter, configureAction));
			return this;
		}

		/// <summary>
		/// Adds formatting for entry types <typeparamref name="TEntry"/> using <paramref name="formatAction"/>.
		/// </summary>
		/// <typeparam name="TEntry"></typeparam>
		/// <param name="formatAction"></param>
		/// <returns></returns>
		public TextLogWriterConfig Format<TEntry>(FormatAction<TEntry> formatAction)
			where TEntry : ILogEntry
		{
			Contract.Requires<ArgumentNullException>(formatAction != null);

			return Format((LogFormatter<TEntry>) formatAction);
		}

		#region ILogWriterConfig

		public override bool Equals(ILogWriterConfig other)
		{
			TextLogWriterConfig typedOther = other as TextLogWriterConfig;
			if (typedOther == null)
			{
				return false;
			}

			return base.Equals(typedOther) && EqualityUtil.AreEquivalent(_formatters, typedOther._formatters);
		}

		#endregion

		/// <summary>
		/// Applies all configured formatters to <paramref name="writer"/>.
		/// </summary>
		/// <param name="writer"></param>
		protected void ApplyConfiguredFormatters(TextLogWriter writer)
		{
			foreach (var formatterTuple in _formatters)
			{
				var configureFormatterAction = formatterTuple.Item3;
				configureFormatterAction(writer);
			}
		}

	}

}
