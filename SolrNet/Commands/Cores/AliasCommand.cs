using System;

namespace SolrNet.Commands.Cores {
    /// <summary>
    /// Adds an additional name for a core.
    /// </summary>
    public class AliasCommand : CoreCommand {
        /// <summary>
        /// Initializes a new instance of the <see cref="AliasCommand"/> class.
        /// </summary>
        /// <param name="coreName">Existing Name of the core.</param>
        /// <param name="aliasName">New alias to use for the same core.</param>
        public AliasCommand(string coreName, string aliasName) {
            if (string.IsNullOrEmpty(coreName))
                throw new ArgumentException("Core Name must be specified.", "coreName");
            if (string.IsNullOrEmpty(aliasName))
                throw new ArgumentException("The new Alias must be specified.", "otherName");

            AddParameter("action", "ALIAS");
            AddParameter("core", coreName);
            AddParameter("other", aliasName);
        }
    }
}