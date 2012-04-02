using System;

namespace SolrNet.Commands.Cores {
    /// <summary>
    /// Creates a new core based on preexisting instanceDir/solrconfig.xml/schema.xml, and registers it. 
    /// If persistence is enabled (persist=true), the configuration for this new core will be saved in 'solr.xml'. 
    /// If a core with the same name exists, while the "new" created core is initalizing, the "old" one will continue to accept requests. 
    /// Once it has finished, all new request will go to the "new" core, and the "old" core will be unloaded.
    /// </summary>
    public class CreateCommand : CoreCommand {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommand"/> class.
        /// </summary>
        /// <param name="name">The name of the core.</param>
        /// <param name="instanceDir">The Solr instance directory.</param>
        public CreateCommand(string name, string instanceDir): this(name, instanceDir, null, null, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommand"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="instanceDir">The Solr instance directory.</param>
        /// <param name="configFile">The config file to use. (optional)</param>
        /// <param name="schemaFile">The schema file to use. (optional)</param>
        /// <param name="dataDir">The data directory. (optional)</param>
        public CreateCommand(string name, string instanceDir, string configFile, string schemaFile, string dataDir) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Core Name must be specified.", "name");
            if (string.IsNullOrEmpty(instanceDir))
                throw new ArgumentException("The Solr instance directory must be specified.", "instanceDir");

            AddParameter("action", "CREATE");
            AddParameter("name", name);
            AddParameter("instanceDir", instanceDir);
            if (configFile != null)
                AddParameter("config", configFile);
            if (schemaFile != null)
                AddParameter("schema", schemaFile);
            if (dataDir != null)
                AddParameter("dataDir", dataDir);
        }
    }
}