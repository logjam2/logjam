// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinStartup.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Service
{
    using System.Linq;
    using System.Web.Http;

    using global::Owin;
    using global::Owin.WebSocket.Extensions;

    using LogJam.Service.WebSocket;

    using Microsoft.Owin;


    /// <summary>
    /// OWIN configuration for the LogJam service.
    /// </summary>
    public sealed class OwinStartup
    {

        public void Configuration(IAppBuilder owinAppBuilder)
        {
            owinAppBuilder.LogHttpRequests(owinAppBuilder.GetLogManagerConfig().Writers);
            owinAppBuilder.UseOwinTracerLogging();

            owinAppBuilder.UseErrorPage();

            owinAppBuilder.UseWelcomePage(new PathString("/welcome"));

            owinAppBuilder.MapWebSocketRoute<LogWebSocketConnection>("/ws/log");
            owinAppBuilder.MapWebSocketRoute<QueryWebSocketConnection>("/ws/query");

            var webApiConfiguration = new HttpConfiguration();
            webApiConfiguration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            webApiConfiguration.MapHttpAttributeRoutes();
            owinAppBuilder.UseWebApi(webApiConfiguration);
        }

    }

}