using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SolrNet.Commands.Cores;

namespace SolrNet.Impl {
    /// <summary>
    /// Solr core administration commands.
    /// </summary>
    /// <seealso href="http://wiki.apache.org/solr/CoreAdmin"/>
    public class SolrCoreAdmin : ISolrCoreAdmin {
        private readonly ISolrConnection connection;
        private readonly ISolrHeaderResponseParser headerParser;
        private readonly ISolrStatusResponseParser resultParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrCoreAdmin"/> class.
        /// </summary>
        public SolrCoreAdmin(ISolrConnection connection, ISolrHeaderResponseParser headerParser, ISolrStatusResponseParser resultParser) {
            this.connection = connection;
            this.headerParser = headerParser;
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
            return ParseStatusResponse(Send(new StatusCommand()));
        }

        /// <summary>
        /// The STATUS action returns the status of the named core.
        /// </summary>
        /// <param name="coreName">The name of a core, as listed in the "name" attribute of a &lt;core&gt; element in solr.xml.</param>
        public List<CoreResult> Status(string coreName) {
            return ParseStatusResponse(Send(new StatusCommand(coreName)));
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
            return SendAndParseHeader(new UnloadCommand(coreName));
        }

        /// <summary>
        /// The UNLOAD action removes a core from Solr. Active requests will
        /// continue to be processed, but no new requests will be sent to the named core.
        /// If a core is registered under more than one name, only the given name is removed.
        /// </summary>
        /// <param name="coreName">The name of the core to be to be removed. If the persistent
        /// attribute of &lt;solr&gt; is set to "true", the &lt;core&gt; element
        /// with this "name" attribute will be removed from solr.xml.</param>
        /// <param name="deleteIndex"></param>
        public ResponseHeader Unload(string coreName, bool deleteIndex) {
            return SendAndParseHeader(new UnloadCommand(coreName, deleteIndex));
        }

        /// <summary>
        /// Sends a command and parses the ResponseHeader.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public ResponseHeader SendAndParseHeader(ISolrCommand cmd) {
            var r = Send(cmd);
            var xml = XDocument.Parse(r);
            return headerParser.Parse(xml);
        }

        /// <summary>
        /// Sends the specified Command to Solr.
        /// </summary>
        /// <param name="command">The Command to send.</param>
        /// <returns></returns>
        public string Send(ISolrCommand command) {
            return command.Execute(connection);
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
}