// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Service
{
    using System;
    using System.Diagnostics;

    using LogJam.Trace;

    using Microsoft.Owin.Hosting;


    /// <summary>
    /// Command-line app entry point for the LogJam service.
    /// </summary>
    public sealed class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var service = new LogJamService();
                service.Start(args);

                Console.WriteLine("LogJam.Service started; <enter> to exit.");
                Console.ReadLine();

                service.Stop();
                return 0;
            }
            catch (Exception appException)
            {
                var tracer = TraceManager.Instance.TracerFor<Program>();
                tracer.Severe(appException, "LogJam.Service terminating due to unhandled Exception");
                return -1;
            }
        }

    }

}