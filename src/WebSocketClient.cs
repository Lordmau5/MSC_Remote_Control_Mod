using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebSocketSharp;

namespace MSC_Remote_Control_Mod
{
    public class WebSocketMessage
    {
        public string Type { get; set; }

        public string Message { get; set; }
    }

    public class WebSocketClient
    {
        private readonly WebSocket webSocket;
        private readonly List<string> messageBuffer = new List<string>();

        public delegate void OnOpenedDelegate(object sender);
        public event OnOpenedDelegate OnOpened;

        public delegate void OnMessageReceivedDelegate(object sender, WebSocketMessage message);
        public event OnMessageReceivedDelegate OnMessageReceived;

        public WebSocketClient(string serverUri)
        {
            this.webSocket = new WebSocket(serverUri);
            this.webSocket.OnOpen += this.WebSocket_Opened;
            this.webSocket.OnError += this.WebSocket_Error;
            this.webSocket.OnClose += this.WebSocket_Closed;
            this.webSocket.OnMessage += this.WebSocket_MessageReceived;
        }

        public void Connect()
        {
            if (this.webSocket.ReadyState != WebSocketState.Open)
            {
                this.webSocket.Connect();
            }
        }

        public void Send(string message)
        {
            if (this.webSocket.ReadyState == WebSocketState.Open)
            {
                this.webSocket.Send(message);
            }
            else
            {
                this.messageBuffer.Add(message);
            }
        }

        public void Close()
        {
            if (this.webSocket.ReadyState == WebSocketState.Open)
            {
                this.webSocket.Close();
            }
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            OnOpened?.Invoke(this);

            Console.WriteLine("WebSocket opened.");
            foreach (var message in this.messageBuffer)
            {
                this.Send(message);
            }

            this.messageBuffer.Clear();
        }

        private void Reconnect()
        {
            System.Threading.Thread.Sleep(3000);
            webSocket.Connect();
        }

        private void WebSocket_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"WebSocket error: {e.Exception.Message} - Reconnecting in 3 seconds");
            Reconnect();
        }

        private void WebSocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("WebSocket closed. Reconnecting in 3 seconds");
            Reconnect();
        }

        private void WebSocket_MessageReceived(object sender, MessageEventArgs e)
        {
            var msg = JsonConvert.DeserializeObject<WebSocketMessage>(e.Data);
            OnMessageReceived?.Invoke(this, msg);
        }
    }
}