// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorConfig.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Service
{
    using System.Web.Http;

    using LogJam.Trace;

    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;


    /// <summary>
    /// Configures SimpleInjector for the LogJam service.
    /// </summary>
    internal sealed class InjectorConfig
    {

        public InjectorConfig()
        {
            Container = new Container();    
            Configure();
        }

        public Container Container { get; private set; }

        public void Configure()
        {
            Container.RegisterSingle<ITracerFactory>(TraceManager.Instance);
            Container.RegisterSingle(LogManager.Instance);
        }

        public void AddToWebApi(HttpConfiguration httpConfiguration)
        {
            Container.RegisterWebApiControllers(httpConfiguration);

            httpConfiguration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Container);
        }

    }

}