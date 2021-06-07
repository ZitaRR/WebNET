namespace WebNET.EventArgs
{
    /// <summary>
    ///     Data about the client that disconnected
    /// </summary>
    public readonly struct DisconnectedEventArgs
    {
        /// <summary>
        ///     ID of the client
        /// </summary>
        public int ClientId { get; }

        /// <summary>
        ///     The reason for disconnecting
        /// </summary>
        public string Reason { get; }

        internal DisconnectedEventArgs(int id, string reason)
        {
            ClientId = id;
            Reason = reason;
        }
    }
}
