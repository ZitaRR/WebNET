using System;
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

        public event Func<ConnectedEventArgs, Task> OnClientConnected;
        public event Func<ReceivedEventArgs, Task> OnClientReceived;
        public event Func<DisconnectedEventArgs, Task> OnClientDisconnected;

        /// <summary>
        ///     Setup the connection details using the machine's IPv4 address
        /// </summary>
        /// <param name="port">The port number</param>
        public Server(int port)
        {
            ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1];
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
                    Client client = new(this, tcp)
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
