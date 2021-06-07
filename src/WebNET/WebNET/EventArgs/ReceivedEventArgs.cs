namespace WebNET.EventArgs
{
    /// <summary>
    ///     Data sent from a client
    /// </summary>
    public readonly struct ReceivedEventArgs
    {
        /// <summary>
        ///     The ID of the client that sent this message
        /// </summary>
        public int ClientId { get; }

        /// <summary>
        ///     The message from the client
        /// </summary>
        public string Message { get; }

        internal ReceivedEventArgs(int id, string message)
        {
            ClientId = id;
            Message = message;
        }
    }
}
