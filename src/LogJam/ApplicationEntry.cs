// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationEntry.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;


    /// <summary>
    /// Holds metadata about a running application; this is the top level context data, all other context data is scoped to the <c>ApplicationEntry</c>
    /// </summary>
    public sealed class ApplicationEntry : ITimestampedLogEntry
    {

        /// <summary>
        /// Uniquely identifies the application instance
        /// </summary>
        [DataMember]
        public Guid ApplicationInstanceId { get; set; }

        [DataMember]
        public readonly string Name;

        [DataMember]
        public readonly string Assembly;

        [DataMember]
        public readonly string Version;

        [DataMember]
        public readonly string MachineName;

        [DataMember]
        public readonly DateTime StartTimestampUtc;

        [DataMember]
        public readonly int ProcessId;

        [DataMember]
        public readonly string StartDirectory;

        public ApplicationEntry(string name, string assembly, string version, string machineName, DateTime startTimestampUtc, int processId, string startDirectory)
        {
            Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(name));   
            Contract.Requires<ArgumentException>(! string.IsNullOrWhiteSpace(machineName));   
            Contract.Requires<ArgumentException>(startTimestampUtc <= DateTime.UtcNow);

            ApplicationInstanceId = Guid.NewGuid();
            Name = name;
            Assembly = assembly;
            Version = version;
            MachineName = machineName;
            StartTimestampUtc = startTimestampUtc;
            ProcessId = processId;
            StartDirectory = startDirectory;
        }

        public ApplicationEntry(Assembly startupAssembly, AppDomain appDomain = null, Process applicationProcess = null)
        {
            Contract.Requires<ArgumentNullException>(startupAssembly != null);

            if (appDomain == null)
            {
                appDomain = AppDomain.CurrentDomain;
            }
            if (applicationProcess == null)
            {
                applicationProcess = Process.GetCurrentProcess();
            }

            ApplicationInstanceId = Guid.NewGuid();
            Name = appDomain.FriendlyName;
            Assembly = startupAssembly.GetName().Name;

            var informationalVersionAttribute = (AssemblyInformationalVersionAttribute) startupAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).FirstOrDefault();
            if (informationalVersionAttribute != null)
            {
                Version = informationalVersionAttribute.InformationalVersion;
            }
            else
            {
                Version = startupAssembly.GetName().Version.ToString();
            }

            MachineName = Environment.MachineName;
            StartTimestampUtc = applicationProcess.StartTime.ToUniversalTime();
            ProcessId = applicationProcess.Id;

            string startupDir = applicationProcess.StartInfo.WorkingDirectory;
            if (string.IsNullOrEmpty(startupDir))
            {
                startupDir = ".";
            }
            StartDirectory = new DirectoryInfo(startupDir).FullName;
        }

        DateTime ITimestampedLogEntry.TimestampUtc { get { return this.StartTimestampUtc; } }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} pid:{3}", Name, Version, MachineName, ProcessId);
        }

    }

}