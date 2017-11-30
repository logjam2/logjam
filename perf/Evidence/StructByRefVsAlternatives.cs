// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructByRefVsAlternatives.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace Evidence
{
    using System;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Columns;
    using BenchmarkDotNet.Attributes.Exporters;
    using BenchmarkDotNet.Attributes.Jobs;


    /// <summary>
    /// Compares overhead of alternatives for creating and passing LogEntry types to inner entry writers.
    /// </summary>
    [MinColumn, MaxColumn, ClrJob, CoreJob, MemoryDiagnoser, HtmlExporter]
    public class StructByRefVsAlternatives
    {

        private const byte c_recursionDepth = 3;
        private readonly DateTime _dateTime;

        public StructByRefVsAlternatives()
        {
            _dateTime = DateTime.UtcNow;
        }

        [Benchmark]
        public void StructByRef()
        {
            var logEntry = new StructLogEntry(c_recursionDepth, _dateTime, Int32.MaxValue);
            LogStructByRef(ref logEntry);
        }

        private void LogStructByRef(ref StructLogEntry logEntry)
        {
            if (logEntry.remainingRecursionDepth > 0)
            {
                logEntry.remainingRecursionDepth--;
                LogStructByRef(ref logEntry);
            }
        }

        // TODO
        //[Benchmark]
        //public void ReadOnlyStructByRef()
        //{ }


        [Benchmark]
        public void StructCopy()
        {
            var logEntry = new StructLogEntry(c_recursionDepth, _dateTime, Int32.MaxValue);
            LogStructCopy(logEntry);
        }

        private void LogStructCopy(StructLogEntry logEntry)
        {
            if (logEntry.remainingRecursionDepth > 0)
            {
                logEntry.remainingRecursionDepth--;
                LogStructCopy(logEntry);
            }
        }

        [Benchmark]
        public void ClassRef()
        {
            var logEntry = new ClassLogEntry(c_recursionDepth, _dateTime, Int32.MaxValue);
            LogClassRef(logEntry);
        }

        private void LogClassRef(ClassLogEntry logEntry)
        {
            if (logEntry.remainingRecursionDepth > 0)
            {
                logEntry.remainingRecursionDepth--;
                LogClassRef(logEntry);
            }
        }

        [Benchmark]
        public void StructInterfaceByRef()
        {
            ILogEntry logEntry = new StructLogEntry(c_recursionDepth, _dateTime, Int32.MaxValue);
            LogInterfaceByRef(ref logEntry);
        }

        private void LogInterfaceByRef(ref ILogEntry logEntry)
        {
            if (logEntry.RemainingRecursionDepth > 0)
            {
                logEntry.RemainingRecursionDepth--;
                LogInterfaceByRef(ref logEntry);
            }
        }

        [Benchmark]
        public void ClassInterfaceByRef()
        {
            ILogEntry logEntry = new ClassLogEntry(c_recursionDepth, _dateTime, Int32.MaxValue);
            LogInterfaceByRef(ref logEntry);
        }

        private interface ILogEntry
        {
            byte RemainingRecursionDepth { get; set; }
            DateTime DateTime { get; }
            int IntMax { get; }
        }

        private struct StructLogEntry : ILogEntry
        {
            public byte remainingRecursionDepth;
            public DateTime dateTime;
            public int intMax;

            public StructLogEntry(byte remainingRecursionDepth, DateTime dateTime, int intMax)
            {
                this.remainingRecursionDepth = remainingRecursionDepth;
                this.dateTime = dateTime;
                this.intMax = intMax;
            }

            public byte RemainingRecursionDepth { get => remainingRecursionDepth; set => remainingRecursionDepth = value; }
            public DateTime DateTime => dateTime;
            public int IntMax => intMax;
        }

        private class ClassLogEntry : ILogEntry
        {
            public byte remainingRecursionDepth;
            public DateTime dateTime;
            public int intMax;

            public ClassLogEntry(byte remainingRecursionDepth, DateTime dateTime, int intMax)
            {
                this.remainingRecursionDepth = remainingRecursionDepth;
                this.dateTime = dateTime;
                this.intMax = intMax;
            }

            public byte RemainingRecursionDepth { get => remainingRecursionDepth; set => remainingRecursionDepth = value; }
            public DateTime DateTime => dateTime;
            public int IntMax => intMax;
        }

    }

}
