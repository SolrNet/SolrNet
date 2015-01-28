# More-like-this

This feature returns a list of similar documents for each document returned in the results of the original query.

Parameters are defined through the `MoreLikeThis` property of the `QueryOptions`.

Example: searching for "apache", for each document in the result search for similar documents in the "cat" (category) and "manu" (manufacturer) fields:

```C#
ISolrBasicOperations<Product> solr = ...
var results = solr.Query(new SolrQuery("apache"), new QueryOptions {
    MoreLikeThis = new MoreLikeThisParameters(new[] {"cat", "manu"}) {
        MinDocFreq = 1, // minimum document frequency
        MinTermFreq = 1, // minimum term frequency
    },
});
foreach (var r in results.SimilarResults) {
    Console.WriteLine("Similar documents to {0}", r.Key.Id);
    foreach (var similar in r.Value)
        Console.WriteLine(similar.Id);
    Console.WriteLine();
}
```

All parameters defined in the [Solr docs](http://wiki.apache.org/solr/MoreLikeThis) are supported.
