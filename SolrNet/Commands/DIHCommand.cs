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
        /// Default to true in Solr.
        /// </summary>
        public bool? Clean { get; set; }

        /// <summary>
        /// Tells whether to commit after the operation.
        /// Default to true in Solr.
        /// </summary>
        public bool? Commit { get; set; }

        /// <summary>
        /// Tells whether to optimize after the operation. 
        /// Please note: this can be a very expensive operation and usually does not make sense for delta-imports.
        /// Default to true up to Solr 3.6, false afterwards.
        /// </summary>
        public bool? Optimize { get; set; }

        /// <summary>
        /// Runs in debug mode.
        /// Please note that in debug mode, documents are never committed automatically. 
        /// If you want to run debug mode and commit the results too, add 'commit=true' as a request parameter.
        /// Default to false in Solr.
        /// </summary>
        public bool? Debug { get; set; }

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
            if (Commit.HasValue)
                yield return KV.Create("commit", Commit.ToString().ToLower());
            if (Optimize.HasValue)
                yield return KV.Create("optimize", Optimize.ToString().ToLower());
            if (Debug.HasValue)
                yield return KV.Create("debug", Debug.ToString().ToLower());
        }

        private string CommandValue(DIHCommands commands) {
            switch (commands) {
                case DIHCommands.FullImport:
                    return "full-import";
                case DIHCommands.DeltaImport:
                    return "delta-import";
                case DIHCommands.ReloadConfig:
                    return "reload-config";
                case DIHCommands.Abort:
                    return "abort";
                default:
                    return "";
            }
        }
    }
}