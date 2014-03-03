using System.Collections.Generic;
using SolrNet.Impl;

namespace SolrNet {
    ///<summary>
    /// Manages Solr core replication.
    ///</summary>
    public interface ISolrCoreReplication 
    {
        /// <summary>
        /// Enables replication on the master for all its slaves. 
        /// </summary>
        /// <returns></returns>
        ReplicationStatusResponse EnableReplication();

        /// <summary>
        /// Disables replication on the master for all its slaves. 
        /// </summary>
        /// <returns></returns>
        ReplicationStatusResponse DisableReplication();

        /// <summary>
        /// Returns the version of the latest replicatable index on the specified master or slave. 
        /// </summary>
        /// <returns></returns>
        ReplicationIndexVersionResponse IndexVersion();

        /// <summary>
        /// Retrieves configuration details and current status. 
        /// </summary>
        /// <returns></returns>
        ReplicationDetailsResponse Details();

        /// <summary>
        /// Enables the specified slave to poll for changes on the master. 
        /// </summary>
        /// <returns></returns>
        ReplicationStatusResponse EnablePoll();

        /// <summary>
        /// Enables the specified slave to poll for changes on the master. 
        /// </summary>
        /// <returns></returns>
        ReplicationStatusResponse DisablePoll();

        /// <summary>
        /// Forces the specified slave to fetch a copy of the index from its master. If you like, you 
        /// can pass an extra attribute such as masterUrl or compression (or any other parameter which 
        /// is specified in the &lt;lst name="slave"&gt; tag) to do a one time replication from a master. 
        /// This obviates the need for hard-coding the master in the slave. 
        /// </summary>
        ReplicationStatusResponse FetchIndex();

        /// <summary>
        /// Forces the specified slave to fetch a copy of the index from its master. If you like, you 
        /// can pass an extra attribute such as masterUrl or compression (or any other parameter which 
        /// is specified in the &lt;lst name="slave"&gt; tag) to do a one time replication from a master. 
        /// This obviates the need for hard-coding the master in the slave. 
        /// </summary>
        /// <param name="parameters">Optional parameters</param>
        ReplicationStatusResponse FetchIndex(IEnumerable<KeyValuePair<string, string>> parameters);

        /// <summary>
        /// Aborts copying an index from a master to the specified slave.
        /// </summary>
        ReplicationStatusResponse AbortFetch();
    }
}