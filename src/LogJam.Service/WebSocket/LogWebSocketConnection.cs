// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWebSocketConnection.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Service.WebSocket
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading.Tasks;

    using global::Owin.WebSocket;

    using LogJam.Trace;

    using Microsoft.Owin;


    /// <summary>
    /// Received log data from callers over a websocket.
    /// </summary>
    public sealed class LogWebSocketConnection : WebSocketConnection
    {

        private Tracer _tracer;

        public override void OnOpen()
        {
            _tracer = Context.GetTracerFactory().TracerFor(this);

            _tracer.Info("WebSocket connection established for request # {0}", Context.GetRequestNumber());
        }

        public override void OnClose(WebSocketCloseStatus? closeStatus, string closeStatusDescription)
        {
            if (closeStatus == WebSocketCloseStatus.NormalClosure)
            {
                _tracer.Info("WebSocket connection closed normally for request # {0} : {1}", Context.GetRequestNumber(), closeStatusDescription);
            }
            else
            {
                _tracer.Warn("WebSocket connection closed abnormally (status: {0}) for request # {1} : {2}", closeStatus, Context.GetRequestNumber(), closeStatusDescription);
            }
        }

        public override async Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        {
            // Look at
            //base.Context.Request.Headers;
            //base.Arguments;

            if (type == WebSocketMessageType.Text)
            {
                string messageReceived = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
                _tracer.Verbose("Received: " + messageReceived);
                if (messageReceived.Contains("'Hello World'"))
                {
                    byte[] buffer = new byte[255];
                    string response = "{ 'message': 'Hey! What's up?' }";
                    int cb = Encoding.UTF8.GetBytes(response, 0, response.Length, buffer, 0);
                    var responseBytes = new ArraySegment<byte>(buffer, 0, cb);
                    await SendText(responseBytes, true);
                    return;
                }

                await Close(WebSocketCloseStatus.InvalidPayloadData, "Message not handled");
            }

            await Close(WebSocketCloseStatus.InvalidMessageType, "Unexpected message type");
        }

    }

}