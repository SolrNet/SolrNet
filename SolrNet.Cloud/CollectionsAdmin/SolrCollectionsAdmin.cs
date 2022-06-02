using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using SolrNet.Impl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud.CollectionsAdmin {
    public class SolrCollectionsAdmin : LowLevelSolrServer, ISolrCollectionsAdmin
    {
        private const string AdminHandler = "/admin/collections";

        /// <summary>
        /// prefix before core properties (to form property like 'property.name=value' for a given name-value pair)
        /// <see cref="https://cwiki.apache.org/confluence/display/solr/Defining+core.properties"/>
        /// </summary>
        private const string CORE_PROPERTY_KEY_PREFIX = "property.";

        public SolrCollectionsAdmin(ISolrConnection connection, ISolrHeaderResponseParser headerParser)
            : base(connection, headerParser)
        {
        }

        public ResponseHeader CreateCollection(
            string collection, 
            string routerName = null, 
            int? numShards = null, 
            string configName = null, 
            string shards = null, 
            int? maxShardsPerNode = null,
            int? replicationFactor = null,
            string createNodeSet = null,
            bool? createNodeSetShuffle = null,
            string routerField = null,
            IReadOnlyDictionary<string, string> coreProperties = null,
            bool? autoAddReplicas = null,
            string rule = null,
            string snitch = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "create")
                .AddRequired("name", collection)
                .AddOptional("router.name", routerName)
                .AddOptional("numShards", numShards)
                .AddOptional("collection.configName", configName)
                .AddOptional("shards", shards)
                .AddOptional("maxShardsPerNode", maxShardsPerNode.ToString())
                .AddOptional("replicationFactor", replicationFactor)
                .AddOptional("createNodeSet", createNodeSet)
                .AddOptional("createNodeSet.shuffle", createNodeSetShuffle)
                .AddOptional("router.field", routerField)
                .AddOptional(CORE_PROPERTY_KEY_PREFIX, coreProperties)
                .AddOptional("autoAddReplicas", autoAddReplicas)
                .AddOptional("rule", rule)
                .AddOptional("snitch", snitch);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader DeleteCollection(string collection)
        {
            var solrPapams = new SolrParams()
                .AddRequired("action", "delete")
                .AddRequired("name", collection);

            return SendAndParseHeader(AdminHandler, solrPapams);
        }

        public ResponseHeader CreateShard(
            string collection, 
            string shard,
            string createNodeSet = null,
            IReadOnlyDictionary<string, string> coreProperties = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "createshard")
                .AddRequired("collection", collection)
                .AddRequired("shard", shard)
                .AddOptional("createNodeSet", createNodeSet)
                .AddOptional(CORE_PROPERTY_KEY_PREFIX, coreProperties);

            return SendAndParseHeader(AdminHandler, solrParams);
        }        

        public ResponseHeader DeleteShard(
            string collection, 
            string shard,
            bool? deleteInstanceDir = null,
            bool? deleteDataDir = null,
            bool? deleteIndex = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "deleteshard")
                .AddRequired("collection", collection)
                .AddRequired("shard", shard)
                .AddOptional("deleteInstanceDir", deleteInstanceDir)
                .AddOptional("deleteDataDir", deleteDataDir)
                .AddOptional("deleteIndex", deleteIndex);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader ReloadCollection(string collection)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "reload")
                .AddRequired("name", collection);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public SolrCloudState GetClusterStatus(string collection, string shard = null, string route = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("wt", "json")
                .AddRequired("action", "clusterstate")
                .AddOptional("shard", shard)
                .AddOptional("_route_", route);

            var json = SendRaw(AdminHandler, solrParams);

            var status = SolrCloudStateParser.Parse(json);

            return status;
        }

        public List<string> ListCollections()
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "list");

            var results = Send(AdminHandler, solrParams);

            var paramNodes = results.XPathSelectElements("response/arr[@name='collections']/str");

            return paramNodes.Select(n => n.Value).ToList();
        }

        public ResponseHeader ModifyCollection(
            string collection,
            int? maxShardsPerNode = null,
            int? replicationFactor = null,
            bool? autoAddReplicas = null,
            string rule = null,
            string snitch = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "MODIFYCOLLECTION")
                .AddRequired("collection", collection)
                .AddOptional("maxShardsPerNode", maxShardsPerNode.ToString())
                .AddOptional("replicationFactor", replicationFactor)
                .AddOptional("autoAddReplicas", autoAddReplicas)
                .AddOptional("rule", rule)
                .AddOptional("snitch", snitch);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader SplitShard(
            string collection, 
            string shard,
            string ranges = null,
            string splitKey = null,
            IReadOnlyDictionary<string, string> coreProperties = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "SPLITSHARD")
                .AddRequired("collection", collection)
                .AddRequired("shard", shard)
                .AddOptional("ranges", ranges)
                .AddOptional("split.key", splitKey)
                .AddOptional(CORE_PROPERTY_KEY_PREFIX, coreProperties);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader CreateAlias(
            string name,
            string collections)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "CREATEALIAS")
                .AddRequired("name", name)
                .AddRequired("collections", collections);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader DeleteAlias(string name)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "DELETEALIAS")
                .AddRequired("name", name);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader DeleteReplica(
            string collection,
            string shard,
            string replica,
            bool? deleteInstanceDir = null,
            bool? deleteDataDir = null,
            bool? deleteIndex = null,
            bool? onlyIfDown = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "DELETEREPLICA")
                .AddRequired("collection", collection)
                .AddRequired("shard", shard)
                .AddRequired("replica", replica)
                .AddOptional("deleteInstanceDir", deleteInstanceDir)
                .AddOptional("deleteDataDir", deleteDataDir)
                .AddOptional("deleteIndex", deleteIndex)
                .AddOptional("onlyIfDown", onlyIfDown);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader AddReplica(
            string collection,
            string shard = null,
            string route = null,
            string node = null,
            bool? instanceDir = null,
            bool? dataDir = null,
            IReadOnlyDictionary<string, string> coreProperties = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "ADDREPLICA")
                .AddRequired("collection", collection)
                .AddOptional("shard", shard)
                .AddOptional("_route_", route)
                .AddOptional("node", node)
                .AddOptional("instanceDir", instanceDir)
                .AddOptional("dataDir", dataDir)
                .AddOptional(CORE_PROPERTY_KEY_PREFIX, coreProperties);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader ClusterPropertySetDelete(
            string name,
            string value = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "CLUSTERPROP")
                .AddRequired("name", name)
                .AddOptional("value", value);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader Migrate(
           string collection,
           string targetCollection,
           string splitKey,
           int? forwardTimeout = null,
           IReadOnlyDictionary<string, string> coreProperties = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "MIGRATE")
                .AddRequired("collection", collection)
                .AddRequired("target.collection", targetCollection)
                .AddRequired("split.key", splitKey)
                .AddOptional("forward.timeout", forwardTimeout)
                .AddOptional(CORE_PROPERTY_KEY_PREFIX, coreProperties);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader AddRole(
            string role,
            string node)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "ADDROLE")
                .AddRequired("role", role)
                .AddRequired("node", node);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader RemoveRole(
            string role,
            string node)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "REMOVEROLE")
                .AddRequired("role", role)
                .AddRequired("node", node);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader AddReplicaProperty(
            string collection,
            string shard,
            string replica,
            string property,
            string propertyValue,
            bool? shardUnique = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "ADDREPLICAPROP")
                .AddRequired("collection", collection)
                .AddRequired("shard", shard)
                .AddRequired("replica", replica)
                .AddRequired("property", property)
                .AddRequired("property.value", propertyValue)
                .AddOptional("shardUnique", shardUnique);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader DeleteReplicaProperty(
            string collection,
            string shard,
            string replica,
            string property)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "DELETEREPLICAPROP")
                .AddRequired("collection", collection)
                .AddRequired("shard", shard)
                .AddRequired("replica", replica)
                .AddRequired("property", property);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader BalanceShardUnique(
            string collection,
            string property,
            bool? onlyActiveNodes = null,
            bool? shardUnique = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "BALANCESHARDUNIQUE")
                .AddRequired("collection", collection)
                .AddRequired("property", property)
                .AddOptional("onlyactivenodes", onlyActiveNodes)
                .AddOptional("shardUnique", shardUnique);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public ResponseHeader RebalanceLeaders(
            string collection,
            string maxAtOnce = null,
            string maxWaitSeconds = null)
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "REBALANCELEADERS")
                .AddRequired("collection", collection)
                .AddOptional("maxAtOnce", maxAtOnce)
                .AddOptional("maxWaitSeconds", maxWaitSeconds);

            return SendAndParseHeader(AdminHandler, solrParams);
        }

        public OverseerStatusClass OverseerStatus()
        {
            var solrParams = new SolrParams()
                .AddRequired("action", "OVERSEERSTATUS")
                .AddRequired("wt", "json");

            var overseerStatusJsonString = SendRaw(AdminHandler, solrParams);
            var overseerStatus = OverseerStatusClass.Parse(overseerStatusJsonString);
            return overseerStatus;
        }
    }

    public class OverseerStatusClass
    {
        public class ResponseHeaderClass
        {
            public int Status { get; set; }

            public int QTime { get; set; }
        }

        public class OverseerOperationsClass
        {
            public string OperationType { get; set; }

            public int Requests { get; set; }

            public int Errors { get; set; }

            public float TotalTime { get; set; }

            public float AvgRequestsPerMinute { get; set; }

            [JsonProperty("5minRateRequestsPerMinute")]
            public double Rate5minRateRequestsPerMinute { get; set; }

            [JsonProperty("15minRateRequestsPerMinute")]
            public double Rate15minRateRequestsPerMinute { get; set; }

            public float AvgTimePerRequest { get; set; }

            public float MedianRequestTime { get; set; }

            [JsonProperty("75thPctlRequestTime")]
            public float Percentile75thPctlRequestTime { get; set; }

            [JsonProperty("95thPctlRequestTime")]
            public float Percentile95thPctlRequestTime { get; set; }

            [JsonProperty("99thPctlRequestTime")]
            public float Percentile99thPctlRequestTime { get; set; }

            [JsonProperty("999thPctlRequestTime")]
            public float Percentile999thPctlRequestTime { get; set; }
        }

        public ResponseHeaderClass ResponseHeader { get; set; }

        public string Leader { get; set; }

        [JsonProperty("overseer_queue_size")]
        public int QueueSize { get; set; }

        [JsonProperty("overseer_work_queue_size")]
        public int WorkQueueSize { get; set; }

        [JsonProperty("overseer_collection_queue_size")]
        public int CollectionQueueSize { get; set; }

        public Dictionary<string, OverseerOperationsClass> OverseerOperations { get; set; }

        public Dictionary<string, OverseerOperationsClass> CollectionOperations { get; set; }

        public Dictionary<string, OverseerOperationsClass> OverseerQueue { get; set; }

        public Dictionary<string, OverseerOperationsClass> OverseerInternalQueue { get; set; }

        public Dictionary<string, OverseerOperationsClass> CollectionQueue { get; set; }

        public static OverseerStatusClass Parse(string jsonString)
        {
            var jobject = JToken.Parse(jsonString);
            var overseerStatus = jobject.ToObject<OverseerStatusClass>();
            
            overseerStatus.OverseerOperations = ParseOperationDict( jobject["overseer_operations"]);
            overseerStatus.CollectionOperations = ParseOperationDict(jobject["collection_operations"]);
            overseerStatus.OverseerQueue = ParseOperationDict(jobject["overseer_queue"]);
            overseerStatus.OverseerInternalQueue = ParseOperationDict(jobject["overseer_internal_queue"]);
            overseerStatus.CollectionQueue = ParseOperationDict(jobject["collection_queue"]);


            return overseerStatus;
        }

        private static Dictionary<string, OverseerOperationsClass> ParseOperationDict(JToken operationJArray)
        {
            var operationsDict = new Dictionary<string, OverseerOperationsClass>();
            string operationType = null;
            foreach (var operationJObject in operationJArray)
            {
                if (operationJObject is JObject)
                {
                    var operation = (operationJObject as JObject).ToObject<OverseerOperationsClass>();
                    operation.OperationType = operationType;
                    operationsDict[operationType] = operation;
                }
                else
                {
                    operationType = operationJObject.ToString();
                }
            }
            return operationsDict;
        }
    }
}
