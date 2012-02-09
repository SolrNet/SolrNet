namespace SolrNet.Commands.Cores {
    /// <summary>
    /// Get the status for a given core or all cores if no core is specified.
    /// </summary>
    public class StatusCommand : CoreCommand {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCommand"/> class.
        /// </summary>
        public StatusCommand() {
            AddParameter("action", "STATUS");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCommand"/> class.
        /// </summary>
        /// <param name="coreName">Name of the core to get status for.</param>
        public StatusCommand(string coreName) : this() {
            if (!string.IsNullOrEmpty(coreName))
                AddParameter("core", coreName);
        }
    }
}