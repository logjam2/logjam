// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerConfigTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Trace.Config
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using LogJam.Config;
    using LogJam.Config.Json;
    using LogJam.Internal.UnitTests.Examples;
    using LogJam.Trace;
    using LogJam.Trace.Config;
    using LogJam.Trace.Format;
    using LogJam.Trace.Switches;
    using LogJam.Writer;

    using Json = Newtonsoft.Json;

    using Xunit;
    using Xunit.Abstractions;
    using LogJam.Shared.Internal;


    /// <summary>
    /// Exercises use cases for <see cref="TraceManager.Config" /> modification.
    /// </summary>
    public sealed class TraceManagerConfigTests
    {

        private readonly ITestOutputHelper _testOutputHelper;

        public TraceManagerConfigTests(ITestOutputHelper testOutputHelper)
        {
            Arg.NotNull(testOutputHelper, nameof(testOutputHelper));

            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(ConfigForm.ObjectGraph)]
        [InlineData(ConfigForm.Fluent)]
        public void TraceToString(ConfigForm configForm)
        {
            var stringWriter = new StringWriter();

            TraceManager traceManager;
            if (configForm == ConfigForm.ObjectGraph)
            {
                var config = new TextWriterLogWriterConfig(stringWriter).Format(new DefaultTraceFormatter()
                                                                                {
                                                                                    IncludeDate = true,
                                                                                    IncludeTimestamp = true
                                                                                });
                traceManager = new TraceManager(config);
            }
            else if (configForm == ConfigForm.Fluent)
            {
                traceManager = new TraceManager();
                traceManager.Config.TraceTo(stringWriter,
                                            traceFormatter: new DefaultTraceFormatter()
                                                            {
                                                                IncludeDate = true,
                                                                IncludeTimestamp = true
                                                            });
            }
            else
            {
                throw new NotImplementedException();
            }

            using (traceManager)
            {
                var tracer = traceManager.TracerFor(this);
                Assert.True(traceManager.IsStarted);
                Assert.True(traceManager.LogManager.IsStarted);

                Assert.True(tracer.IsInfoEnabled());
                Assert.False(tracer.IsVerboseEnabled());

                Assert.False(traceManager.IsStopped);
                Assert.False(traceManager.LogManager.IsStopped);
                Assert.True(traceManager.IsHealthy);

                //Assert.Single(tracer.Writers);
                //Assert.IsType<DebuggerLogWriter>(tracer.Writers[0].innerEntryWriter);
                tracer.Info("Info message");
                tracer.Debug("Debug message not written");
            }

            Assert.Matches(@"Info\s+LJ\.UT\.T\.Config\.TraceManagerConfigTests\s+Info message\r\n", stringWriter.ToString());

            _testOutputHelper.WriteEntries(traceManager.SetupLog);

            Assert.False(traceManager.IsStarted);
            Assert.False(traceManager.LogManager.IsStarted);
            Assert.True(traceManager.IsStopped);
            Assert.True(traceManager.LogManager.IsStopped);
            Assert.True(traceManager.IsHealthy);
        }

        /// <summary>
        /// Ensures that everything works as expected when no TraceWriterConfig elements are configured.
        /// </summary>
        [Fact]
        public void NoTraceWritersConfiguredWorks()
        {
            var traceManagerConfig = new TraceManagerConfig();
            Assert.Empty(traceManagerConfig.Writers);
            using (var traceManager = new TraceManager(traceManagerConfig))
            {
                traceManager.Start();
                var tracer = traceManager.TracerFor(this);

                Assert.False(tracer.IsInfoEnabled());

                tracer.Info("Info");
            }
        }

        //[Fact]
        //public void RootThresholdCanBeSetOnInitialization()
        //{
        //	var listTraceLog = new ListLogWriter<TraceEntry>();
        //	// Trace output has threshold == Error
        //	using (var traceManager = new TraceManager(listTraceLog, new ThresholdTraceSwitch(TraceLevel.Error)))
        //	{
        //		var tracer = traceManager.TracerFor(this);
        //		Assert.False(tracer.IsInfoEnabled());
        //		Assert.False(tracer.IsWarnEnabled());
        //		Assert.True(tracer.IsErrorEnabled());
        //		Assert.True(tracer.IsSevereEnabled());
        //	}
        //}

        [Fact]
        public void RootThresholdCanBeModifiedAfterTracing()
        {
            var setupTracerFactory = new SetupLog();
            var listLogWriter = new ListLogWriter<TraceEntry>(setupTracerFactory);

            // Start with threshold == Info
            var traceSwitch = new ThresholdTraceSwitch(TraceLevel.Info);
            using (var traceManager = new TraceManager(listLogWriter, traceSwitch))
            {
                traceManager.Start();
                var tracer = traceManager.TracerFor(this);

                // Log stuff
                tracer.Info("Info"); // Should log
                tracer.Verbose("Verbose"); // Shouldn't log
                Assert.Single(listLogWriter);

                // Change threshold
                traceSwitch.Threshold = TraceLevel.Verbose;

                // Log
                tracer.Info("Info");
                tracer.Verbose("Verbose"); // Should log
                tracer.Debug("Debug"); // Shouldn't log
                Assert.Equal(3, listLogWriter.Count());
            }
        }

        //[Fact]
        //public void RootLogWriterCanBeReplacedAfterTracing()
        //{
        //	// First log entry is written here
        //	var initialList = new ListLogWriter<TraceEntry>();
        //	var secondList = new ListLogWriter<TraceEntry>();

        //	var rootTracerConfig = new TracerConfig(Tracer.RootTracerName, new ThresholdTraceSwitch(TraceLevel.Info), initialList);
        //	using (var traceManager = new TraceManager(rootTracerConfig))
        //	{
        //		var tracer = traceManager.TracerFor(this);

        //		tracer.Info("Info");
        //		tracer.Verbose("Verbose"); // Shouldn't log
        //		Assert.Equal(1, initialList.Count);

        //		// Change LogWriter
        //		rootTracerConfig.Replace(null, secondList);

        //		// Log
        //		tracer.Info("Info");
        //		tracer.Verbose("Verbose"); // Shouldn't log
        //		Assert.Equal(1, secondList.Count);
        //	}			
        //}

        //[Fact]
        //public void RootLogWriterCanBeAddedAfterTracing()
        //{
        //	var initialList = new ListLogWriter<TraceEntry>();
        //	var secondList = new ListLogWriter<TraceEntry>();

        //	var rootTracerConfig = new TracerConfig(Tracer.RootTracerName, new ThresholdTraceSwitch(TraceLevel.Info), initialList);
        //	using (var traceManager = new TraceManager(rootTracerConfig))
        //	{
        //		var tracer = traceManager.TracerFor(this);

        //		tracer.Info("Info");
        //		tracer.Verbose("Verbose"); // Shouldn't log
        //		Assert.Equal(1, initialList.Count);

        //		// Add a LogWriter
        //		rootTracerConfig.Add(new OnOffTraceSwitch(true), secondList);

        //		// Log
        //		tracer.Info("Info");
        //		tracer.Verbose("Verbose"); // Shouldn't log to first
        //		Assert.Equal(2, initialList.Count);
        //		Assert.Equal(2, secondList.Count);
        //		Assert.DoesNotContain("Verbose", initialList.Select(entry => entry.Message));
        //		Assert.Contains("Verbose", secondList.Select(entry => entry.Message));
        //	}			
        //}

        [Fact(Skip = "Not yet implemented")]
        public void RootLogWriterCanBeAddedThenRemoved()
        {}

        [Fact(Skip = "Not yet implemented")]
        public void NonRootThresholdCanBeModified()
        {}

        [Fact(Skip = "Not yet implemented")]
        public void NonRootLogWriterCanBeReplaced()
        {}

        [Fact(Skip = "Not yet implemented")]
        public void NonRootLogWriterCanBeAdded()
        {}

        [Fact(Skip = "Not yet implemented")]
        public void NonRootLogWriterCanBeAddedThenRemoved()
        {}

        [Fact(Skip = "Not yet implemented")]
        public void CanReadTraceManagerConfigFromFile()
        {}

        [Fact]
        public void MultipleTraceLogWritersForSameNamePrefixWithDifferentSwitchThresholds()
        {
            var setupTracerFactory = new SetupLog();
            var allListLogWriter = new ListLogWriter<TraceEntry>(setupTracerFactory);
            var errorListLogWriter = new ListLogWriter<TraceEntry>(setupTracerFactory);

            var traceWriterConfigAll = new TraceWriterConfig(allListLogWriter)
                                       {
                                           Switches =
                                           {
                                               { "LogJam.UnitTests", new OnOffTraceSwitch(true) }
                                           }
                                       };
            var traceWriterConfigErrors = new TraceWriterConfig(errorListLogWriter)
                                          {
                                              Switches =
                                              {
                                                  { "LogJam.UnitTests", new ThresholdTraceSwitch(TraceLevel.Error) }
                                              }
                                          };
            using (var traceManager = new TraceManager(new TraceManagerConfig(traceWriterConfigAll, traceWriterConfigErrors), setupTracerFactory))
            {
                var tracer = traceManager.TracerFor(this);
                var fooTracer = traceManager.GetTracer("foo");

                tracer.Info("Info");
                tracer.Verbose("Verbose");
                tracer.Error("Error");
                tracer.Severe("Severe");

                // fooTracer shouldn't log to either of these lists
                fooTracer.Severe("foo Severe");

                Assert.Equal(2, errorListLogWriter.Count);
                Assert.Equal(4, allListLogWriter.Count);
            }
        }

        public static IEnumerable<object[]> TestTraceManagerConfigs
        {
            get
            {
                // test TraceManagerConfig #1
                var config = new TraceManagerConfig(
                    new TraceWriterConfig()
                    {
                        LogWriterConfig = new ListLogWriterConfig<TraceEntry>(),
                        Switches =
                        {
                            { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Info) },
                            { "Microsoft.WebApi.", new ThresholdTraceSwitch(TraceLevel.Warn) }
                        }
                    },
                    new TraceWriterConfig()
                    {
                        LogWriterConfig = new DebuggerLogWriterConfig(),
                        Switches =
                        {
                            { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Info) },
                        }
                    });
                yield return new object[] { config };
            }
        }

        [Theory(Skip = "Not yet implemented")]
        [MemberData(nameof(TestTraceManagerConfigs))]
        public void CanRoundTripTraceManagerConfigToJson(TraceManagerConfig traceManagerConfig)
        {
            var jsonSettings = new Json.JsonSerializerSettings();
            jsonSettings.ContractResolver = new JsonConfigContractResolver(jsonSettings.ContractResolver);
            string json = Json.JsonConvert.SerializeObject(traceManagerConfig, Json.Formatting.Indented, jsonSettings);

            _testOutputHelper.WriteLine(json);

            // TODO: Deserialize back to TraceManagerConfig, then validate that the config is equal.
        }

        [Theory(Skip = "Not yet implemented")]
        [MemberData(nameof(TestTraceManagerConfigs))]
        public void CanRoundTripTraceManagerConfigToXml(TraceManagerConfig traceManagerConfig)
        {
            // Serialize to xml
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TraceManagerConfig));
            var sw = new StringWriter();
            xmlSerializer.Serialize(sw, traceManagerConfig);

            string xml = sw.ToString();
            _testOutputHelper.WriteLine(xml);

            // Deserialize back to TraceManagerConfig
            TraceManagerConfig deserializedConfig = (TraceManagerConfig) xmlSerializer.Deserialize(new StringReader(xml));

            Assert.Equal(traceManagerConfig, deserializedConfig);
        }

        [Fact]
        public void ConfigCanBeChangedAfterStarting()
        {
            var setupTracerFactory = new SetupLog();
            var logWriter = new ListLogWriter<TraceEntry>(setupTracerFactory);

            var config = new TraceManagerConfig();
            // TODO: Rename to tracerConfig?
            var traceWriterConfig = config.UseLogWriter(logWriter, "A"); // Only logs for tracer A

            TraceManager traceManager;
            using (traceManager = new TraceManager(config, setupTracerFactory))
            {
                var tracerA = traceManager.GetTracer("A");
                var tracerB = traceManager.GetTracer("B");

                Assert.True(traceManager.IsStarted);
                Assert.True(traceManager.LogManager.IsStarted);

                Assert.True(tracerA.IsInfoEnabled());
                Assert.False(tracerB.IsInfoEnabled());

                // Option 1: Change the config
                traceWriterConfig.Switches.Add("B", new OnOffTraceSwitch(true));
                traceManager.Start(); // Explicit restart required

                Assert.True(tracerB.IsInfoEnabled());
                Assert.True(tracerB.IsDebugEnabled());

                // For option 2, see the next test: AddNewTracerConfigToSameWriterAfterStarting
            }
        }

        [Fact]
        public void AddNewTracerConfigToSameWriterAfterStarting()
        {
            var setupTracerFactory = new SetupLog();
            var logWriter = new ListLogWriter<TraceEntry>(setupTracerFactory);

            var config = new TraceManagerConfig();
            config.UseLogWriter(logWriter, "A"); // Only logs for tracer A

            TraceManager traceManager;
            using (traceManager = new TraceManager(config, setupTracerFactory))
            {
                var tracerA = traceManager.GetTracer("A");
                var tracerB = traceManager.GetTracer("B");
                Assert.True(traceManager.IsStarted);

                Assert.True(tracerA.IsInfoEnabled());
                Assert.False(tracerB.IsInfoEnabled());

                // For option 1, see the previous test: ConfigCanBeChangedAfterStarting

                // Option 2: Add a new TraceWriterConfig, and restart, to enable B
                config.UseLogWriter(logWriter, "B", new OnOffTraceSwitch(true));
                Assert.False(tracerB.IsInfoEnabled()); // before the restart, tracerB is disabled
                traceManager.Start();

                Assert.True(tracerB.IsInfoEnabled());
                Assert.True(tracerB.IsDebugEnabled());
            }
        }

        [Fact]
        public void AddingTraceWriterConfigUpdatesLogManagerConfig()
        {
            var textWriterLogWriterConfig = new TextWriterLogWriterConfig(new StringWriter());

            var logWriterConfigs = new ILogWriterConfig[]
                                   {
                                       new ListLogWriterConfig<TraceEntry>(),
                                       textWriterLogWriterConfig
                                   };

            using (var traceManager = new TraceManager())
            {
                foreach (var logWriterConfig in logWriterConfigs)
                {
                    traceManager.Config.Writers.Add(new TraceWriterConfig(logWriterConfig, TraceManagerConfig.CreateDefaultSwitchSet()));

                    Assert.Contains(traceManager.LogManager.Config.Writers, lwc => lwc == logWriterConfig);
                }
            }
        }

        [Fact]
        public void RemovingTraceWriterConfigDoesNotRemoveLogWriterConfigs()
        {
            var textWriterLogWriterConfig = new TextWriterLogWriterConfig(new StringWriter());

            var logWriterConfigs = new ILogWriterConfig[]
                                   {
                                       new ListLogWriterConfig<TraceEntry>(),
                                       textWriterLogWriterConfig
                                   };
            var traceWriterConfigs = new List<TraceWriterConfig>();

            using (var traceManager = new TraceManager())
            {
                foreach (var logWriterConfig in logWriterConfigs)
                {
                    traceWriterConfigs.Add(new TraceWriterConfig(logWriterConfig, TraceManagerConfig.CreateDefaultSwitchSet()));
                }
                traceManager.Config.Writers.UnionWith(traceWriterConfigs);

                // Test removing each
                for (int i = 0; i < logWriterConfigs.Length; ++i)
                {
                    var logWriterConfig = logWriterConfigs[i];
                    var traceWriterConfig = traceWriterConfigs[i];

                    // Each logWriterConfig should exist in the LogManagerConfig before it is removed from the TraceManagerConfig
                    Assert.Contains(traceManager.LogManager.Config.Writers, lwc => lwc == logWriterConfig);

                    traceManager.Config.Writers.Remove(traceWriterConfig);
                    Assert.DoesNotContain(traceManager.Config.Writers, twc => twc == traceWriterConfig);

                    // LogWriters are left in place, because they may be used for other purposes
                    Assert.Contains(traceManager.LogManager.Config.Writers, lwc => lwc == logWriterConfig);
                }
            }
        }

        [Fact]
        public void RemovingTraceWriterConfigLeavesWritersFormattedForOtherEntryTypes()
        {
            var textWriterLogWriterConfig = new TextWriterLogWriterConfig(new StringWriter());
            textWriterLogWriterConfig.Format<MessageEntry>();

            using (var traceManager = new TraceManager())
            {
                // Add tracing to the textWriter
                var traceWriterConfig = traceManager.Config.TraceTo(textWriterLogWriterConfig);

                // The textWriterLogWriter should now include formatters for 2 entry types
                Assert.True(textWriterLogWriterConfig.HasFormatterFor<TraceEntry>());
                Assert.True(textWriterLogWriterConfig.HasFormatterFor<MessageEntry>());

                // Remove the LogWriter from LogManager.Config
                traceManager.LogManager.Config.Writers.Remove(textWriterLogWriterConfig);

                // Now it should not exist either in the TraceManagerConfig or the LogManagerConfig
                Assert.DoesNotContain(traceManager.LogManager.Config.Writers, lwc => lwc == textWriterLogWriterConfig);
                Assert.DoesNotContain(traceManager.Config.Writers, twc => twc == traceWriterConfig);

                // Add tracing to the textWriter (again)
                traceWriterConfig = traceManager.Config.TraceTo(textWriterLogWriterConfig);

                // Now it should exist in both the TraceManagerConfig and the LogManagerConfig
                Assert.Contains(traceManager.LogManager.Config.Writers, lwc => lwc == textWriterLogWriterConfig);
                Assert.Contains(traceManager.Config.Writers, twc => twc == traceWriterConfig);

                // Remove tracing to the textWriter
                Assert.True(traceManager.Config.Writers.Remove(traceWriterConfig));

                // Now it should exist only in the LogManagerConfig
                Assert.Contains(traceManager.LogManager.Config.Writers, lwc => lwc == textWriterLogWriterConfig);
                Assert.DoesNotContain(traceManager.Config.Writers, twc => twc == traceWriterConfig);
            }
        }

    }

}
