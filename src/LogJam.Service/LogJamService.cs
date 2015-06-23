// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogJamService.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Service
{
    using System;
    using System.ServiceProcess;
    using System.Threading;

    using LogJam.Trace;

    using Microsoft.Owin.Hosting;


    /// <summary>
    /// Service controller for the LogJam service
    /// </summary>
    public sealed class LogJamService : ServiceBase
    {
        // Default port = logs = 1095
        public const short DefaultPort = 1095;

        private string _listenHostName = "localhost";
        private short _listenPort = DefaultPort;


        private readonly Tracer _tracer;
        private IDisposable _owinWebApp;

        public LogJamService()
        {
            _tracer = TraceManager.Instance.TracerFor(this);
            _owinWebApp = null;
        }

        public void Start(string[] args)
        {
            _tracer.Info("Starting LogJam Service...");
            ParseArgs(args);
            RunWebServer();
        }

        public new void Stop()
        {
            if (_owinWebApp != null)
            {
                _tracer.Info("Stopping LogJam Service...");
                _owinWebApp.Dispose();
                _owinWebApp = null;
                Thread.Sleep(1000);
            }
        }

        private void ParseArgs(string[] args)
        {
            if (args.Length > 0)
            {
                _tracer.Info("LogJam.Service command-line args: {0}", string.Join(" ", args));
            }
        }

        private void RunWebServer()
        {
            _owinWebApp = WebApp.Start<OwinStartup>(BuildListenUrl());
        }

        private string BuildListenUrl()
        {
            return "http://" + _listenHostName + ":" + _listenPort + "/";
        }

        #region ServiceBase overrides

        protected override void OnStart(string[] args)
        {
            Start(args);
        }

        protected override void OnStop()
        {
            Stop();
        }

        protected override void OnShutdown()
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }

}