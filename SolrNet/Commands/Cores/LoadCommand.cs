using System;

namespace SolrNet.Commands.Cores {
    /// <summary>
    /// This will load a new core from an existing configuration (will be implemented when cores can be described with a lazy-load flag).
    /// </summary>
    /// <remarks>
    /// Not Implemented Yet
    /// </remarks>
    public class LoadCommand : CoreCommand {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadCommand"/> class.
        /// </summary>
        /// <param name="coreName">Name of the core.</param>
        public LoadCommand(string coreName) {
            if (string.IsNullOrEmpty(coreName))
                throw new ArgumentException("Core Name must be specified.", "coreName");

            AddParameter("action", "LOAD");
            AddParameter("core", coreName);
        }
    }
}