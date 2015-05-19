# Highlighting

This feature will "highlight" (typically with markup) the terms that matched the query, including a snippet of the text around the matched term. 

To enable highlighting please include a HighlightingParameters QueryOptions object where you include which fields you want to apply highlighting to.

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

If you need to specify additional parameters, for example the snippet size for a specific field, it is necessary to do so using ExtraParams which then are added to the QueryOptions object of your query. 
```
Dictionary<string, string> extraParams = new Dictionary<string, string>();
extraParams.Add("f.features.hl.fragsize", "250");
```

The results object will include:
- A Highlights property which is an IDictionary<string, SolrNet.Impl.HighlightedSnippets>. 
- The string corresponds to the document uniquekey
- The HighlightedSnippets is an IDictionary<string, ICollection<string>>. This object indicates which field is being returned, for example "features" and the snppet text "<em>Noise</em>Guard, SilentSeek technology, Fluid Dynamic Bearing (FDB) motor" 


The returned tag corresponds to <em> although it can be configured directly in Solr to use a different tag. Also, if the request handler in Solr is configured to include highlights, it is not required to add the query options.

For more details about this feature, see the [Solr wiki](http://wiki.apache.org/solr/HighlightingParameters) and the [reference guide](https://cwiki.apache.org/confluence/display/solr/Highlighting).
