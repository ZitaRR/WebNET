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
        public Client Client { get; }

        internal ConnectedEventArgs(Client client)
        {
            Client = client;
        }
    }
}
