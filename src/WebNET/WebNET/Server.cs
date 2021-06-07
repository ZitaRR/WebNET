using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
        ///     Setup the connection details using the machine's IPv4 address
        /// </summary>
        /// <param name="port">The port number</param>
        public Server(int port)
        {
            ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1];
            listener = new TcpListener(ip, port);
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
                _ = Task.Run(async () => await new Client(this, tcp).StartAsync());
            }
        }
    }
}
