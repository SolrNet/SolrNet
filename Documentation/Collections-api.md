# Collections admin API

SolrNet has functionality to execute some [Solr collections API commands] (https://cwiki.apache.org/confluence/display/solr/Collections+API). See the Solr wiki for detailed explanations of these commands.

First, build an instance of `ISolrCollectionsAdmin`:

```
const string solrUrl = "http://localhost:8983/solr";
var headerParser = ServiceLocator.Current.GetInstance<ISolrHeaderResponseParser>();
ISolrCollectionsAdmin solrCollectionsAdmin = new SolrCollectionsAdmin(new SolrConnection(solrUrl), headerParser);
```

`ISolrCollectionsAdmin` can execute the following collections admin API commands:

## Create collection

```
var colletionName = "collection_name";
var confName = "conf_name";
var numShards = 1;

/// Create collection with explicit router:
var response = solrCollectionsAdmin.CreateCollection(collectionName, configName: confName, numShards: numShards);

/// Create collection with implicit router with two shards (shard1, shard2):
var response = solrCollectionsAdmin.CreateCollection(collectionName, configName: confName, routerName: "implicit", shards: "shard1, shard2", maxShardsPerNode:10);
```

## Delete collection

```
var colletionName = "collection_name";
var response = solrCollectionsAdmin.DeleteCollection(collectionName);
```

## Create shard

```
/// Note, that collection should have 'implicit' router to perform CreateShard command without any errors
var colletionName = "collection_name";
var shardName = "shard_name";
var response = solrCollectionsAdmin.CreateShard(collectionName, shardName);
```

## Delete shard

```
/// Note, that collection should have 'implicit' router to perform DeleteShard command without any errors
var colletionName = "collection_name";
var shardName = "shard_name";
var response = solrCollectionsAdmin.DeleteShard(collectionName, shardName);
```

## Reload collection

```
var colletionName = "collection_name";
var response = solrCollectionsAdmin.ReloadCollection(collectionName);
```

## Get cluster status

```
var colletionName = "collection_name";
var clusterStatus = solrCollectionsAdmin.GetClusterStatus(collectionName);
```

## List collections

```
var collectionsList = solrCollectionsAdmin.ListCollections();
```
