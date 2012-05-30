using System.Linq;
using System.Xml.Linq;
using SolrNet.Impl;
using SolrNet.Utils;

namespace SolrNet {
    public class SolrDIHOperations : ISolrDIHOperations {
        private const string DefaultHandlerName = "dataimport";
        private readonly ISolrConnection connection;
        private readonly ISolrDIHStatusParser dihStatusParser;

        public SolrDIHOperations(ISolrConnection connection, ISolrDIHStatusParser dihStatusParser) {
            this.connection = connection;
            this.dihStatusParser = dihStatusParser;
        }

        /// <summary>
        /// Full Import operation
        /// </summary>
        /// <param name="options">The data import options</param>
        /// <returns></returns>
        public SolrDIHStatus FullImport(DIHOptions options) {
            return GetAndParse("full-import", options);
        }

        /// <summary>
        /// For incremental imports and change detection.
        /// It supports the same clean, commit, optimize and debug parameters.
        /// </summary>
        /// <param name="options">The data import options</param>
        /// <returns></returns>
        public SolrDIHStatus DeltaImport(DIHOptions options) {
            return GetAndParse("delta-import", options);
        }

        /// <summary>
        ///  If the data-config is changed and you wish to reload the file without restarting Solr.
        /// </summary>
        /// <param name="handlerName">The name of the data import handler. default to "dataimport"</param>
        /// <returns></returns>
        public SolrDIHStatus ReloadConfig(string handlerName = DefaultHandlerName) {
            return GetAndParse("reload-config", new DIHOptions(handlerName ?? DefaultHandlerName));
        }

        /// <summary>
        /// Abort an ongoing operation.
        /// </summary>
        /// <param name="handlerName">The name of the data import handler. default to "dataimport"</param>
        /// <returns></returns>
        public SolrDIHStatus Abort(string handlerName = DefaultHandlerName) {
            return GetAndParse("abort", new DIHOptions(handlerName));
        }

        /// <summary>
        /// To know the status of the current command.
        /// It gives an elaborate statistics on no. of docs created, deleted, queries run, rows fetched, status etc.
        /// </summary>
        /// <param name="handlerName">The name of the data import handler. default to "dataimport"</param>
        /// <returns></returns>
        public SolrDIHStatus Status(string handlerName = DefaultHandlerName) {
            return GetAndParse("status", new DIHOptions(handlerName ?? DefaultHandlerName));
        }

        private SolrDIHStatus GetAndParse(string command, DIHOptions options) {
            return ParseDihStatus(Get(command, options));
        }

        private SolrDIHStatus ParseDihStatus(string response) {
            var xdocument = XDocument.Parse(response);
            return dihStatusParser.Parse(xdocument);
        }

        private string Get(string command, DIHOptions options) {
            return connection.Get(string.Format("/{0}", options.HandlerName ?? DefaultHandlerName), new[] {
                KV.Create("command", command)
            }.Concat(options.ToParameters()));
        }
    }
}