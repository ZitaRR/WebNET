namespace WebNET.EventArgs
{
    /// <summary>
    ///     Data about the client that disconnected
    /// </summary>
    public readonly struct DisconnectedEventArgs
    {
        /// <summary>
        ///     The client that disconnected
        /// </summary>
        public ClientConnection Client { get; }

        /// <summary>
        ///     The reason for disconnecting
        /// </summary>
        public string Reason { get; }

        internal DisconnectedEventArgs(ClientConnection client, string reason)
        {
            if (string.IsNullOrEmpty(reason))
                reason = "No reason provided";

            Client = client;
            Reason = reason;
        }
    }
}
