using System;

namespace SolrNet.Commands.Cores {
    /// <summary>
    /// Change the names used to access a core.
    /// </summary>
    public class RenameCommand : CoreCommand {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenameCommand"/> class.
        /// </summary>
        /// <param name="coreName">Name of the core to rename.</param>
        /// <param name="newName">The new name to use.</param>
        public RenameCommand(string coreName, string newName) {
            if (string.IsNullOrEmpty(coreName))
                throw new ArgumentException("Core Name must be specified.", "coreName");
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException("The new Core Name must be specified.", "newName");

            AddParameter("action", "RENAME");
            AddParameter("core", coreName);
            AddParameter("other", newName);
        }
    }
}