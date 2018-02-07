// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterTests.cs">
// Copyright (c) 2011-2018 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


using System.IO;

using LogJam.Config;
using LogJam.Trace;
using LogJam.Trace.Config;

using Xunit;

namespace LogJam.UnitTests.Writer.Text
{

    /// <summary>
    /// Validates logging to a <see cref="TextWriter"/>
    /// </summary>
    public sealed class TextWriterTests
    {

        [Fact(DisplayName = "TextWriter that is passed to LogManagerConfig is not disposed on LogManager.Dispose")]
        public void ExternalTextWriter_NotDisposed()
        {
            var sw = new DisposeTrackingStringWriter();

            using (var logManager = new LogManager())
            using (var traceManager = new TraceManager(logManager))
            {
                var textWriterConfig = logManager.Config.UseTextWriter(sw);
                Assert.False(textWriterConfig.DisposeTextWriter);
                traceManager.Config.TraceTo(textWriterConfig);

                var tracer = traceManager.TracerFor(this);

                tracer.Info(nameof(ExternalTextWriter_NotDisposed));
            }

            Assert.Contains(nameof(ExternalTextWriter_NotDisposed), sw.ToString());
            Assert.False(sw.IsDisposed);
        }


        private class DisposeTrackingStringWriter : StringWriter
        {
            public bool IsDisposed { get; private set; }

            #region Overrides of StringWriter

            protected override void Dispose(bool disposing)
            {
                IsDisposed = true;
                base.Dispose(disposing);
            }

            #endregion

        }


    }

}
