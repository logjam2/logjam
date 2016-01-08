// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TracerExtensions.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Trace
{
    using System;


    /// <summary>
    /// Extension methods for <see cref="Tracer" />.
    /// </summary>
    public static class TracerExtensions
    {

        /// <summary>
        /// Gets a value indicating whether debug tracing is enabled.
        /// </summary>
        /// <value>
        /// The is debug enabled.
        /// </value>
        public static bool IsDebugEnabled(this Tracer tracer)
        {
            return tracer.IsTraceEnabled(TraceLevel.Debug);
        }

        /// <summary>
        /// Gets a value indicating whether is error enabled.
        /// </summary>
        /// <value>
        /// The is error enabled.
        /// </value>
        public static bool IsErrorEnabled(this Tracer tracer)
        {
            return tracer.IsTraceEnabled(TraceLevel.Error);
        }

        /// <summary>
        /// Gets a value indicating whether is info enabled.
        /// </summary>
        /// <value>
        /// The is info enabled.
        /// </value>
        public static bool IsInfoEnabled(this Tracer tracer)
        {
            return tracer.IsTraceEnabled(TraceLevel.Info);
        }

        public static bool IsSevereEnabled(this Tracer tracer)
        {
            return tracer.IsTraceEnabled(TraceLevel.Severe);
        }

        /// <summary>
        /// Gets a value indicating whether is verbose enabled.
        /// </summary>
        /// <value>
        /// The is verbose enabled.
        /// </value>
        public static bool IsVerboseEnabled(this Tracer tracer)
        {
            return tracer.IsTraceEnabled(TraceLevel.Verbose);
        }

        /// <summary>
        /// Gets a value indicating whether is warn enabled.
        /// </summary>
        /// <value>
        /// The is warn enabled.
        /// </value>
        public static bool IsWarnEnabled(this Tracer tracer)
        {
            return tracer.IsTraceEnabled(TraceLevel.Warn);
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Debug(this Tracer tracer, string message)
        {
            tracer.Trace(TraceLevel.Debug, null, message);
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Debug(this Tracer tracer, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Debug, null, message, arg0);
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Debug(this Tracer tracer, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Debug, null, message, arg0, arg1);
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Debug(this Tracer tracer, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Debug, null, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Debug(this Tracer tracer, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Debug, null, message, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Error(this Tracer tracer, string message)
        {
            tracer.Trace(TraceLevel.Error, null, message);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Error(this Tracer tracer, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Error, null, message, arg0);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Error(this Tracer tracer, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Error, null, message, arg0, arg1);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Error(this Tracer tracer, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Error, null, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Error(this Tracer tracer, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Error, null, message, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Error(this Tracer tracer, Exception exception, string message)
        {
            tracer.Trace(TraceLevel.Error, exception, message);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Error(this Tracer tracer, Exception exception, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Error, exception, message, arg0);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Error(this Tracer tracer, Exception exception, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Error, exception, message, arg0, arg1);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Error(this Tracer tracer, Exception exception, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Error, exception, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Error(this Tracer tracer, Exception exception, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Error, exception, message, args);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Info(this Tracer tracer, string message)
        {
            tracer.Trace(TraceLevel.Info, null, message);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Info(this Tracer tracer, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Info, null, message, arg0);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Info(this Tracer tracer, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Info, null, message, arg0, arg1);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Info(this Tracer tracer, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Info, null, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Info(this Tracer tracer, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Info, null, message, args);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Verbose(this Tracer tracer, string message)
        {
            tracer.Trace(TraceLevel.Verbose, null, message);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Verbose(this Tracer tracer, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Verbose, null, message, arg0);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Verbose(this Tracer tracer, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Verbose, null, message, arg0, arg1);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Verbose(this Tracer tracer, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Verbose, null, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Verbose(this Tracer tracer, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Verbose, null, message, args);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Verbose(this Tracer tracer, Exception exception, string message)
        {
            tracer.Trace(TraceLevel.Verbose, exception, message);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Verbose(this Tracer tracer, Exception exception, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Verbose, exception, message, arg0);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Verbose(this Tracer tracer, Exception exception, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Verbose, exception, message, arg0, arg1);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Verbose(this Tracer tracer, Exception exception, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Verbose, exception, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The verbose.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Verbose(this Tracer tracer, Exception exception, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Verbose, exception, message, args);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Warn(this Tracer tracer, string message)
        {
            tracer.Trace(TraceLevel.Warn, null, message);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Warn(this Tracer tracer, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Warn, null, message, arg0);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Warn(this Tracer tracer, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Warn, null, message, arg0, arg1);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Warn(this Tracer tracer, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Warn, null, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Warn(this Tracer tracer, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Warn, null, message, args);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Warn(this Tracer tracer, Exception exception, string message)
        {
            tracer.Trace(TraceLevel.Warn, exception, message);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Warn(this Tracer tracer, Exception exception, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Warn, exception, message, arg0);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Warn(this Tracer tracer, Exception exception, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Warn, exception, message, arg0, arg1);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Warn(this Tracer tracer, Exception exception, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Warn, exception, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Warn(this Tracer tracer, Exception exception, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Warn, exception, message, args);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Severe(this Tracer tracer, string message)
        {
            tracer.Trace(TraceLevel.Severe, null, message);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Severe(this Tracer tracer, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Severe, null, message, arg0);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Severe(this Tracer tracer, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Severe, null, message, arg0, arg1);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Severe(this Tracer tracer, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Severe, null, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Severe(this Tracer tracer, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Severe, null, message, args);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void Severe(this Tracer tracer, Exception exception, string message)
        {
            tracer.Trace(TraceLevel.Severe, exception, message);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public static void Severe(this Tracer tracer, Exception exception, string message, object arg0)
        {
            tracer.Trace(TraceLevel.Severe, exception, message, arg0);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public static void Severe(this Tracer tracer, Exception exception, string message, object arg0, object arg1)
        {
            tracer.Trace(TraceLevel.Severe, exception, message, arg0, arg1);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public static void Severe(this Tracer tracer, Exception exception, string message, object arg0, object arg1, object arg2)
        {
            tracer.Trace(TraceLevel.Severe, exception, message, arg0, arg1, arg2);
        }

        /// <summary>
        /// The Severe.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Severe(this Tracer tracer, Exception exception, string message, params object[] args)
        {
            tracer.Trace(TraceLevel.Severe, exception, message, args);
        }

    }
}
