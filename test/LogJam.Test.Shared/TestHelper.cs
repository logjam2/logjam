// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestHelper.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Test.Shared
{
    using System;

    using LogJam.Trace;


    /// <summary>
    /// Helper methods used by unit tests.
    /// </summary>
    public static class TestHelper
    {

        public static void WarnException(Tracer tracer, int callstackDepth)
        {
            try
            {
                ThrowExceptionWithCallStack(callstackDepth);
            }
            catch (Exception excp)
            {
                tracer.Warn(excp, "Warning exception");
            }
        }

        public static void ThrowExceptionWithCallStack(int callstackDepth)
        {
            ThrowExceptionWithCallStack(callstackDepth, new ApplicationException("Exception to show tracing an exception"));
        }

        public static void ThrowExceptionWithCallStack(int callstackDepth, Exception exception)
        {
            if (callstackDepth > 1)
            {
                // Recurse
                ThrowExceptionWithCallStack(callstackDepth - 1, exception);
            }
            else
            {
                throw exception;
            }
        }

        public static int CountLineBreaks(string text)
        {
            int countLineBreaks = 0;
            int len = text.Length;
            for (int i = 0;;)
            {
                i = text.IndexOfAny(new[] { '\r', '\n' }, i);
                if (i < 0)
                {
                    return countLineBreaks;
                }
                else
                {
                    if ((text[i] == '\r') && (i + 1 < len) && (text[i + 1] == '\n'))
                    {
                        countLineBreaks++;
                        i += 2;
                    }
                    else
                    {
                        countLineBreaks++;
                        i++;
                    }
                }
            }
        }

    }

}
