﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SolrNet.Commands.Cores;

namespace SolrNet.Impl {
    /// <summary>
    /// Solr core administration commands.
    /// </summary>
    /// <seealso href="http://wiki.apache.org/solr/CoreAdmin"/>
    public class SolrCoreAdmin : LowLevelSolrServer, ISolrCoreAdmin {
        private readonly ISolrStatusResponseParser resultParser;

        private const string coreHandler = "/admin/cores";
        /// <summary>
        /// Initializes a new instance of the <see cref="SolrCoreAdmin"/> class.
        /// </summary>
        public SolrCoreAdmin(ISolrConnection connection, ISolrHeaderResponseParser headerParser, ISolrStatusResponseParser resultParser)
        :base (connection, headerParser) {
            this.resultParser = resultParser;
        }

        /// <summary>
        /// The ALIAS action establishes an additional name by which a core may be referenced.
        /// Subsequent actions may use the core's original name or any of its aliases.
        /// </summary>
        /// <remarks>
        /// This action is still considered experimental.
        /// </remarks>
        /// <param name="coreName">The name or alias of an existing core.</param>
        /// <param name="otherName">The additional name by which this core should be known.</param>
        public ResponseHeader Alias(string coreName, string otherName) {
            return SendAndParseHeader(new AliasCommand(coreName, otherName));
        }

        /// <summary>
        /// The CREATE action creates a new core and registers it. If persistence is enabled
        /// (persistent="true" on the &lt;solr&gt; element), the updated configuration for this new core will be
        /// saved in solr.xml. If a core with the given name already exists, it will continue to handle requests
        /// while the new core is initializing. When the new core is ready, it will take new requests and the old core
        /// will be unloaded.
        /// </summary>
        /// <param name="coreName">The name of the new core. Same as "name" on the &lt;core&gt; element.</param>
        /// <param name="instanceDir">The directory where files for this core should be stored. Same as "instanceDir" on the &lt;core&gt; element.</param>
        public ResponseHeader Create(string coreName, string instanceDir) {
            return SendAndParseHeader(new CreateCommand(coreName, instanceDir));
        }

        /// <summary>
        /// The CREATE action creates a new core and registers it. If persistence is enabled
        /// (persistent="true" on the &lt;solr&gt; element), the updated configuration for this new core will be
        /// saved in solr.xml. If a core with the given name already exists, it will continue to handle requests
        /// while the new core is initializing. When the new core is ready, it will take new requests and the old core
        /// will be unloaded.
        /// </summary>
        /// <param name="coreName">The name of the new core. Same as "name" on the &lt;core&gt; element.</param>
        /// <param name="instanceDir">The directory where files for this SolrCore should be stored. Same as "instanceDir" on the &lt;core&gt; element.</param>
        /// <param name="configFile">(Optional) Name of the config file (solrconfig.xml) relative to "instanceDir".</param>
        /// <param name="schemaFile">(Optional) Name of the schema file (schema.xml) relative to "instanceDir".</param>
        /// <param name="dataDir">(Optional) Name of the data directory relative to "instanceDir".</param>
        public ResponseHeader Create(string coreName, string instanceDir, string configFile, string schemaFile, string dataDir) {
            return SendAndParseHeader(new CreateCommand(coreName, instanceDir, configFile, schemaFile, dataDir));
        }

        /// <summary>
        /// The RELOAD action loads a new core from the configuration of an existing, registered core.
        /// While the new core is initializing, the existing one will continue to handle requests.
        /// When the new core is ready, it takes over and the old core is unloaded.
        /// This is useful when you've made changes to a core's configuration on disk, such as adding
        /// new field definitions. Calling the RELOAD action lets you apply the new configuration without
        /// having to restart the Web container.
        /// </summary>
        /// <param name="coreName">The name of the core to be reloaded.</param>
        public ResponseHeader Reload(string coreName) {
            return SendAndParseHeader(new ReloadCommand(coreName));
        }

        /// <summary>
        /// The RENAME action changes the name of a core.
        /// </summary>
        /// <param name="coreName">The name of the core to be renamed.</param>
        /// <param name="otherName">The new name for the core. If the persistent attribute of &lt;solr&gt; is
        /// "true", the new name will be written to solr.xml as the "name" attribute
        /// of the &lt;core&gt; attribute.</param>
        public ResponseHeader Rename(string coreName, string otherName) {
            return SendAndParseHeader(new RenameCommand(coreName, otherName));
        }

        /// <summary>
        /// The STATUS action returns the status of all running cores.
        /// </summary>
        public List<CoreResult> Status() {
            return ParseStatusResponse(Send(new StatusCommand()).Response);
        }

        /// <summary>
        /// The STATUS action returns the status of the named core.
        /// </summary>
        /// <param name="coreName">The name of a core, as listed in the "name" attribute of a &lt;core&gt; element in solr.xml.</param>
        public CoreResult Status(string coreName) {
            return ParseStatusResponse(Send(new StatusCommand(coreName)).Response).FirstOrDefault();
        }

        /// <summary>
        /// SWAP atomically swaps the names used to access two existing cores.
        /// This can be used to swap new content into production. The prior core
        /// remains available and can be swapped back, if necessary. Each core will
        /// be known by the name of the other, after the swap.
        /// </summary>
        /// <param name="coreName">The name of one of the cores to be swapped.</param>
        /// <param name="otherName">The name of one of the cores to be swapped.</param>
        public ResponseHeader Swap(string coreName, string otherName) {
            return SendAndParseHeader(new SwapCommand(coreName, otherName));
        }

        /// <summary>
        /// The UNLOAD action removes a core from Solr. Active requests will
        /// continue to be processed, but no new requests will be sent to the named core.
        /// If a core is registered under more than one name, only the given name is removed.
        /// </summary>
        /// <param name="coreName">The name of the core to be to be removed. If the persistent
        /// attribute of &lt;solr&gt; is set to "true", the &lt;core&gt; element
        /// with this "name" attribute will be removed from solr.xml.</param>
        public ResponseHeader Unload(string coreName) {
            return SendAndParseHeader(new UnloadCommand(coreName, null));
        }

        /// <summary>
        /// The UNLOAD action removes a core from Solr. Active requests will
        /// continue to be processed, but no new requests will be sent to the named core.
        /// If a core is registered under more than one name, only the given name is removed.
        /// </summary>
        /// <param name="coreName">The name of the core to be to be removed. If the persistent
        /// attribute of &lt;solr&gt; is set to "true", the &lt;core&gt; element
        /// with this "name" attribute will be removed from solr.xml.</param>
        /// <param name="deleteIndex">If set to <c>true</c> deletes the index once the core is unloaded.  (Only available in 3.3 and above).</param>
        [Obsolete("Use Unload(string coreName, UnloadCommand.Delete delete) instead")]
        public ResponseHeader Unload(string coreName, bool deleteIndex) {
            return Unload(coreName, UnloadCommand.Delete.Index);
        }

        /// <summary>
        /// The UNLOAD action removes a core from Solr. Active requests will
        /// continue to be processed, but no new requests will be sent to the named core.
        /// If a core is registered under more than one name, only the given name is removed.
        /// </summary>
        /// <param name="coreName">The name of the core to be to be removed. If the persistent
        /// attribute of &lt;solr&gt; is set to "true", the &lt;core&gt; element
        /// with this "name" attribute will be removed from solr.xml.</param>
        /// <param name="delete">If not null, deletes the index once the core is unloaded.  (Only available in 3.3 and above).</param>
        public ResponseHeader Unload(string coreName, UnloadCommand.Delete delete) {
            return SendAndParseHeader(new UnloadCommand(coreName, delete));
        }

        /// <summary>
        /// Merge indexes using their core names to identify them.
        /// Requires Solr 3.3+
        /// </summary>
        /// <param name="destinationCore"></param>
        /// <param name="srcCore"></param>
        /// <param name="srcCores"></param>
        public ResponseHeader Merge(string destinationCore, MergeCommand.SrcCore srcCore, params MergeCommand.SrcCore[] srcCores) {
            return SendAndParseHeader(new MergeCommand(destinationCore, srcCore, srcCores));
        }

        /// <summary>
        /// Merge indexes using their path to identify them.
        /// Requires Solr 1.4+
        /// </summary>
        /// <param name="destinationCore"></param>
        /// <param name="indexDir"></param>
        /// <param name="indexDirs"></param>
        public ResponseHeader Merge(string destinationCore, MergeCommand.IndexDir indexDir, params MergeCommand.IndexDir[] indexDirs) {
            return SendAndParseHeader(new MergeCommand(destinationCore, indexDir, indexDirs));
        }

        /// <summary>
        
        /// Parses the status response.
        /// </summary>
        /// <param name="responseXml">The response XML.</param>
        /// <returns></returns>
        protected List<CoreResult> ParseStatusResponse(string responseXml) {
            var xml = XDocument.Parse( responseXml );
            return resultParser.Parse( xml );
        }
    }

    public class SolrParams : List<KeyValuePair<string, string>>
    {
        public SolrParams AddOptional(string keyPrefix, IDictionary<string, string> values)
        {
            if (values == null || values.Count == 0)
            {
                return this;
            }
            
            foreach (var valueKV in values)
            {
                var key = !string.IsNullOrEmpty(keyPrefix) ? keyPrefix + valueKV.Key : valueKV.Key;
                var value = valueKV.Value;
                AddOptional(key, value);
            }
            return this;
        }

        public SolrParams AddOptional(string key, object value)
        {
            if (value == null)
            {
                return this;
            }
            return AddOptional(key, value.ToString());
        }

        public SolrParams AddOptional(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                Add(new KeyValuePair<string, string>(key, value));

            return this;
        }

        public SolrParams AddRequired(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                Add(new KeyValuePair<string, string>(key, value));
            else
                throw new Exception("Parameter " + key + " is required");

            return this;
        }
    }
}
