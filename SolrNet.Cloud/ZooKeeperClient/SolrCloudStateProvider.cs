using System;
using System.Collections.Generic;
using System.Text;
using org.apache.zookeeper;
using System.Threading.Tasks;
using System.Linq;

namespace SolrNet.Cloud.ZooKeeperClient
{
    public class SolrCloudStateProvider : Watcher, ISolrCloudStateProvider
    {
        /// <summary>
        /// Cluster state path constant
        /// </summary>
        public const string ClusterState = "/clusterstate.json";

        /// <summary>
        /// External collections state file name
        /// </summary>
        public const string CollectionState = "state.json";

        /// <summary>
        /// Collection zookeeper node path
        /// </summary>
        public const string CollectionsZkNode = "/collections";

        /// <summary>
        /// Live nodes zookeeper node path
        /// </summary>
        public const string LiveNodesZkNode = "/live_nodes";

        public string Key { get; private set; }

        /// <summary>
        /// Is disposed
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Is initialized
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// Actual cloud state
        /// </summary>
        private SolrCloudState state;

        /// <summary>
        /// Object for lock
        /// </summary>
        private readonly System.Threading.SemaphoreSlim semaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);

        /// <summary>
        /// ZooKeeper client instance
        /// </summary>
        private ZooKeeper zooKeeper;

        /// <summary>
        /// ZooKeeper connection string
        /// </summary>
        private readonly string zooKeeperConnection;

        private List<string> liveNodes;

        private readonly int zooKeeperTimeoutMs;

        /// <summary>
        /// Constuctor
        /// </summary>
        public SolrCloudStateProvider(string zooKeeperConnection, int zooKeeperTimeoutMs = 10_000)
        {
            if (string.IsNullOrEmpty(zooKeeperConnection))
                throw new ArgumentNullException("zooKeeperConnection");

            this.zooKeeperConnection = zooKeeperConnection;
            this.zooKeeperTimeoutMs = zooKeeperTimeoutMs;
            Key = zooKeeperConnection;
        }

        /// <summary>
        /// Initialize cloud state
        /// </summary>
        public async Task InitAsync()
        {
            if (isInitialized)
            {
                return;
            }

            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!isInitialized)
                {
                    await UpdateAsync().ConfigureAwait(false);

                    isInitialized = true;
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }

        }

        /// <summary>
        /// Get cloud state
        /// </summary>
        /// <returns>Solr Cloud State</returns>
        public SolrCloudState GetCloudState()
        {
            return state;
        }

        /// <summary>
        /// Reinitialize connection and get fresh cloud state.
        /// Not included in ISolrCloudStateProvider interface due to the testing purpose only 
        /// (causes reloading all cloud data and too slow to use in production)
        /// </summary>
        /// <returns>Solr Cloud State</returns>
        public async Task<SolrCloudState> GetFreshCloudStateAsync()
        {
            await SynchronizedUpdateAsync(cleanZookeeperConnection: true).ConfigureAwait(false);
            return GetCloudState();
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            if (!isDisposed)
            {
                if (zooKeeper != null)
                {
                    zooKeeper.closeAsync().ConfigureAwait(false);
                }
                isDisposed = true;
            }

        }

        public async Task DisposeAsync()
        {
            if (!isDisposed && zooKeeper != null)
            {
                await zooKeeper.closeAsync().ConfigureAwait(false);
                isDisposed = true;
            }

        }

        /// <summary>
        /// Watcher for zookeeper events
        /// </summary>
        /// <param name="event">zookeeper event</param>
        public override async Task process(WatchedEvent @event)
        {
            if (@event.get_Type() != Event.EventType.None && !string.IsNullOrEmpty(@event.getPath()))
            {
                await SynchronizedUpdateAsync().ConfigureAwait(false);
            }
            else if (@event.get_Type() == Event.EventType.None && @event.getState() == Event.KeeperState.Disconnected)
            {
                await SynchronizedUpdateAsync(cleanZookeeperConnection: true).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Synchronized updates of zookeeper connection and actual cloud state
        /// </summary>
        /// <param name="cleanZookeeperConnection">clean zookeeper connection and create new one</param>
        private async Task SynchronizedUpdateAsync(bool cleanZookeeperConnection = false)
        {

            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                await UpdateAsync(cleanZookeeperConnection).ConfigureAwait(false);
            }
            finally
            {
                semaphoreSlim.Release();
            }

        }

        /// <summary>
        /// Updates zookeeper connection and actual cloud state
        /// </summary>
        /// <param name="cleanZookeeperConnection">clean zookeeper connection and create new one</param>
        private async Task UpdateAsync(bool cleanZookeeperConnection = false)
        {
            if (zooKeeper == null || cleanZookeeperConnection)
            {
                if (zooKeeper != null)
                {
                    await zooKeeper.closeAsync().ConfigureAwait(false);
                }
                zooKeeper = new ZooKeeper(zooKeeperConnection, zooKeeperTimeoutMs, this);
            }

            liveNodes = await GetLiveNodesAsync().ConfigureAwait(false);

            state = liveNodes.Any()
                ? (await GetInternalCollectionsStateAsync().ConfigureAwait(false)).Merge(await GetExternalCollectionsStateAsync().ConfigureAwait(false))
                : new SolrCloudState(new Dictionary<string, SolrCloudCollection>());
        }

        /// <summary>
        /// Get Live nodes from zookeeper
        /// </summary>
        private async Task<List<string>> GetLiveNodesAsync()
        {
            List<string> result;

            try
            {
                var liveNodesChildren = await zooKeeper.getChildrenAsync($"{LiveNodesZkNode}", this).ConfigureAwait(false);
                result = new List<string>(liveNodesChildren.Children);
            }
            catch (KeeperException)
            {
                result = new List<string>();
            }

            return result;
        }

        /// <summary>
        /// Returns parsed internal collections cloud state
        /// </summary>
        private async Task<SolrCloudState> GetInternalCollectionsStateAsync()
        {
            DataResult data;

            try
            {
                data = await zooKeeper.getDataAsync(ClusterState, true).ConfigureAwait(false);
            }
            catch (KeeperException ex)
            {
                return new SolrCloudState(new Dictionary<string, SolrCloudCollection>());
            }

            var collectionsState =
                data != null
                ? SolrCloudStateParser.Parse(Encoding.Default.GetString(data.Data), liveNodes)
                : new SolrCloudState(new Dictionary<string, SolrCloudCollection>());
            return collectionsState;
        }

        /// <summary>
        /// Returns parsed external collections cloud state
        /// </summary>
        private async Task<SolrCloudState> GetExternalCollectionsStateAsync()
        {
            var resultState = new SolrCloudState(new Dictionary<string, SolrCloudCollection>());
            ChildrenResult children;

            try
            {
                children = await zooKeeper.getChildrenAsync(CollectionsZkNode, true).ConfigureAwait(false);
            }
            catch (KeeperException ex)
            {
                return resultState;
            }

            if (children == null || !children.Children.Any())
                return resultState;

            foreach (var child in children.Children)
            {
                DataResult data;

                try
                {
                    data = await zooKeeper.getDataAsync(GetCollectionPath(child), true).ConfigureAwait(false);
                }
                catch (KeeperException ex)
                {
                    data = null;
                }

                var collectionState =
                    data != null
                    ? SolrCloudStateParser.Parse(Encoding.Default.GetString(data.Data), liveNodes)
                    : new SolrCloudState(new Dictionary<string, SolrCloudCollection>());
                resultState = resultState.Merge(collectionState);
            }

            return resultState;
        }

        /// <summary>
        /// Returns path to collection
        /// </summary>
        private string GetCollectionPath(string collectionName)
        {
            return string.Format("{0}/{1}/{2}", CollectionsZkNode, collectionName, CollectionState);
        }
    }
}
