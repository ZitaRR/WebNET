namespace WebNET.EventArgs
{
    /// <summary>
    ///     Data sent from a client
    /// </summary>
    public readonly struct ReceivedEventArgs
    {
        /// <summary>
        ///     The client that sent the message
        /// </summary>
        public Client Client { get; }

        /// <summary>
        ///     The message from the client
        /// </summary>
        public string Message { get; }

        internal ReceivedEventArgs(Client client, string message)
        {
            Client = client;
            Message = message;
        }
    }
}
