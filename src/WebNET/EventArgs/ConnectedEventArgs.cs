namespace WebNET.EventArgs
{
    /// <summary>
    ///     Data about the client that connected
    /// </summary>
    public readonly struct ConnectedEventArgs
    {
        /// <summary>
        ///     The client that connected
        /// </summary>
        public ClientConnection Client { get; }

        internal ConnectedEventArgs(ClientConnection client)
        {
            Client = client;
        }
    }
}
