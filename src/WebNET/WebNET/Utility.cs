using System;
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
            int op = bytes[0] & 0b_0000_1111;
            int payload = bytes[1] & 0b_0111_1111;
            int length = 0;
            int startIndex = 2;

            if (op == 8)
            {
                message = "Connection closed";
                return false;
            }

            switch (payload)
            {
                case 126:
                    length = 2;
                    break;
                case 127:
                    length = 8;
                    break;
            }

            startIndex += length;

            byte[] mask = bytes.Skip(startIndex).Take(4).ToArray();
            byte[] encoded = bytes.Skip(startIndex + 4).ToArray();
            byte[] decoded = new byte[encoded.Length];

            for (int i = 0; i < encoded.Length; i++)
            {
                decoded[i] = (byte)(encoded[i] ^ mask[i % 4]);
            }

            message = Encoding.UTF8.GetString(decoded);
            return true;
        }

        /// <summary>
        ///     Encodes a message
        /// </summary>
        /// <param name="message">The message to encode</param>
        internal static byte[] EncodeMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            if (messageBytes.Length > 125)
                throw new ArgumentException("Message is too big. Max size: 125");

            byte[] bytes = new byte[messageBytes.Length + 2];
            bytes[0] = 0b_1000_0001;
            bytes[1] = (byte)messageBytes.Length;

            messageBytes.CopyTo(bytes, 2);
            return bytes;
        }

        /// <summary>
        ///     Builds the close frame
        /// </summary>
        /// <returns>Byte array</returns>
        internal static byte[] BuildCloseFrame()
        {
            return new byte[] { 0b_1000_1000, 0b_0000_0000 };
        }
    }
}
