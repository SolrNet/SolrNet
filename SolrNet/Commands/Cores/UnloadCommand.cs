using System;

namespace SolrNet.Commands.Cores {
    /// <summary>
    /// Removes a core from Solr. Existing requests will continue to be processed, but no new requests can be sent to this core by the name. 
    /// If a core is registered under more than one name, only that specific mapping is removed.
    /// </summary>
    public class UnloadCommand : CoreCommand {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadCommand"/> class.
        /// </summary>
        /// <param name="coreName">Name of the core.</param>
        public UnloadCommand(string coreName) {
            if (string.IsNullOrEmpty(coreName))
                throw new ArgumentException("Core Name must be specified.", "coreName");

            AddParameter("action", "UNLOAD");
            AddParameter("core", coreName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadCommand"/> class.
        /// </summary>
        /// <remarks>
        /// Only available in Solr 3.3 and above.
        /// </remarks>
        /// <param name="coreName">Name of the core.</param>
        /// <param name="deleteIndex">If set to <c>true</c> deletes the index once the core is unloaded.  (Only available in 3.3 and above).</param>
        public UnloadCommand(string coreName, bool deleteIndex) {
            if (string.IsNullOrEmpty(coreName))
                throw new ArgumentException("Core Name must be specified.", "coreName");

            AddParameter("action", "UNLOAD");
            AddParameter("core", coreName);
            AddParameter("deleteIndex", deleteIndex.ToString().ToLower());
        }
    }
}