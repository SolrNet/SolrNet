using System;

namespace SolrNet.Commands.Cores {
    /// <summary>
    /// Atomically swaps the names used to access two existing cores. 
    /// This can be useful for replacing a "live" core with an "ondeck" core, and keeping the old "live" core running in case you decide to roll-back.
    /// </summary>
    public class SwapCommand : CoreCommand {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwapCommand"/> class.
        /// </summary>
        /// <param name="coreName">Name of the core to swap.</param>
        /// <param name="otherName">Name of the other core to swap with.</param>
        public SwapCommand(string coreName, string otherName) {
            if (string.IsNullOrEmpty(coreName))
                throw new ArgumentException("Core Name must be specified.", "coreName");
            if (string.IsNullOrEmpty(otherName))
                throw new ArgumentException("Other Core Name must be specified.", "otherName");

            AddParameter("action", "SWAP");
            AddParameter("core", coreName);
            AddParameter("other", otherName);
        }
    }
}