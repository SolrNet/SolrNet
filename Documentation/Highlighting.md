# Highlighting

This feature will "highlight" (typically with markup) the terms that matched the query, including a snippet of the text around the matched term.

Sample code:

```c#
var results = solr.Query(new SolrQueryByField("features", "noise"), new QueryOptions {
    Highlight = new HighlightingParameters {
        Fields = new[] {"features"},
    }
});
foreach (var h in results.Highlights[results[0].Id]) {
    Console.WriteLine("{0}: {1}", h.Key, string.Join(", ", h.Value.ToArray()));
}
```
would print for example:

```
features: <em>Noise</em>Guard, SilentSeek technology, Fluid Dynamic Bearing (FDB) motor
```

Solr reference documentation: http://wiki.apache.org/solr/HighlightingParameters
