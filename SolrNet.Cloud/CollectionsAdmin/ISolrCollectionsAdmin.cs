using System.Collections.Generic;

namespace SolrNet.Cloud.CollectionsAdmin
{
    /// <summary>
    /// See for full API description 
    /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api1 
    /// </summary>
    public interface ISolrCollectionsAdmin {
        /// <summary>
        /// Creates a collection
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api1
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="routerName"></param>
        /// <param name="numShards"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        ResponseHeader CreateCollection(string collection, string routerName = null, int? numShards = null, string configName = null, string shards=null, int? maxShardsPerNode=null);

        /// <summary>
        /// Deletes a collection by name
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api6
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        ResponseHeader DeleteCollection(string collection);

        /// <summary>
        /// Shards can only created with this API for collections that use the 'implicit' router.
        /// Use SPLITSHARD for collections using the 'compositeId' router.
        /// A new shard with a name can be created for an existing 'implicit' collection.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api8
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="shard"></param>
        /// <returns></returns>
        ResponseHeader CreateShard(string collection, string shard);

        /// <summary>
        /// Deleting a shard will unload all replicas of the shard and remove them from clusterstate.json.
        /// It will only remove shards that are inactive, or which have no range given for custom sharding.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api7
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="shard"></param>
        /// <returns></returns>
        ResponseHeader DeleteShard(string collection, string shard);

        /// <summary>
        /// The RELOAD action is used when you have changed a configuration in ZooKeeper.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api2
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        ResponseHeader ReloadCollection(string collection);

        /// <summary>
        /// Fetch the cluster status including collections, shards, replicas as well as collection aliases and cluster properties.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api18
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="shard"></param>
        /// <returns></returns>
        SolrCloudState GetClusterStatus(string collection, string shard = null);


        /// <summary>
        /// Fetch the names of the collections in the cluster.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-List
        /// </summary>
        /// <returns></returns>
        List<string> ListCollections();
    }
}
