using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebNET
{
    internal static class Utility
    {
        private const string EOL = "\r\n";
        private const string GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        private static readonly Regex getRegex = new("^GET");
        private static readonly Regex keyRegex = new("Sec-WebSocket-Key: (.*)");

        /// <summary>
        ///     Attempts to handshake the client
        /// </summary>
        /// <param name="tcp">The TCP client</param>
        /// <returns>Bool</returns>
        internal static async Task<bool> TryHandshakeAsync(TcpClient tcp)
        {
            Socket socket = tcp.Client;
            NetworkStream stream = tcp.GetStream();

            byte[] buffer = new byte[556];
            await socket.ReceiveAsync(buffer, SocketFlags.None);
            string headers = Encoding.UTF8.GetString(buffer);

            bool isGet = getRegex.IsMatch(headers);
            if (isGet)
            {
                string key = keyRegex.Match(headers).Groups[1].Value.Trim() + GUID;
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                string sha1 = Convert.ToBase64String(SHA1.Create().ComputeHash(keyBytes));
                byte[] response = Encoding.UTF8.GetBytes(
                    $"HTTP/1.1 101 Switching Protocols{EOL}" +
                    $"Connection: Upgrade{EOL}" +
                    $"Upgrade: websocket{EOL}" +
                    $"Sec-WebSocket-Accept: {sha1}{EOL}{EOL}");
                await stream.WriteAsync(response.AsMemory(0, response.Length));
                await stream.FlushAsync();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Attempts to decode a byte array into a string
        /// </summary>
        /// <param name="bytes">The bytes representing the message</param>
        /// <param name="message">The message</param>
        /// <returns>Bool</returns>
        internal static bool TryDecodeMessage(byte[] bytes, out string message)
        {
            throw new NotImplementedException(nameof(TryDecodeMessage));
        }

        /// <summary>
        ///     Encodes a message
        /// </summary>
        /// <param name="message">The message to encode</param>
        internal static void EncodeMessage(string message)
        {
            throw new NotImplementedException(nameof(EncodeMessage));
        }
    }
}
