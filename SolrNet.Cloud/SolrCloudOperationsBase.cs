using System;
using System.Collections.Generic;
using System.Linq;

namespace SolrNet.Cloud {
    /// <summary>
    /// Solr cloud operations base
    /// </summary>
    public abstract class SolrCloudOperationsBase<T> {
        /// <summary>
        /// Is post connection
        /// </summary>
        private readonly bool isPostConnection;

        /// <summary>
        /// Collection name
        /// </summary>
        private readonly string collectionName;

        /// <summary>
        /// Cloud state provider
        /// </summary>
        private readonly ISolrCloudStateProvider cloudStateProvider;

        /// <summary>
        /// Operations provider
        /// </summary>
        private readonly ISolrOperationsProvider operationsProvider;

        /// <summary>
        /// Random instance
        /// </summary>
        private readonly Random random;

        /// <summary>
        /// Constructor
        /// </summary>
        protected SolrCloudOperationsBase(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, bool isPostConnection) {
            this.cloudStateProvider = cloudStateProvider;
            this.operationsProvider = operationsProvider;
            this.isPostConnection = isPostConnection;
            random = new Random();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected SolrCloudOperationsBase(ISolrCloudStateProvider cloudStateProvider, ISolrOperationsProvider operationsProvider, bool isPostConnection, string collectionName = null)
            : this(cloudStateProvider, operationsProvider, isPostConnection)
        {
            this.collectionName = collectionName;
        }

        /// <summary>
        /// Performs basic operation
        /// </summary>
        protected TResult PerformBasicOperation<TResult>(Func<ISolrBasicOperations<T>, TResult> operation, bool leader = false)
        {
            var replicas = SelectReplicas(leader);
            var operations = operationsProvider.GetBasicOperations<T>(
                replicas[random.Next(replicas.Count)].Url,
                isPostConnection);
            if (operations == null)
                throw new ApplicationException("Operations provider returned null.");
            return operation(operations);
        }

        /// <summary>
        /// Perform operation
        /// </summary>
        protected TResult PerformOperation<TResult>(Func<ISolrOperations<T>, TResult> operation, bool leader = false) {
            var replicas = SelectReplicas(leader);
            var operations = operationsProvider.GetOperations<T>(
                replicas[random.Next(replicas.Count)].Url,
                isPostConnection);
            if (operations == null)
                throw new ApplicationException("Operations provider returned null.");
            return operation(operations);
        }

        /// <summary>
        /// Returns collection of replicas
        /// </summary>
        private IList<SolrCloudReplica> SelectReplicas(bool leaders) {
            var state = cloudStateProvider.GetCloudState();
            if (state == null || state.Collections == null || state.Collections.Count == 0)
            {
                throw new ApplicationException("Didn't get any collection's state from zookeeper.");
            }
            if (collectionName != null && !state.Collections.ContainsKey(collectionName))
            {
                throw new ApplicationException(string.Format("Didn't get '{0}' collection state from zookeeper.", collectionName));
            }
            var collection = collectionName == null
                ? state.Collections.Values.First()
                : state.Collections[collectionName];
            var replicas = collection.Shards.Values
                .Where(shard => shard.IsActive)
                .SelectMany(shard => shard.Replicas.Values)
                .Where(replica => replica.IsActive && (!leaders || replica.IsLeader))
                .ToList();
            if (replicas.Count == 0)
            {
                throw new ApplicationException("No appropriate node was selected to perform the operation.");
            }
            return replicas;
        }
    }
}
