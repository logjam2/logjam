// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTests.cs">
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
    using Xunit.Abstractions;


    /// <summary>
    /// Validates behavior of <see cref="TypeExtensions"/> methods.
    /// </summary>
    public sealed class TypeExtensionsTests
    {
        private readonly ITestOutputHelper _testOutput;

        public TypeExtensionsTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        public static IEnumerable<object[]> CSharpNameTestCases()
        {
            var testCases = new TheoryData<Type, string, bool>
            {
                { typeof(LogManager), "LogJam.LogManager", false },
                { typeof(string), "string", false },
                { new KeyValuePair<string, Type>().GetType(), "System.Collections.Generic.KeyValuePair<string, System.Type>", false },
                { new KeyValuePair<string, Type>().GetType(), "System.Collections.Generic.KeyValuePair", true },
                { new KeyValuePair<Int32, Int64>().GetType(), "System.Collections.Generic.KeyValuePair<int, long>", false },
                { typeof(KeyValuePair<,>), "System.Collections.Generic.KeyValuePair<,>", false },
                { typeof(KeyValuePair<,>), "System.Collections.Generic.KeyValuePair", true },
                { typeof(IEntryWriter<TraceEntry>), "LogJam.Writer.IEntryWriter<LogJam.Trace.TraceEntry>", false },
                { typeof(IEntryWriter<>), "LogJam.Writer.IEntryWriter<>", false },
                { typeof(Nested<,>), "LogJam.Internal.UnitTests.Util.TypeExtensionsTests.Nested<,>", false },
                { typeof(Nested<int, KeyValuePair<int, string>>), "LogJam.Internal.UnitTests.Util.TypeExtensionsTests.Nested<int, System.Collections.Generic.KeyValuePair<int, string>>", false },
                { typeof(Nested<int, KeyValuePair<int, string>>), "LogJam.Internal.UnitTests.Util.TypeExtensionsTests.Nested", true }
            };
            return testCases;
        }

        [Theory]
        [MemberData(nameof(CSharpNameTestCases))]
        public void GetCSharpNameReturnsExpected(Type type, string expectedCSharpName, bool omitTypeParameters)
        {
            var csharpName = type.GetCSharpName(omitTypeParameters);
            _testOutput.WriteLine($"{type} => {csharpName}");
            Assert.Equal(expectedCSharpName, csharpName);
        }

        public class Nested<T, V>
        {
            public T t;
            public V v;
        }
    }

}
