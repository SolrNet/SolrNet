# Core admin

SolrNet offers some facilities to execute [Solr core admin commands](https://wiki.apache.org/solr/CoreAdmin). See the Solr wiki for detailed explanations of these commands.

First, build an instance of `ISolrCoreAdmin`:

```
const string solrUrl = "http://localhost:8983/solr";
var headerParser = ServiceLocator.Current.GetInstance<ISolrHeaderResponseParser>();
var statusParser = ServiceLocator.Current.GetInstance<ISolrStatusResponseParser>();
ISolrCoreAdmin solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl), headerParser, statusParser);
```

`ISolrCoreAdmin` can execute the following core admin commands:

## Status

```
/// Get the status of all registered cores:
IList<CoreResult> coreStatus = solrCoreAdmin.Status();

/// Get the status of a single core:
var coreStatus = solrCoreAdmin.Status("core1");
```

## Create

```
solrCoreAdmin.Create(coreName: "items", instanceDir: "items");
```

## Reload

```
solrCoreAdmin.Reload("core1");
```

## Rename

```
solrCoreAdmin.Rename("core1", "newCoreName");
```

## Swap

```
solrCoreAdmin.Swap("core0", "core1");
```

## Unload

```
solrCoreAdmin.Unload("core0", UnloadCommand.Delete.Data);
```

## Merge

```
solrCoreAdmin.Merge("destinationCore", new MergeCommand.SrcCore("sourceCore0"), new MergeCommand.SrcCore("sourceCore1"));
```

## Alias

```
solrCoreAdmin.Alias("existingCore", "alias");
```
