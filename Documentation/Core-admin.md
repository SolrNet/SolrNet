# Core admin

SolrNet offers some facilities to execute [Solr core admin commands](https://wiki.apache.org/solr/CoreAdmin).

First, build an instance of `ISolrCoreAdmin`:

```
const string solrUrl = "http://localhost"8983/solr";
var headerParser = ServiceLocator.Current.GetInstance<ISolrHeaderResponseParser>();
var statusParser = ServiceLocator.Current.GetInstance<ISolrStatusResponseParser>();
ISolrCoreAdmin solrCoreAdmin = new SolrCoreAdmin(new SolrConnection(solrUrl), headerParser, statusParser);
```

`ISolrCoreAdmin` has the following core admin commands:

## Status

```
/// Get the status of all registered cores:
IList<CoreResult> coreStatus = solrCoreAdmin.Status();

/// Get the status of a single core:
var coreStatus = solrCoreAdmin.Status("core1");
```

