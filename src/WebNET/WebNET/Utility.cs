using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebNET
{
    internal static class Utility
    {
        /// <summary>
        ///     Attempts to handshake the client
        /// </summary>
        /// <param name="tcp">The TCP client</param>
        /// <returns>Bool</returns>
        internal static async Task<bool> TryHandshakeAsync(TcpClient tcp)
        {
            throw new NotImplementedException(nameof(TryHandshakeAsync));
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
