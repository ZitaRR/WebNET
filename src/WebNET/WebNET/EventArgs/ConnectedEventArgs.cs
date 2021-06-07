namespace WebNET.EventArgs
{
    /// <summary>
    ///     Data about the client that connected
    /// </summary>
    public readonly struct ConnectedEventArgs
    {
        /// <summary>
        ///     ID of the client
        /// </summary>
        public int Id { get; }

        internal ConnectedEventArgs(int id)
        {
            Id = id;
        }
    }
}
