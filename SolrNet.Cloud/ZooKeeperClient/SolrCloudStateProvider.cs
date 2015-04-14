﻿using System;
using System.Collections.Generic;
using System.Text;
using ZooKeeperNet;

namespace SolrNet.Cloud.ZooKeeperClient
{
    public class SolrCloudStateProvider : ISolrCloudStateProvider, IWatcher
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
        private readonly object syncLock;

        /// <summary>
        /// ZooKeeper client instance
        /// </summary>
        private IZooKeeper zooKeeper;

        /// <summary>
        /// ZooKeeper connection string
        /// </summary>
        private readonly string zooKeeperConnection;

        /// <summary>
        /// Constuctor
        /// </summary>
        public SolrCloudStateProvider(string zooKeeperConnection)
        {
            if (string.IsNullOrEmpty(zooKeeperConnection))
                throw new ArgumentNullException("zooKeeperConnection");

            this.zooKeeperConnection = zooKeeperConnection;
            syncLock = new object();
            Key = zooKeeperConnection;
        }

        public void Init()
        {
            lock (syncLock)
                if (!isInitialized)
                {
                    Update();
                    isInitialized = true;
                }
        }

        public SolrCloudState GetCloudState()
        {
            return state;
        }

        public void Dispose()
        {
            lock (syncLock)
                if (!isDisposed)
                {
                    zooKeeper.Dispose();
                    isDisposed = true;
                }
        }

        void IWatcher.Process(WatchedEvent @event)
        {
            if (@event.Type == EventType.NodeDataChanged)
                lock (syncLock)
                    try
                    {
                        Update();
                    } catch (Exception ex) {
                        // log exceptions here
                    }
            else if (@event.Type == EventType.None && @event.State == KeeperState.Disconnected)
            {
                lock (syncLock)
                    try
                    {
                        zooKeeper = null;
                        Update();
                    } catch (Exception ex) {
                        // log exceptions here
                    }
            }
        }

        /// <summary>
        /// Updates zookeeper connection and actual cloud state
        /// </summary>
        private void Update()
        {
            if (zooKeeper == null)
                zooKeeper = new ZooKeeper(zooKeeperConnection, TimeSpan.FromSeconds(10), this);

            state = GetInternalCollectionsState().Merge(GetExternalCollectionsState());
        }

        /// <summary>
        /// Returns parsed internal collections cloud state
        /// </summary>
        private SolrCloudState GetInternalCollectionsState()
        {
            byte[] data;

            try
            {
                data = zooKeeper.GetData(ClusterState, true, null);
            }
            catch (KeeperException ex)
            {
                return new SolrCloudState(new Dictionary<string, SolrCloudCollection>());
            }

            var collectionsState =
                data != null
                ? SolrCloudStateParser.Parse(Encoding.Default.GetString(data))
                : new SolrCloudState(new Dictionary<string, SolrCloudCollection>());
            return collectionsState;
        }

        /// <summary>
        /// Returns parsed external collections cloud state
        /// </summary>
        private SolrCloudState GetExternalCollectionsState()
        {
            var resultState = new SolrCloudState(new Dictionary<string, SolrCloudCollection>());
            List<string> children;

            try
            {
                children = zooKeeper.GetChildren(CollectionsZkNode, true);
            }
            catch (KeeperException ex)
            {
                return resultState;
            }

            if (children == null || children.IsEmpty())
                return resultState;

            foreach (var child in children)
            {
                byte[] data;

                try
                {
                    data = zooKeeper.GetData(GetCollectionPath(child), true, null);
                }
                catch (KeeperException ex)
                {
                    data = null;
                }

                var collectionState = 
                    data != null 
                    ? SolrCloudStateParser.Parse(Encoding.Default.GetString(data)) 
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