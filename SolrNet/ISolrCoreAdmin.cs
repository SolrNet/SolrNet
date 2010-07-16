namespace SolrNet
{
    ///<summary>
    /// Contract that defines the behaviour of managing Solr Cores (multiple indexes).
    ///</summary>
    public interface ISolrCoreAdmin
    {
        /// <summary>
        /// The ALIAS action establishes an additional name by which a SolrCore may be referenced. 
        /// Subsequent actions may use the SolrCore's original name or any of its aliases.
        /// </summary>
        /// <remarks>
        /// This action is still considered experimental.
        /// </remarks>
        /// <param name="coreName">The name or alias of an existing core.</param>
        /// <param name="otherName">The additional name by which this core should be known.</param>
        void Alias(string coreName, string otherName);

        /// <summary>
        /// The CREATE action creates a new core and registers it. If persistence is enabled
        /// (persistent=”true” on the <solr> element), the updated configuration for this new core will be
        /// saved in solr.xml. If a SolrCore with the given name already exists, it will continue to handle requests
        /// while the new core is initializing. When the new core is ready, it will take new requests and the old core
        /// will be unloaded.
        /// </summary>
        /// <param name="coreName">The name of the new core. Same as “name” on the <core> element.</param>
        /// <param name="instanceDir">The directory where files for this SolrCore should be stored. Same as “instanceDir” on the <core> element.</param>
        /// <param name="configFile">(Optional) Name of the config file (solrconfig.xml) relative to “instanceDir”.</param>
        /// <param name="schemaFile">(Optional) Name of the schema file (schema.xml) relative to “instanceDir”.</param>
        /// <param name="dataDir">(Optional) Name of the data directory relative to “instanceDir”.</param>
        void Create(string coreName, string instanceDir, string configFile, string schemaFile, string dataDir);
        
        /// <summary>
        /// The RELOAD action loads a new core from the configuration of an existing, registered SolrCore. 
        /// While the new core is initializing, the existing one will continue to handle requests. 
        /// When the new SolrCore is ready, it takes over and the old core is unloaded.
        /// This is useful when you've made changes to a SolrCore's configuration on disk, such as adding 
        /// new field definitions. Calling the RELOAD action lets you apply the new configuration without 
        /// having to restart the Web container.
        /// </summary>
        /// <param name="coreName">The name of the core to be reloaded.</param>
        void Reload(string coreName);

        /// <summary>
        /// The RENAME action hanges the name of a SolrCore.
        /// </summary>
        /// <param name="coreName">The name of the SolrCore to be renamed.</param>
        /// <param name="otherName">
        /// The new name for the SolrCore. If the persistent attribute of <solr> is 
        /// “true”, the new name will be written to solr.xml as the “name” attribute 
        /// of the <core> attribute.
        /// </param>
        void Rename(string coreName, string otherName);

        /// <summary>
        /// The STATUS action returns the status of all running SolrCores,
        /// </summary>
        void Status();
        
        /// <summary>
        /// The STATUS action returns the status of the named core.
        /// </summary>
        /// <param name="coreName">
        /// The name of a core, as listed in the “name” attribute of a <core> element in solr.xml.
        /// </param>
        void Status(string coreName);

        /// <summary>
        /// SWAP atomically swaps the names used to access two existing SolrCores. 
        /// This can be used to swap new content into production. The prior core 
        /// remains available and can be swapped back, if necessary. Each core will
        /// be known by the name of the other, after the swap.
        /// </summary>
        /// <param name="coreName">The name of one of the cores to be swapped.</param>
        /// <param name="otherName">The name of one of the cores to be swapped.</param>
        void Swap(string coreName, string otherName);

        /// <summary>
        /// The UNLOAD action removes a core from LucidWorks for Solr. Active requests will 
        /// continue to be processed, but no new requests will be sent to the named core. 
        /// If a core is registered under more than one name, only the given name is removed.
        /// </summary>
        /// <param name="coreName">
        /// The name of the core to be to be removed. If the persistent
        /// attribute of <solr> is set to “true”, the <core> element
        /// with this “name” attribute will be removed from solr.xml.
        /// </param>
        void Unload(string coreName);
    }
}