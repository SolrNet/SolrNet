using System;

namespace SolrNet.Commands.Cores {
    /// <summary>
    /// Load a new core from the same configuration as an existing registered core. While the "new" core is initializing, the "old" one will continue to accept requests. 
    /// Once it has finished, all new request will go to the "new" core, and the "old" core will be unloaded.
    /// </summary>
    /// <remarks>
    /// This can be useful when (backwards compatible) changes have been made to your solrconfig.xml or schema.xml files 
    /// (e.g. new &lt;field&gt; declarations, changed default params for a &gt;requestHandler&lt;, etc...) and you want to start using 
    /// them without stopping and restarting your whole Servlet Container.
    /// </remarks>
    public class ReloadCommand : CoreCommand {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReloadCommand"/> class.
        /// </summary>
        /// <param name="coreName">Name of the core to reload.</param>
        public ReloadCommand(string coreName) {
            if (string.IsNullOrEmpty(coreName))
                throw new ArgumentException("Core Name must be specified.", "coreName");

            AddParameter("action", "RELOAD");
            AddParameter("core", coreName);
        }
    }
}
