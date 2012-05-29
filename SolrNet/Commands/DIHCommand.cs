using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet.Utils;

namespace SolrNet.Commands {
    /// <summary>
    /// Request solr to perform a data import.
    /// </summary>
    public class DIHCommand : ISolrCommand {
        /// <summary>
        /// Name of the data import request handler.
        /// Default is dataimport.
        /// </summary>
        public string HandlerName { get; set; }

        /// <summary>
        /// Define what kind of import to perform. 
        /// If no CommandType is set, the handler returns the status (e.g.: idle).
        /// </summary>
        public DIHCommands? Command { get; set; }

        /// <summary>
        /// Tells whether to clean up the index before the indexing is started.
        /// </summary>
        public bool? Clean { get; set; }

        public DIHCommand() {
            HandlerName = "dataimport";
        }

        public string Execute(ISolrConnection connection) {
            string url = string.Format("/{0}", HandlerName);
            var parameters = GetParameters();
            return connection.Get(url, parameters);
        }

        private IEnumerable<KeyValuePair<string, string>> GetParameters() {
            if (Command.HasValue)
                yield return KV.Create("command", CommandValue(Command.Value));
            if (Clean.HasValue)
                yield return KV.Create("clean", Clean.ToString().ToLower());
        }

        private string CommandValue(DIHCommands commands) {
            switch (commands) {
                case DIHCommands.FullImport:
                    return "full-import";
                case DIHCommands.DeltaImport:
                    return "delta-import";
                default:
                    return "";
            }
        }
    }
}