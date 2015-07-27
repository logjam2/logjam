// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebSocketLogTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Service.UnitTests
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using LogJam.Trace;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// 
    /// </summary>
    public sealed class WebSocketLogTests : BaseLogJamServiceTest
    {

        private const string c_wsLogUriFormat = "ws://localhost:{0}/ws/log";
        private const short c_servicePort = 15095;

        public WebSocketLogTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {}

        [Fact]
        public async Task SimpleWebSocketConnection()
        {
            using (var server = CreateTestService(c_servicePort))
            using (var clientSocket = new ClientWebSocket())
            {
                //clientSocket.Options.SetRequestHeader("Accept", "application/json");
                //clientSocket.Options.SetRequestHeader("Content-Type", "application/json");
                clientSocket.Options.SetBuffer(4096, 4096 * 16);

                var wsUri = new Uri(string.Format(c_wsLogUriFormat, c_servicePort));
                await clientSocket.ConnectAsync(wsUri, TestCancellationToken);

                byte[] buffer = new byte[4096 * 16];

                string json = "{ 'message': 'Hello World' }";
                int cb = Encoding.UTF8.GetBytes(json, 0, json.Length, buffer, 0);
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(buffer, 0, cb);

                await clientSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, TestCancellationToken);

                ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(buffer, 4096 * 12, 4096 * 4);
                WebSocketReceiveResult webSocketReceiveResult = await clientSocket.ReceiveAsync(receiveBuffer, TestCancellationToken);

                if (webSocketReceiveResult.EndOfMessage && webSocketReceiveResult.MessageType == WebSocketMessageType.Text)
                {
                    string reply = Encoding.UTF8.GetString(receiveBuffer.Array, receiveBuffer.Offset, webSocketReceiveResult.Count);
                    Tracer.Info("Received web socket reply: {0}", reply);
                    Assert.InRange(reply.Length, 15, 40);
                }
                else
                {
                    throw new ApplicationException("Expected complete text response, not " + webSocketReceiveResult.MessageType);
                }

                await clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test client closing", TestCancellationToken);
            }
        }

    }

}