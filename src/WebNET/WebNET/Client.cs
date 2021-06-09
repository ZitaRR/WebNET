﻿using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WebNET.EventArgs;

namespace WebNET
{
    /// <summary>
    ///     The client, handles the connection and its' operations
    /// </summary>
    public sealed class Client
    {
        private static int entities = 0;

        private readonly TcpClient tcp;
        private readonly NetworkStream stream;

        internal Func<ConnectedEventArgs, Task> OnConnected;
        internal Func<ReceivedEventArgs, Task> OnReceived;
        internal Func<DisconnectedEventArgs, Task> OnDisconnected;

        /// <summary>
        ///     Client ID
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     The state of the socket
        /// </summary>
        public SocketState State { get; private set; } = SocketState.Connecting;

        /// <summary>
        ///     The local endpoint
        /// </summary>
        public EndPoint Local { get; }

        /// <summary>
        ///     The remote endpoint
        /// </summary>
        public EndPoint Remote { get; }

        /// <summary>
        ///     Wraps a TCP client and binds it to the server
        /// </summary>
        /// <param name="tcp">The TCP client</param>
        internal Client(TcpClient tcp)
        {
            this.tcp = tcp;
            stream = this.tcp.GetStream();

            Local = this.tcp.Client.LocalEndPoint;
            Remote = this.tcp.Client.RemoteEndPoint;

            Id = ++entities;
        }

        /// <summary>
        ///     Starts the client for read and write operations
        /// </summary>
        /// <returns>Task</returns>
        internal async Task StartAsync()
        {
            try
            {
                if (!await Utility.TryHandshakeAsync(tcp))
                {
                    State = SocketState.Closing;
                    throw new SocketException(10053);
                }

                State = SocketState.Open;
                await OnConnected?.Invoke(new ConnectedEventArgs(this));
                await ReadAsync();
            }
            catch (Exception e)
            {
                State = SocketState.Closed;
                await OnDisconnected?.Invoke(new DisconnectedEventArgs(this, e.ToString()));
            }
        }

        /// <summary>
        ///     Reads data from the socket stream
        /// </summary>
        /// <returns>Task</returns>
        private async Task ReadAsync()
        {
            while (true)
            {
                if (!stream.DataAvailable)
                    continue;

                byte[] bytes = new byte[tcp.Available];
                await stream.ReadAsync(bytes.AsMemory(0, bytes.Length));
                if (bytes.Length <= 0)
                    continue;

                if (!Utility.TryDecodeMessage(bytes, out string message))
                    throw new Exception(message);

                await OnReceived?.Invoke(new ReceivedEventArgs(this, message));
            }
        } 
        
        /// <summary>
        ///     Writes to the socket stream
        /// </summary>
        /// <param name="data">The object to write</param>
        /// <returns>Task</returns>
        public async Task WriteAsync(object data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                byte[] bytes = Utility.EncodeMessage(json);
                await stream.WriteAsync(bytes.AsMemory(0, bytes.Length));
                await stream.FlushAsync();
            }
            catch (JsonSerializationException e)
            {
                throw new ArgumentException(e.Message, e);
            }
        }
    }
}

