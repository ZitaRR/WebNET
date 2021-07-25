using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WebNET.EventArgs;

namespace WebNET
{
    /// <summary>
    ///     The client, handles the connection and its' operations
    /// </summary>
    public sealed class ClientConnection
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
        internal ClientConnection(TcpClient tcp)
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
            catch (SocketException e)
            {
                await CloseAsync(e);
            }
        }

        /// <summary>
        ///     Closes the client connection
        /// </summary>
        /// <param name="reason">The reason for closing the connection</param>
        /// <returns>Task</returns>
        public async Task CloseAsync(string reason = null)
        {
            State = SocketState.Closing;
            await SendPayload(Utility.BuildCloseFrame());
            State = SocketState.Closed;
            await OnDisconnected?.Invoke(new DisconnectedEventArgs(this, reason));
        }

        /// <summary>
        ///     Closes the client connection with an exception
        /// </summary>
        /// <param name="e">The exception</param>
        /// <returns>Task</returns>
        internal async Task CloseAsync(Exception e)
        {
            State = SocketState.Closing;
            await SendPayload(Utility.BuildCloseFrame());
            State = SocketState.Closed;
            await OnDisconnected?.Invoke(new DisconnectedEventArgs(this, e.ToString()));
        }

        /// <summary>
        ///     Reads data from the socket stream
        /// </summary>
        /// <returns>Task</returns>
        private async Task ReadAsync()
        {
            while (State != (SocketState.Closed | SocketState.Closing))
            {
                if (!stream.DataAvailable)
                    continue;

                byte[] bytes = new byte[tcp.Available];
                await stream.ReadAsync(bytes.AsMemory(0, bytes.Length));
                if (bytes.Length <= 0)
                    continue;

                if (!Utility.TryDecodeMessage(bytes, out string message))
                {
                    if (message == "Connection closed")
                        await CloseAsync(message);
                    return;
                }

                await OnReceived?.Invoke(new ReceivedEventArgs(this, message));
            }
        } 
        
        /// <summary>
        ///     Writes an object to the socket stream
        /// </summary>
        /// <param name="data">The object to write</param>
        /// <returns>Task</returns>
        public async Task WriteAsync(object data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data);
                byte[] bytes = Utility.EncodeMessage(json);
                await stream.WriteAsync(bytes.AsMemory(0, bytes.Length));
                await stream.FlushAsync();
            }
            catch { throw; }
        }

        /// <summary>
        ///     Writes a string to the socket stream
        /// </summary>
        /// <param name="text">The string to write</param>
        /// <returns>Task</returns>
        public async Task WriteAsync(string text)
        {
            try
            {
                byte[] bytes = Utility.EncodeMessage(text);
                await stream.WriteAsync(bytes.AsMemory(0, bytes.Length));
                await stream.FlushAsync();
            }
            catch { throw; }
        }

        /// <summary>
        ///     Sends a payload to the client
        /// </summary>
        /// <param name="bytes">The payload</param>
        /// <returns>Task</returns>
        internal async Task SendPayload(byte[] bytes)
        {
            await stream.WriteAsync(bytes.AsMemory(0, bytes.Length));
            await stream.FlushAsync();
        }
    }
}

