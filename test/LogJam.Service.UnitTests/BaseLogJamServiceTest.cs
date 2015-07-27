// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseLogJamServiceTest.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Service.UnitTests
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Threading;

    using global::Owin;

    using LogJam.Owin.Http;
    using LogJam.Trace;
    using LogJam.Trace.Switches;
    using LogJam.XUnit2;

    using Microsoft.Owin;
    using Microsoft.Owin.Testing;

    using Owin;

    using Xunit.Abstractions;


    /// <summary>
    /// Shared logic for testing LogJam.Service
    /// </summary>
    public abstract class BaseLogJamServiceTest : IDisposable
    {

#if DEBUG
        protected const int DefaultTestTimeoutMs = 10 * 60 * 1000;
#else
        protected const int DefaultTestTimeoutMs = 60 * 1000;
#endif

        protected readonly CancellationTokenSource cancellationTokenSource;

		private readonly ThresholdTraceSwitch _unitTestTraceSwitch;
		private readonly TraceManager _traceManager;


        protected BaseLogJamServiceTest(ITestOutputHelper testOutput)
        {
            cancellationTokenSource = new CancellationTokenSource();

            _unitTestTraceSwitch = new ThresholdTraceSwitch(TraceLevel.Debug);
            var logWriterConfig = new TestOutputLogWriterConfig(testOutput)
                .UseTestTraceFormat()
                .Format(new HttpRequestFormatter())
                .Format(new HttpResponseFormatter());
			_traceManager = new TraceManager(logWriterConfig, _unitTestTraceSwitch);
        }

        public void Dispose()
        {
            _traceManager.Dispose();
        }

        protected void CancelTestAfter(TimeSpan elapsed)
        {
            cancellationTokenSource.CancelAfter(elapsed);            
        }

        protected CancellationToken TestCancellationToken
        {
            get { return cancellationTokenSource.Token; }
        }

        public TraceLevel TraceThreshold
        {
            get { return _unitTestTraceSwitch.Threshold; }
            set { _unitTestTraceSwitch.Threshold = value; }
        }

        protected TraceManager TestTraceManager
        {
            get { return _traceManager; }
        }

        protected Tracer Tracer
        {
            get { return _traceManager.TracerFor(this); }
        }

        
        public TestServer CreateTestServer()
        {
            string appName = GetType().FullName;

            return TestServer.Create(appBuilder =>
            {
                appBuilder.Properties["host.AppName"] = appName;

                appBuilder.RegisterLogManager(_traceManager);
                new OwinStartup().Configuration(appBuilder);
            });
        }

        public LogJamService CreateTestService(short port)
        {
            var service = new LogJamService(_traceManager)
                          {
                              ListenPort = port
                          };
            service.OwinConfigurationFunc = appBuilder =>
                                            {
                                                appBuilder.RegisterLogManager(_traceManager);
                                                new OwinStartup().Configuration(appBuilder);
                                            };

            service.Start(new string[] { });
            return service;
        }

    }

}