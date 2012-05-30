using System;
using System.Collections.Generic;
using SolrNet.Utils;

namespace SolrNet {
    /// <summary>
    /// The options to make a DataImportHandler request.
    /// </summary>
    public class DIHOptions {
        /// <summary>
        /// The data handler options.
        /// </summary>
        /// <param name="handlerName">the name of the handler to query. This argument cannot be null.</param>
        public DIHOptions(string handlerName = "dataimport") {
            if (handlerName == null)
                throw new ArgumentNullException("handlerName", "you must provide a handler name other than null");
            HandlerName = handlerName;
        }

        /// <summary>
        /// Name of the data import request handler.
        /// Default is "dataimport".
        /// </summary>
        public string HandlerName { get; set; }

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

        public IEnumerable<KeyValuePair<string, string>> ToParameters() {
            if (Clean.HasValue)
                yield return KV.Create("clean", Clean.ToString().ToLower());
            if (Commit.HasValue)
                yield return KV.Create("commit", Commit.ToString().ToLower());
            if (Optimize.HasValue)
                yield return KV.Create("optimize", Optimize.ToString().ToLower());
            if (Debug.HasValue)
                yield return KV.Create("debug", Debug.ToString().ToLower());
        }
    }
}