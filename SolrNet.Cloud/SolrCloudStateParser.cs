using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud
{
    /// <summary>
    /// Replica `state` is used when its node name is found in the list of liveNodes
    /// Shard `state` is used when at least one replica is a leader AND is active
    /// </summary>
    public static class SolrCloudStateParser
    {
        /// <summary>
        /// Returns parsed solr cloud state
        /// If liveNodes is passed then it will only consider shards and replicas that are live
        /// </summary>
        public static SolrCloudState Parse(string json, IReadOnlyCollection<string> liveNodes = null)
        {
            return new SolrCloudState(
                JObject.Parse(json).Properties()
                    .Select(jProperty => BuildCollection(jProperty, liveNodes))
                    .ToDictionary(collection => collection.Name, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Builds cloud collection model from json object
        /// </summary>
        private static SolrCloudCollection BuildCollection(JProperty json, IReadOnlyCollection<string> liveNodes)
        {
            var shards = (JObject)json.Value["shards"];
            return new SolrCloudCollection(
                json.Name,
                BuildRouter(json.Value["router"] as JObject),
                shards.Properties()
                    .Select(property => BuildShard(json.Name, property, liveNodes))
                    .ToDictionary(shard => shard.Name, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Builds cloud replica model from json object
        /// </summary>
        private static SolrCloudReplica BuildReplica(string collection, JProperty json, IReadOnlyCollection<string> liveNodes)
        {
            var baseUrl = (string)json.Value["base_url"];
            var leader = json.Value["leader"];
            var state = (string)json.Value["state"];
            var nodeName = (string)json.Value["node_name"];

            return new SolrCloudReplica(
                IsActiveReplica(state, nodeName, liveNodes),
                leader != null && (bool)leader,
                json.Name,
                baseUrl + "/" + collection);
        }

        /// <summary>
        /// Builds cloud router model from json object
        /// </summary>
        private static SolrCloudRouter BuildRouter(JObject json)
        {
            return new SolrCloudRouter(
                (string)json["name"]);
        }

        /// <summary>
        /// Builds cloud shard model from json object
        /// </summary>
        private static SolrCloudShard BuildShard(string collection, JProperty json, IReadOnlyCollection<string> liveNodes)
        {
            var state = (string)json.Value["state"];
            var range = (string)json.Value["range"];
            int? rangeEnd = null;
            int? rangeStart = null;
            if (!string.IsNullOrEmpty(range))
            {
                var parts = range.Split('-');
                rangeStart = int.Parse(parts[0], NumberStyles.HexNumber);
                rangeEnd = int.Parse(parts[1], NumberStyles.HexNumber);
            }
            var replicas = (JObject)json.Value["replicas"];

            var solrCloudReplicas = replicas.Properties()
                .Select(property => BuildReplica(collection, property, liveNodes))
                .ToDictionary(replica => replica.Name, StringComparer.OrdinalIgnoreCase);

            return new SolrCloudShard(
                IsActiveShard(state, solrCloudReplicas, liveNodes),
                json.Name,
                rangeEnd,
                rangeStart,
                solrCloudReplicas);
        }

        /// <summary>
        /// Checks if state is active
        /// </summary>
        private static bool IsActive(string state)
        {
            return "active".Equals(state, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsActiveReplica(string state, string nodeName, IReadOnlyCollection<string> liveNodes)
        {
            var isLiveNode = liveNodes?.Contains(nodeName) ?? true;
            return isLiveNode && IsActive(state);
        }

        private static bool IsActiveShard(string state, IReadOnlyDictionary<string, SolrCloudReplica> solrCloudReplicas, IReadOnlyCollection<string> liveNodes)
        {
            var isLiveNode = liveNodes == null || solrCloudReplicas.Any(pair => pair.Value.IsActive && pair.Value.IsLeader);

            return isLiveNode && IsActive(state);
        }
    }
}
