using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using WebSocketSharp;

namespace MSC_Remote_Control_Mod
{
    public class WebSocketMessage
    {
        public string Type { get; set; }

        public string Message { get; set; }
    }

    public static class WebSocketClient
    {
        private static WebSocket _webSocket;
        private static readonly ManualResetEvent ResetEvent = new ManualResetEvent(false);
        private static readonly List<string> MessageBuffer = new List<string>();

        public delegate void OnOpenedDelegate(object sender);
        public static event OnOpenedDelegate OnOpened;

        public delegate void OnMessageReceivedDelegate(object sender, WebSocketMessage message);
        public static event OnMessageReceivedDelegate OnMessageReceived;

        public static void Setup(string serverUri)
        {
            _webSocket = new WebSocket(serverUri);
            _webSocket.OnOpen += WebSocket_Opened;
            _webSocket.OnError += WebSocket_Error;
            _webSocket.OnClose += WebSocket_Closed;
            _webSocket.OnMessage += WebSocket_MessageReceived;
            
            var thread = new Thread(WebSocketThread);
            thread.Start();
        }
        
        private static void WebSocketThread()
        {
            // Connect to the WebSocket server
            Connect();
            
            // Wait for the connection to be closed
            ResetEvent.WaitOne();

            // Reconnect in a loop when the connection is closed
            while (true)
            {
                // Add your reconnection logic here
                // For simplicity, let's just wait for a few seconds and then reconnect
                Console.WriteLine($"Reconnecting in 3 seconds...");
                Thread.Sleep(3000);

                // Reconnect to the WebSocket server
                _webSocket.Connect();

                // Reset the event for the next iteration
                ResetEvent.Reset();
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static void Connect()
        {
            if (_webSocket.ReadyState != WebSocketState.Open)
            {
                _webSocket.Connect();
            }
        }

        public static void Send(string message)
        {
            if (_webSocket.ReadyState == WebSocketState.Open)
            {
                _webSocket.Send(message);
            }
            else
            {
                MessageBuffer.Add(message);
            }
        }

        public static void Close()
        {
            if (_webSocket.ReadyState == WebSocketState.Open)
            {
                _webSocket.Close();
            }
        }

        private static void WebSocket_Opened(object sender, EventArgs e)
        {
            OnOpened?.Invoke(sender);

            Console.WriteLine("WebSocket opened.");
            foreach (var message in MessageBuffer)
            {
                Send(message);
            }

            MessageBuffer.Clear();
        }

        public static void Reconnect()
        {
            ResetEvent.Set();
            _webSocket.Close();
        }

        private static void WebSocket_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"WebSocket error: {e.Exception.Message}");
            Reconnect();
        }

        private static void WebSocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("WebSocket closed. Reconnecting in 3 seconds");
            Reconnect();
        }

        private static void WebSocket_MessageReceived(object sender, MessageEventArgs e)
        {
            var msg = JsonConvert.DeserializeObject<WebSocketMessage>(e.Data);
            OnMessageReceived?.Invoke(sender, msg);
        }
    }
}