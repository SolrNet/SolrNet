using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolrNet.Commands 
{
    /// <summary>
    /// Replication command
    /// http://wiki.apache.org/solr/SolrReplication
    /// https://cwiki.apache.org/confluence/display/solr/Index+Replication
    /// </summary>
    public class ReplicationCommand : ISolrCommand 
    {
        /// <summary>
        /// List of Parameters that will be sent to the /replication handler.
        /// </summary>
        protected readonly List<KeyValuePair<string, string>> Parameters = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Executes a Replication command
        /// </summary>
        /// <param name="connection">The SolrConnection to use.</param>
        /// <returns>The results of the Command.</returns>
        public string Execute(ISolrConnection connection) 
        {
            return connection.Get("/replication", Parameters.ToArray());
        }

        /// <summary>
        /// Executes a Replication command
        /// </summary>
        /// <param name="connection">The SolrConnection to use.</param>
        /// <returns>The results of the Command.</returns>
        public Task<string> ExecuteAsync(ISolrConnection connection)
        {
            return connection.GetAsync("/replication", Parameters.ToArray());
        }

        /// <summary>
        /// Adds the specified parameter to the current command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected void AddParameter(string key, string value) 
        {
            Parameters.Add(new KeyValuePair<string, string>(key, value));
        }
    }
}
