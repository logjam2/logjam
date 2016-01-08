// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeTestOutputHelper.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2.UnitTests
{
    using System.Text;

    using Xunit.Abstractions;


    /// <summary>
    /// A test fake for <see cref="ITestOutputHelper" />.
    /// </summary>
    internal sealed class FakeTestOutputHelper : ITestOutputHelper
    {

        private readonly StringBuilder _sb;

        public FakeTestOutputHelper()
        {
            _sb = new StringBuilder(1024);
        }

        public FakeTestOutputHelper(StringBuilder sb)
        {
            _sb = sb;
        }

        public void WriteLine(string message)
        {
            _sb.AppendLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            _sb.AppendFormat(format, args);
            _sb.AppendLine();
        }

        /// <summary>
        /// Returns the accumulated test output.
        /// </summary>
        /// <returns></returns>
        public string GetTestOutput()
        {
            return _sb.ToString();
        }

    }

}
