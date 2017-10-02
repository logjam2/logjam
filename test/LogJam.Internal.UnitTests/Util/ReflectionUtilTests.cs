// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionUtilTests.cs">
// Copyright (c) 2011-2017 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal.UnitTests.Util
{
    using System;
    using System.Collections.Generic;

    using LogJam.Trace;
    using LogJam.Util;
    using LogJam.Writer;

    using Xunit;


    /// <summary>
    /// Validates behavior of <see cref="ReflectionUtil"/> methods.
    /// </summary>
    public sealed class ReflectionUtilTests
    {

        public static IEnumerable<object[]> CSharpNameTestCases()
        {
            var testCases = new TheoryData<Type, string>();
            testCases.Add(typeof(LogManager), "LogJam.LogManager");
            testCases.Add(typeof(string), "string");
            testCases.Add(new KeyValuePair<string, Type>().GetType(), "System.Collections.Generic.KeyValuePair<string, System.Type>");
            testCases.Add(new KeyValuePair<Int32, Int64>().GetType(), "System.Collections.Generic.KeyValuePair<int, long>");
            testCases.Add(typeof(KeyValuePair<,>), "System.Collections.Generic.KeyValuePair<, >");
            testCases.Add(typeof(IEntryWriter<TraceEntry>), "LogJam.Writer.IEntryWriter<LogJam.Trace.TraceEntry>");
            testCases.Add(typeof(IEntryWriter<>), "LogJam.Writer.IEntryWriter<>");
            return testCases;
        }

        [Theory]
        [MemberData(nameof(CSharpNameTestCases))]
        public void GetCSharpNameReturnsExpected(Type type, string expectedCSharpName)
        {
            Assert.Equal(expectedCSharpName, type.GetCSharpName());
        }


    }

}
