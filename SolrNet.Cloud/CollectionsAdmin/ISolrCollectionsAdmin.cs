using System.Collections.Generic;

namespace SolrNet.Cloud.CollectionsAdmin
{
    /// <summary>
    /// See for full API description 
    /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api1 
    /// </summary>
    public interface ISolrCollectionsAdmin
    {
        /// <summary>
        /// Creates a collection
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api1
        /// </summary>
        /// <param name="collection">The name of the collection to be created.</param>
        /// <param name="routerName">
        /// The router name that will be used. The router defines how documents will be distributed among the shards. 
        /// Possible values are implicit or compositeId. The 'implicit' router does not automatically route documents to different shards.  
        /// Whichever shard you indicate on the indexing request (or within each document) will be used as the destination for those documents. 
        /// The 'compositeId' router hashes the value in the uniqueKey field and looks up that hash in the collection's clusterstate to determine which shard will receive the document, 
        /// with the additional ability to manually direct the routing.  When using the 'implicit' router, the shards parameter is required. 
        /// When using the 'compositeId' router, the numShards parameter is required. 
        /// For more information, see also the section Document Routing 
        /// (https://cwiki.apache.org/confluence/display/solr/Shards+and+Indexing+Data+in+SolrCloud#ShardsandIndexingDatainSolrCloud-DocumentRouting).
        /// </param>
        /// <param name="numShards">The number of shards to be created as part of the collection. This is a required parameter when using the 'compositeId' router.</param>
        /// <param name="configName">
        /// Defines the name of the configurations (which must already be stored in ZooKeeper) to use for this collection. 
        /// If not provided, Solr will default to the collection name as the configuration name.
        /// </param>
        /// <param name="shards">A comma separated list of shard names, e.g., shard-x,shard-y,shard-z . This is a required parameter when using the 'implicit' router.</param>
        /// <param name="maxShardsPerNode">
        /// When creating collections, the shards and/or replicas are spread across all available (i.e., live) nodes, 
        /// and two replicas of the same shard will never be on the same node. 
        /// If a node is not live when the CREATE operation is called, it will not get any parts of the new collection, which could lead to too many replicas being created on a single live node. 
        /// Defining maxShardsPerNode sets a limit on the number of replicas CREATE will spread to each node. 
        /// If the entire collection can not be fit into the live nodes, no collection will be created at all.
        /// </param>
        /// <param name="replicationFactor">The number of replicas to be created for each shard.</param>
        /// <param name="createNodeSet">
        /// Allows defining the nodes to spread the new collection across. 
        /// If not provided, the CREATE operation will create shard-replica spread across all live Solr nodes. 
        /// The format is a comma-separated list of node_names, such as localhost:8983_solr, localhost:8984_solr, localhost:8985_solr. 
        /// Alternatively, use the special value of EMPTY to initially create no shard-replica within the new collection and then later use the 
        /// ADDREPLICA (https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api_addreplica) operation to add shard-replica when and where required.
        /// </param>
        /// <param name="createNodeSetShuffle">
        /// Controls wether or not the shard-replicas created for this collection will be assigned to the nodes specified by the createNodeSet in a sequential manner, 
        /// or if the list of nodes should be shuffled prior to creating individual replicas.  
        /// A 'false' value makes the results of a collection creation predictible and gives more exact control over the location of the individual shard-replicas, 
        /// but 'true' can be a better choice for ensuring replicas are distributed evenly across nodes.
        /// Ignored if createNodeSet is not also specified.
        /// </param>
        /// <param name="rooterField">
        /// If this field is specified, the router will look at the value of the field in an input document to compute the hash and identify a shard instead of looking at the uniqueKey field. 
        /// If the field specified is null in the document, the document will be rejected. 
        /// Please note that RealTime Get (https://cwiki.apache.org/confluence/display/solr/RealTime+Get) 
        /// or retrieval by id would also require the parameter _route_ (or shard.keys) to avoid a distributed search.
        /// </param>
        /// <param name="coreProperties">
        /// Set core property name to value. See the section Defining core.properties (https://cwiki.apache.org/confluence/display/solr/Defining+core.properties) 
        /// for details on supported properties and values.
        /// </param>
        /// <param name="autoAddReplicas">
        /// When set to true, enables auto addition of replicas on shared file systems. 
        /// See the section autoAddReplicas Settings (https://cwiki.apache.org/confluence/display/solr/Running+Solr+on+HDFS#RunningSolronHDFS-autoAddReplicasSettings) 
        /// for more details on settings and overrides.
        /// </param>
        /// <param name="rule">Replica placement rules. See the section Rule-based Replica Placement (https://cwiki.apache.org/confluence/display/solr/Rule-based+Replica+Placement) for details.</param>
        /// <param name="snitch">Details of the snitch provider. See the section Rule-based Replica Placement (https://cwiki.apache.org/confluence/display/solr/Rule-based+Replica+Placement) for details</param>
        /// <returns>Operation response header</returns>
        ResponseHeader CreateCollection(string collection,
            string routerName = null,
            int? numShards = null,
            string configName = null,
            string shards = null,
            int? maxShardsPerNode = null,
            int? replicationFactor = null,
            string createNodeSet = null,
            bool? createNodeSetShuffle = null,
            string rooterField = null,
            IReadOnlyDictionary<string, string> coreProperties = null,
            bool? autoAddReplicas = null,
            string rule = null,
            string snitch = null);

        /// <summary>
        /// Deletes a collection by name
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api6
        /// </summary>
        /// <param name="collection">The name of the collection to delete.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader DeleteCollection(string collection);

        /// <summary>
        /// Shards can only created with this API for collections that use the 'implicit' router.
        /// Use SPLITSHARD for collections using the 'compositeId' router.
        /// A new shard with a name can be created for an existing 'implicit' collection.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api8
        /// </summary>
        /// <param name="collection">The name of the collection that includes the shard that will be splitted.</param>
        /// <param name="shard">The name of the shard to be created.</param>
        /// <param name="createNodeSet">
        /// Allows defining the nodes to spread the new collection across. 
        /// If not provided, the CREATE operation will create shard-replica spread across all live Solr nodes. 
        /// The format is a comma-separated list of node_names, such as localhost:8983_solr, localhost:8984_solr, localhost:8985_solr.
        /// </param>
        /// <param name="coreProperties">
        /// Set core property name to value. See the section Defining core.properties (https://cwiki.apache.org/confluence/display/solr/Defining+core.properties) 
        /// for details on supported properties and values.
        /// </param>
        /// <returns>Operation response header</returns>
        ResponseHeader CreateShard(string collection, string shard, string createNodeSet = null, IReadOnlyDictionary<string, string> coreProperties = null);

        /// <summary>
        /// Deleting a shard will unload all replicas of the shard and remove them from clusterstate.json.
        /// It will only remove shards that are inactive, or which have no range given for custom sharding.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api7
        /// </summary>
        /// <param name="collection">The name of the collection that includes the shard to be deleted.</param>
        /// <param name="shard">The name of the shard to be deleted.</param>
        /// <param name="deleteInstanceDir">
        /// By default Solr will delete the entire instanceDir of each replica that is deleted. 
        /// Set this to false to prevent the instance directory from being deleted.
        /// </param>
        /// <param name="deleteDataDir">By default Solr will delete the dataDir of each replica that is deleted. Set this to false to prevent the data directory from being deleted.</param>
        /// <param name="deleteIndex">By default Solr will delete the index of each replica that is deleted. Set this to false to prevent the index directory from being deleted.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader DeleteShard(string collection, string shard, bool? deleteInstanceDir = null, bool? deleteDataDir = null, bool? deleteIndex = null);

        /// <summary>
        /// The RELOAD action is used when you have changed a configuration in ZooKeeper.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api2
        /// </summary>
        /// <param name="collection">The name of the collection to reload.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader ReloadCollection(string collection);

        /// <summary>
        /// Fetch the cluster status including collections, shards, replicas as well as collection aliases and cluster properties.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api18
        /// </summary>
        /// <param name="collection">The collection name for which information is requested. If omitted, information on all collections in the cluster will be returned.</param>
        /// <param name="shard">The shard(s) for which information is requested.Multiple shard names can be specified as a comma separated list.</param>
        /// <param name="route">This can be used if you need the details of the shard where a particular document belongs to and you don't know which shard it falls under.</param>
        /// <returns>Solr Cloud State</returns>
        SolrCloudState GetClusterStatus(string collection, string shard = null, string route = null);


        /// <summary>
        /// Fetch the names of the collections in the cluster.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-List
        /// </summary>
        /// <returns>List of collection names</returns>
        List<string> ListCollections();

        /// <summary>
        /// Modify attributes of a Collection
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-modifycoll
        /// </summary>
        /// <param name="collection">The name of the collection to be modified.</param>
        /// <param name="maxShardsPerNode">
        /// When creating collections, the shards and/or replicas are spread across all available (i.e., live) nodes, 
        /// and two replicas of the same shard will never be on the same node. 
        /// If a node is not live when the CREATE operation is called, 
        /// it will not get any parts of the new collection, which could lead to too many replicas being created on a single live node. 
        /// Defining maxShardsPerNode sets a limit on the number of replicas CREATE will spread to each node. 
        /// If the entire collection can not be fit into the live nodes, no collection will be created at all.
        /// </param>
        /// <param name="replicationFactor">The number of replicas to be created for each shard.</param>
        /// <param name="autoAddReplicas">
        /// When set to true, enables auto addition of replicas on shared file systems. 
        /// See the section autoAddReplicas Settings (https://cwiki.apache.org/confluence/display/solr/Running+Solr+on+HDFS#RunningSolronHDFS-autoAddReplicasSettings) 
        /// for more details on settings and overrides.
        /// </param>
        /// <param name="rule">Replica placement rules. See the section Rule-based Replica Placement (https://cwiki.apache.org/confluence/display/solr/Rule-based+Replica+Placement) for details.</param>
        /// <param name="snitch">Details of the snitch provider. See the section Rule-based Replica Placement (https://cwiki.apache.org/confluence/display/solr/Rule-based+Replica+Placement) for details.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader ModifyCollection(
            string collection,
            int? maxShardsPerNode = null,
            int? replicationFactor = null,
            bool? autoAddReplicas = null,
            string rule = null,
            string snitch = null);

        /// <summary>
        /// Split a Shard.
        /// Splitting a shard will take an existing shard and break it into two pieces which are written to disk as two (new) shards.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api3
        /// </summary>
        /// <param name="collection">The name of the collection that includes the shard to be split.</param>
        /// <param name="shard">The name of the shard to be split.</param>
        /// <param name="ranges">A comma-separated list of hash ranges in hexadecimal e.g. ranges=0-1f4,1f5-3e8,3e9-5dc</param>
        /// <param name="splitKey">The key to use for splitting the index</param>
        /// <param name="coreProperties">
        /// Set core property name to value. 
        /// See the section Defining core.properties (https://cwiki.apache.org/confluence/display/solr/Defining+core.properties) for details on supported properties and values.
        /// </param>
        /// <returns>Operation response header</returns>
        ResponseHeader SplitShard(
            string collection,
            string shard,
            string ranges = null,
            string splitKey = null,
            IReadOnlyDictionary<string, string> coreProperties = null);

        /// <summary>
        /// Create or modify an Alias for a Collection.
        /// The CREATEALIAS action will create a new alias pointing to one or more collections. 
        /// If an alias by the same name already exists, this action will replace the existing alias, effectively acting like an atomic "MOVE" command.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api4
        /// </summary>
        /// <param name="name">The alias name to be created.</param>
        /// <param name="collections">The list of collections to be aliased, separated by commas.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader CreateAlias(string name, string collections);

        /// <summary>
        /// Delete a Collection Alias.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api5
        /// </summary>
        /// <param name="name">The name of the alias to delete.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader DeleteAlias(string name);

        /// <summary>
        /// Delete a Replica.
        /// Delete a named replica from the specified collection and shard. 
        /// If the corresponding core is up and running the core is unloaded, the entry is removed from the clusterstate, and (by default) delete the instanceDir and dataDir.  
        /// If the node/core is down, the entry is taken off the clusterstate and if the core comes up later it is automatically unregistered.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api9
        /// </summary>
        /// <param name="collection">The name of the collection.</param>
        /// <param name="shard">The name of the shard that includes the replica to be removed.</param>
        /// <param name="replica">The name of the replica to remove.</param>
        /// <param name="deleteInstanceDir">
        /// By default Solr will delete the entire instanceDir of the replica that is deleted. 
        /// Set this to false to prevent the instance directory from being deleted.
        /// </param>
        /// <param name="deleteDataDir">
        /// By default Solr will delete the dataDir of the replica that is deleted. 
        /// Set this to false to prevent the data directory from being deleted.
        /// </param>
        /// <param name="deleteIndex">
        /// By default Solr will delete the index of the replica that is deleted. 
        /// Set this to false to prevent the index directory from being deleted.
        /// </param>
        /// <param name="onlyIfDown">When set to 'true' will not take any action if the replica is active. Default 'false'</param>
        /// <returns>Operation response header</returns>
        ResponseHeader DeleteReplica(
            string collection,
            string shard,
            string replica,
            bool? deleteInstanceDir = null,
            bool? deleteDataDir = null,
            bool? deleteIndex = null,
            bool? onlyIfDown = null);

        /// <summary>
        /// Add Replica.
        /// Add a replica to a shard in a collection. The node name can be specified if the replica is to be created in a specific node
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api_addreplica
        /// </summary>
        /// <param name="collection">The name of the collection.</param>
        /// <param name="shard">
        /// The name of the shard to which replica is to be added.
        /// If shard is not specified, then _route_ must be.
        /// </param>
        /// <param name="route">
        /// If the exact shard name is not known, users may pass the _route_ value and the system would identify the name of the shard.
        /// Ignored if the shard param is also specified.
        /// </param>
        /// <param name="node">The name of the node where the replica should be created</param>
        /// <param name="instanceDir">The instanceDir for the core that will be created</param>
        /// <param name="dataDir">The directory in which the core should be created</param>
        /// <param name="coreProperties">Set core property name to value. See Defining core.properties (https://cwiki.apache.org/confluence/display/solr/Defining+core.properties).</param>
        /// <returns>Operation response header</returns>
        ResponseHeader AddReplica(
            string collection,
            string shard = null,
            string route = null,
            string node = null,
            bool? instanceDir = null,
            bool? dataDir = null,
            IReadOnlyDictionary<string, string> coreProperties = null);

        /// <summary>
        /// Cluster Properties.
        /// Add, edit or delete a cluster-wide property.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api11
        /// </summary>
        /// <param name="name">The name of the property. The two supported properties names are urlScheme and autoAddReplicas. Other names are rejected with an error.</param>
        /// <param name="value">The value of the property. If the value is empty or null, the property is unset.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader ClusterPropertySetDelete(string name, string value = null);

        /// <summary>
        /// Migrate Documents to Another Collection.
        /// The MIGRATE command is used to migrate all documents having the given routing key to another collection. 
        /// The source collection will continue to have the same data as-is but it will start re-routing write requests to the target collection for the number of seconds specified by the forward.timeout parameter. 
        /// It is the responsibility of the user to switch to the target collection for reads and writes after the ‘migrate’ command completes.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api12
        /// </summary>
        /// <param name="collection">The name of the source collection from which documents will be split.</param>
        /// <param name="targetCollection">The name of the target collection to which documents will be migrated.</param>
        /// <param name="splitKey">The routing key prefix. For example, if uniqueKey is a!123, then you would use split.key=a!.</param>
        /// <param name="forwardTimeout">
        /// The timeout, in seconds, until which write requests made to the source collection for the given split.key will be forwarded to the target shard. 
        /// The default is 60 seconds.
        /// </param>
        /// <param name="coreProperties">
        /// Set core property name to value. 
        /// See the section Defining core.properties (https://cwiki.apache.org/confluence/display/solr/Defining+core.properties) 
        /// for details on supported properties and values.
        /// </param>
        /// <returns>Operation response header</returns>
        ResponseHeader Migrate(
           string collection,
           string targetCollection,
           string splitKey,
           int? forwardTimeout = null,
           IReadOnlyDictionary<string, string> coreProperties = null);

        /// <summary>
        /// Add Role.
        /// Assign a role to a given node in the cluster.  The only supported role as of 4.7 is 'overseer'. 
        /// Use this API to dedicate a particular node as Overseer. Invoke it multiple times to add more nodes. 
        /// This is useful in large clusters where an Overseer is likely to get overloaded . 
        /// If available, one among the list of nodes which are assigned the 'overseer' role would become the overseer.  
        /// The system would  assign the role to any other node if none of the designated nodes are up and running.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api15AddRole
        /// </summary>
        /// <param name="role">The name of the role. The only supported role as of now is overseer.</param>
        /// <param name="node">The name of the node. It is possible to assign a role even before that node is started.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader AddRole(string role, string node);

        /// <summary>
        /// Remove Role.
        /// Remove an assigned role. This API is used to undo the roles assigned using ADDROLE operation.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-api16RemoveRole
        /// </summary>
        /// <param name="role">The name of the role. The only supported role as of now is overseer.</param>
        /// <param name="node">The name of the node.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader RemoveRole(string role, string node);

        /// <summary>
        /// Add Replica Property.
        /// Assign an arbitrary property to a particular replica and give it the value specified. If the property already exists, it will be overwritten with the new value.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-AddReplicaProp
        /// </summary>
        /// <param name="collection">The name of the collection this replica belongs to.</param>
        /// <param name="shard">The name of the shard the replica belongs to.</param>
        /// <param name="replica">The replica, e.g. core_node1.</param>
        /// <param name="property">The property to add. Note: this will have the literal 'property.' prepended to distinguish it from system-maintained properties. 
        /// So these two forms are equivalent:
        /// property=special
        /// and
        /// property=property.special
        ///</param>
        /// <param name="propertyValue">The value to assign to the property.</param>
        /// <param name="shardUnique">default: false. If true, then setting this property in one replica will remove the property from all other replicas in that shard.</param>
        /// <returns>Operation response header</returns>
        ResponseHeader AddReplicaProperty(string collection, string shard, string replica, string property, string propertyValue, bool? shardUnique = null);

        /// <summary>
        /// Delete Replica Property.
        /// Deletes an arbitrary property from a particular replica.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-DeleteReplicaProp
        /// </summary>
        /// <param name="collection">The name of the collection this replica belongs to.</param>
        /// <param name="shard">The name of the shard the replica belongs to.</param>
        /// <param name="replica">The replica, e.g. core_node1.</param>
        /// <param name="property">The property to add. Note: this will have the literal 'property.' prepended to distinguish it from system-maintained properties. 
        /// So these two forms are equivalent:
        /// property=special
        /// and
        /// property=property.special
        ///</param>
        /// <returns>Operation response header</returns>
        ResponseHeader DeleteReplicaProperty(string collection, string shard, string replica, string property);

        /// <summary>
        /// Balance a Property.
        /// Insures that a particular property is distributed evenly amongst the physical nodes that make up a collection. 
        /// If the property already exists on a replica, every effort is made to leave it there. 
        /// If the property is not on any replica on a shard one is chosen and the property is added.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-BalanceSliceUnique
        /// </summary>
        /// <param name="collection">The name of the collection to balance the property in.</param>
        /// <param name="property">The property to balance. The literal "property." is prepended to this property if not specified explicitly.</param>
        /// <param name="onlyActiveNodes">
        /// Defaults to true. Normally, the property is instantiated on active nodes only. 
        /// If this parameter is specified as "false", then inactive nodes are also included for distribution.
        /// </param>
        /// <param name="shardUnique">
        /// Something of a safety valve. 
        /// There is one pre-defined property (preferredLeader) that defaults this value to "true". 
        /// For all other properties that are balanced, this must be set to "true" or an error message is returned.
        /// </param>
        /// <returns>Operation response header</returns>
        ResponseHeader BalanceShardUnique(string collection, string property, bool? onlyActiveNodes = null, bool? shardUnique = null);

        /// <summary>
        /// Rebalance Leaders.
        /// Reassign leaders in a collection according to the preferredLeader property across active nodes.
        /// Assigns leaders in a collection according to the preferredLeader property on active nodes. 
        /// This command should be run after the preferredLeader property has been assigned via the BALANCESHARDUNIQUE or ADDREPLICAPROP commands. 
        /// NOTE: it is not required that all shards in a collection have a preferredLeader property. 
        /// Rebalancing will only attempt to reassign leadership to those replicas that have the preferredLeader property set to "true" and are not currently the shard leader and are currently active.
        /// https://cwiki.apache.org/confluence/display/solr/Collections+API#CollectionsAPI-RebalanceLeaders
        /// </summary>
        /// <param name="collection">The name of the collection to rebalance preferredLeaders on.</param>
        /// <param name="maxAtOnce">
        /// The maximum number of reassignments to have queue up at once. Values &lt;= 0 are use the default value Integer.MAX_VALUE. 
        /// When this number is reached, the process waits for one or more leaders to be successfully assigned before adding more to the queue.
        /// </param>
        /// <param name="maxWaitSeconds">
        /// Defaults to 60. This is the timeout value when waiting for leaders to be reassigned. 
        /// NOTE: if maxAtOnce is less than the number of reassignments that will take place, this is the maximum interval that any single wait for at least one reassignment. 
        /// For example, if 10 reassignments are to take place and maxAtOnce is 1 and maxWaitSeconds is 60, the upper bound on the time that the command may wait is 10 minutes.
        /// </param>
        /// <returns>Operation response header</returns>
        ResponseHeader RebalanceLeaders(string collection, string maxAtOnce = null, string maxWaitSeconds = null);
    }
}
