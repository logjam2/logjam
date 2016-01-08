// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensionsTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Util.Text
{
    using System.Text;

    using LogJam.Util.Text;

    using Xunit;


    /// <summary>
    /// Validates functionality of <see cref="StringBuilderExtensions"/>.
    /// </summary>
    public sealed class StringBuilderExtensionsTests
    {

        [Theory]
        [InlineData(0, 0, "0")]
		[InlineData(0, 1, "0")]
		[InlineData(0, 2, "00")]
		[InlineData(0, 4, "0000")]
		[InlineData(-1, 0, "-1")]
        [InlineData(-1, 3, "-01")]
        [InlineData(-9, 2, "-9")]
        [InlineData(-9, 4, "-009")]
        [InlineData(-1000, 6, "-01000")]
        [InlineData(1, 2, "01")]
        [InlineData(9, 2, "09")]
        [InlineData(10, 2, "10")]
        [InlineData(10, 3, "010")]
        [InlineData(10, 5, "00010")]
        [InlineData(999, 3, "999")]
        [InlineData(999, 5, "00999")]
        [InlineData(1000, 3, "1000")]
        [InlineData(1000, 5, "01000")]
        [InlineData(990800000, 10, "0990800000")]
        [InlineData(2000000000, 10, "2000000000")]
        public void VerifyAppendPadZeroes(int number, int width, string expectedText)
        {
            var sb = new StringBuilder();
            sb.AppendPadZeroes(number, width);
            Assert.Equal(expectedText, sb.ToString());
        }

    }

}