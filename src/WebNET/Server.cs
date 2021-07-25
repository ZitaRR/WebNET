﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WebNET.EventArgs;

namespace WebNET
{
    /// <summary>
    ///     The Server, handles all incoming requests
    /// </summary>
    public class Server
    {
        private readonly TcpListener listener;
        private readonly IPAddress ip;

        /// <summary>
        ///     Fires upon client connection
        /// </summary>
        public event Func<ConnectedEventArgs, Task> OnClientConnected;

        /// <summary>
        ///     Fires upon receiving data from client
        /// </summary>
        public event Func<ReceivedEventArgs, Task> OnClientReceived;

        /// <summary>
        ///     Fires upon client disconnection
        /// </summary>
        public event Func<DisconnectedEventArgs, Task> OnClientDisconnected;

        /// <summary>
        ///     Setup the server details using the provided information
        /// </summary>
        /// <param name="host">The IP address</param>
        /// <param name="port">The port number</param>
        public Server(string host, int port)
        {
            if (!IPAddress.TryParse(host, out ip))
                throw new ArgumentException("Invalid IP address");

            listener = new(ip, port);
        }

        /// <summary>
        ///     Starts the server asynchronously 
        /// </summary>
        /// <returns>Task</returns>
        public async Task StartAsync()
        {
            listener.Start();
            while (true)
            {
                TcpClient tcp = await listener.AcceptTcpClientAsync();
                _ = Task.Run(async () =>
                {
                    Client client = new(tcp)
                    {
                        OnConnected = OnClientConnected,
                        OnReceived = OnClientReceived,
                        OnDisconnected = OnClientDisconnected
                    };

                    await client.StartAsync();
                });
            }
        }
    }
}
