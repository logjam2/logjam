// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTraceFormatter.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace.Format
{
    using System;

    using LogJam.Writer.Text;


    /// <summary>
    /// The debugger trace formatter.
    /// </summary>
    public class DefaultTraceFormatter : EntryFormatter<TraceEntry>
    {

        /// <summary>
        /// The default value for <see cref="MaxIndentLevel" /> if no value is set.
        /// </summary>
        public const int DefaultMaxIndentLevel = 4;

        public DefaultTraceFormatter()
        {
            MaxIndentLevel = DefaultMaxIndentLevel;
        }

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
        /// Set to a value to alter the trace entry indent level from what is set.
        /// </summary>
        public int RelativeIndentLevel { get; set; }

        /// <summary>
        /// Don't indent any trace entries by more than this value.
        /// </summary>
        public int MaxIndentLevel { get; set; }

        #endregion

        #region Formatter methods

        public override void Format(ref TraceEntry traceEntry, FormatWriter formatWriter)
        {
            ColorCategory color = ColorCategory.None;
            if (formatWriter.IsColorEnabled)
            {
                color = TraceLevelToColorCategory(traceEntry.TraceLevel);
            }

            int entryIndentLevel = formatWriter.IndentLevel + RelativeIndentLevel;
            entryIndentLevel = Math.Min(entryIndentLevel, MaxIndentLevel);

            formatWriter.BeginEntry(entryIndentLevel);

            if (IncludeDate)
            {
                formatWriter.WriteDate(traceEntry.TimestampUtc, ColorCategory.Debug);
            }
            if (IncludeTimestamp)
            {
                formatWriter.WriteTimestamp(traceEntry.TimestampUtc, ColorCategory.Detail);
            }
            formatWriter.WriteField(TraceLevelToLabel(traceEntry.TraceLevel), color, 7);
            formatWriter.WriteAbbreviatedTypeName(traceEntry.TracerName, ColorCategory.Debug, 36);
            formatWriter.WriteField(traceEntry.Message.Trim(), color);
            if (traceEntry.Details != null)
            {
                ColorCategory detailColor = color == ColorCategory.Debug ? ColorCategory.Debug : ColorCategory.Detail;
                formatWriter.WriteLines(traceEntry.Details.ToString(), detailColor, 1);
            }

            formatWriter.EndEntry();
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
