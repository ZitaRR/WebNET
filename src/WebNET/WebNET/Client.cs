using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebNET
{
    /// <summary>
    ///     The client, handles the connection and its' operations
    /// </summary>
    internal class Client
    {
        private static int entities = 0;

        private readonly Server server;
        private readonly TcpClient tcp;
        private readonly NetworkStream stream;

        /// <summary>
        ///     Client ID
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     Wraps a TCP client and binds it to the server the TCP client originated from
        /// </summary>
        /// <param name="server">The server</param>
        /// <param name="tcp">The TCP client</param>
        internal Client(Server server, TcpClient tcp)
        {
            this.server = server;
            this.tcp = tcp;
            stream = this.tcp.GetStream();

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
                    throw new Exception("Handshake failed");
                }

                await ReadAsync();
            }
            catch (Exception e)
            {
                //Destroy client
                Console.WriteLine("Error: " + e.Message);
            }
        }

        /// <summary>
        ///     Reads data from the socket stream
        /// </summary>
        /// <returns>Task</returns>
        private async Task ReadAsync()
        {
            throw new NotImplementedException(nameof(ReadAsync));
        }
    }
}

