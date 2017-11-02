using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SolrNet.Cloud{
    /// <summary>
    /// Cloud state parser
    /// </summary>
    public static class SolrCloudStateParser {
        /// <summary>
        /// Returns parsed solr cloud state
        /// </summary>
        public static SolrCloudState Parse(string json)
        {
            return new SolrCloudState(
                JObject.Parse(json).Properties()
                    .Select(BuildCollection)
                    .ToDictionary(colection => colection.Name, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Builds cloud collection model from json object
        /// </summary>
        private static SolrCloudCollection BuildCollection(JProperty json) {
            var shards = (JObject) json.Value["shards"];
            return new SolrCloudCollection(
                json.Name,
                BuildRouter(json.Value["router"] as JObject),
                shards.Properties()
                    .Select(property => BuildShard(json.Name, property))
                    .ToDictionary(shard => shard.Name, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Builds cloud replica model from json object
        /// </summary>
        private static SolrCloudReplica BuildReplica(string collection, JProperty json) {
            var baseUrl = (string) json.Value["base_url"];
            var leader = json.Value["leader"];
            var state = (string) json.Value["state"];
            return new SolrCloudReplica(
                IsActive(state),
                leader != null && (bool) leader,
                json.Name,
                baseUrl + "/" + collection);
        }

        /// <summary>
        /// Builds cloud router model from json object
        /// </summary>
        private static SolrCloudRouter BuildRouter(JObject json) {
            return new SolrCloudRouter(
                (string) json["name"]);
        }

        /// <summary>
        /// Builds cloud shard model from json object
        /// </summary>
        private static SolrCloudShard BuildShard(string collection, JProperty json) {
            var state = (string) json.Value["state"];
            var range = (string) json.Value["range"];
            int? rangeEnd = null;
            int? rangeStart = null;
            if (!string.IsNullOrEmpty(range)) {
                var parts = range.Split('-');
                rangeStart = int.Parse(parts[0], NumberStyles.HexNumber);
                rangeEnd = int.Parse(parts[1], NumberStyles.HexNumber);
            }
            var replicas = (JObject) json.Value["replicas"];
            return new SolrCloudShard(
                IsActive(state),
                json.Name,
                rangeEnd,
                rangeStart,
                replicas.Properties()
                    .Select(property => BuildReplica(collection, property))
                    .ToDictionary(replica => replica.Name, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if state is active
        /// </summary>
        private static bool IsActive(string state) {
            return "active".Equals(state, StringComparison.OrdinalIgnoreCase);
        }
    }
}