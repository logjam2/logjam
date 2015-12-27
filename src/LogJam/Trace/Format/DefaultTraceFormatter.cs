// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTraceFormatter.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Format
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    using LogJam.Writer.Text;


    /// <summary>
    /// The debugger trace formatter.
    /// </summary>
    public class DefaultTraceFormatter : EntryFormatter<TraceEntry>
    {

        public DefaultTraceFormatter()
        {}

        #region Public Properties

        /// <summary>
        /// <c>true</c> to include the Date when formatting <see cref="TraceEntry" />s.
        /// </summary>
        public bool IncludeDate { get; set; }

        /// <summary>
        /// <c>true</c> to include the Timestamp when formatting <see cref="TraceEntry" />s.
        /// </summary>
        public bool IncludeTimestamp { get; set; }

        /// <summary>
        /// We don't yet support indenting based on context, but we do support indenting a constant amount. 
        /// </summary>
        public int IndentLevel { get; set; }

        #endregion

        #region Formatter methods

        public override void Format(ref TraceEntry traceEntry, FormatWriter writer)
        {
            ColorCategory color = ColorCategory.None;
            if (writer.IsColorEnabled)
            {
                color = TraceLevelToColorCategory(traceEntry.TraceLevel);
            }

            writer.BeginEntry(writer.IndentLevel + IndentLevel);

            if (IncludeDate)
            {
                writer.WriteDate(traceEntry.TimestampUtc, ColorCategory.Debug);
            }
            if (IncludeTimestamp)
            {
                writer.WriteTimestamp(traceEntry.TimestampUtc, ColorCategory.Debug);
            }
            writer.WriteField(TraceLevelToLabel(traceEntry.TraceLevel), color, 7);
            writer.WriteAbbreviatedTypeName(traceEntry.TracerName, ColorCategory.Debug, 40);
            writer.WriteField(traceEntry.Message.Trim(), color);
            if (traceEntry.Details != null)
            {
                ColorCategory detailColor = color == ColorCategory.Debug ? ColorCategory.Debug : ColorCategory.Detail;
                writer.WriteLines(traceEntry.Details.ToString(), detailColor, 1);
            }

            writer.EndEntry();
        }

        #endregion

        protected ColorCategory TraceLevelToColorCategory(TraceLevel traceLevel)
        {
            switch (traceLevel)
            {
                case TraceLevel.Info:
                    return ColorCategory.Info;
                case TraceLevel.Verbose:
                    return ColorCategory.Detail;
                case TraceLevel.Debug:
                    return ColorCategory.Debug;
                case TraceLevel.Warn:
                    return ColorCategory.Warning;
                case TraceLevel.Error:
                    return ColorCategory.Error;
                case TraceLevel.Severe:
                    return ColorCategory.SevereError;
                default:
                    return ColorCategory.None;
            }
        }

        protected string TraceLevelToLabel(TraceLevel traceLevel)
        {
            switch (traceLevel)
            {
                case TraceLevel.Info:
                    return "Info";
                case TraceLevel.Verbose:
                    return "Verbose";
                case TraceLevel.Debug:
                    return "Debug";
                case TraceLevel.Warn:
                    return "Warn";
                case TraceLevel.Error:
                    return "Error";
                case TraceLevel.Severe:
                    return "SEVERE";
                default:
                    return "";
            }
        }

    }
}
