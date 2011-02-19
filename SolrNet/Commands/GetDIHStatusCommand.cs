using System;
using System.Collections.Generic;
using System.Text;
using SolrNet.Commands.Parameters;

namespace SolrNet.Commands
{
    /// <summary>
    /// Gets the raw Solr schema
    /// </summary>
    public class GetDIHStatusCommand : ISolrCommand
    {
        private KeyValuePair<string, string> options;

        /// <summary>
        /// Get DIH Status Command
        /// </summary>
        /// <param name="options"></param>
        public GetDIHStatusCommand(KeyValuePair<string, string> options)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets the raw Solr schema
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string Execute(ISolrConnection connection)
        {
            return connection.Get("/dataimport", new[] { new KeyValuePair<string, string>() });
        }
    }
}
