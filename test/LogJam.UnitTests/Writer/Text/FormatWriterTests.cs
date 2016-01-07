// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatWriterTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Writer.Text
{
    using System.IO;
    using System.Text;

    using LogJam.Writer.Text;

    using Xunit;


    /// <summary>
    /// Tests <see cref="FormatWriter"/> functionality.
    /// </summary>
    public sealed class FormatWriterTests
    {

        [Theory]
        [InlineData("", 0, "")]
        [InlineData("a", 0, "a\r\n")]
        [InlineData("a\rb", 1, "   a\r\n   b\r\n")]
        [InlineData("a\r\nb\r\n\r\nc\r\n", 1, "   a\r\n   b\r\n   c\r\n")]
        [InlineData("a\r\n \r\nc", 1, "   a\r\n    \r\n   c\r\n")]
        public void WriteLinesStringWorks(string lines, int indentLevel, string expectedOutput)
        {
            // Prepare
            var setupLog = new SetupLog();
            var sbOut = new StringBuilder();
            var textWriter = new StringWriter(sbOut);
            using (var formatWriter = new TextWriterFormatWriter(setupLog, textWriter))
            {
                // Test
                formatWriter.WriteLines(lines, relativeIndentLevel: indentLevel);
            }

            // Verify
            Assert.Equal(expectedOutput, sbOut.ToString());
        }

        [Theory]
        [InlineData("", 0, "")]
        [InlineData("a", 0, "a\r\n")]
        [InlineData("a\rb", 1, "   a\r\n   b\r\n")]
        [InlineData("a\r\nb\r\n\r\nc\r\n", 1, "   a\r\n   b\r\n   c\r\n")]
        [InlineData("a\r\n \r\nc", 1, "   a\r\n    \r\n   c\r\n")]
        public void WriteLinesStringBuilderWorks(string lines, int indentLevel, string expectedOutput)
        {
            // Prepare
            var setupLog = new SetupLog();
            var sbOut = new StringBuilder();
            var textWriter = new StringWriter(sbOut);
            using (var formatWriter = new TextWriterFormatWriter(setupLog, textWriter))
            {
                // Test
                formatWriter.WriteLines(new StringBuilder(lines), relativeIndentLevel: indentLevel);
            }

            // Verify
            Assert.Equal(expectedOutput, sbOut.ToString());
        }

    }

}