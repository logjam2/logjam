## Configuration API Examples

		var logConfig = new LogManagerConfig();
		var textConfig = logConfig.UseTextWriter(textWriter).Format(new TraceFormatter()).Format(new RequestFormatter()).Format(new ResponseFormatter());
			and/or
		var consoleConfig = logConfig.UseColoredConsole().Format(new TraceFormatter()).Format(new RequestFormatter()).Format(new ResponseFormatter());  
			and/or
		var rotatingConfig = logConfig.UseRotatingLogFile(dir, rotator).Format(new TraceFormatter()).Format(new RequestFormatter()).Format(new ResponseFormatter());
			and/or
		var remoteConfig = logConfig.UseRemoting(url).Encode<TraceEntry>().Encode<HttpRequestEntry>().Encode<HttpResponseEntry>();
		logConfig.ProxyWritingToBackgroundThread();

		var logManager = new LogManager(logConfig);

		var traceConfig = new TraceManagerConfig();
		traceConfig.With(textConfig).UseSwitches({ "prefix", new ThresholdTraceSwitch() }, { "prefix", new ThresholdTraceSwitch() });
    var traceManager = new TraceManager(traceConfig, logManager);