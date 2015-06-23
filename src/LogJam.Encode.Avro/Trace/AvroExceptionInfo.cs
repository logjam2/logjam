// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvroExceptionInfo.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Encode.Avro.Trace
{
    using System;
    using System.Runtime.Serialization;

    using LogJam.Schema;


    /// <summary>
    /// Provides support for serializing structured <see cref="Exception"/> data in <see cref="AvroTraceEntry"/>s.
    /// </summary>
    [SerializedTypeName("Exception", AvroTraceEntry.AvroSchemaNamespace)]
    public sealed class AvroExceptionInfo
    {

        /// <summary>
        /// The current wrapped <see cref="Exception"/>.  Can be replaced for subsequent serialization.
        /// </summary>
        private Exception _exception;

        /// <summary>
        /// An <see cref="AvroExceptionInfo"/> for inner exceptions.
        /// </summary>
        private AvroExceptionInfo _innerExceptionInfo;

        public void SetException(Exception exception)
        {
            _exception = exception;
        }

        [IgnoreDataMember]
        public bool IsEmpty { get { return _exception == null; } }

        [Required]
        public string ExceptionType { get { return IsEmpty ? null : _exception.GetType().FullName; } }

        public string Message { get { return IsEmpty ? null : _exception.Message; } }

        public string Source { get { return IsEmpty ? null : _exception.Source; } }

        public string StackTrace { get { return IsEmpty ? null : _exception.StackTrace; } }

        public AvroExceptionInfo InnerException
        {
            get
            {
                if ((_exception == null) || (_exception.InnerException == null))
                {
                    return null;
                }
                if (_innerExceptionInfo == null)
                {
                    _innerExceptionInfo = new AvroExceptionInfo();
                }
                _innerExceptionInfo.SetException(_exception.InnerException);
                return _innerExceptionInfo;
            }
        }

    }

}
