# Stats

Property | Description
--------|------------------
Min	| The minimum value
Max	| The maximum value
Sum	| Sum of all values
Count	| How many (non-null) values
Missing	| How many null values
SumOfSquares	| Sum of all values squared (useful for stddev)
Mean	| The average (v1+v2...+vN)/N
StdDev	| Standard Deviation -- measuring how widely spread the values in a data set are.

Sample usage:

```C#
ISolrOperations<Product> solr = ...
var results = solr.Query(SolrQuery.All, new QueryOptions {
    Rows = 0,
    Stats = new StatsParameters {
        Facets = new[] { "inStock" },
        FieldsWithFacets = new Dictionary<string, ICollection<string>> {
            {"popularity", new List<string> {"price"}}
        }
    }
});

foreach (var kv in results.Stats) {
    Console.WriteLine("Field {0}: ", kv.Key);
    var s = kv.Value;
    Console.WriteLine("Min: {0}", s.Min);
    Console.WriteLine("Max: {0}", s.Max);
    Console.WriteLine("Sum of squares: {0}", s.SumOfSquares);
    foreach (var f in s.FacetResults) {
        Console.WriteLine("Facet: {0}", f.Key);
        foreach (var fv in f.Value) {
            Console.WriteLine("Facet value: {0}", fv.Key);
            Console.WriteLine("Min: {0}", fv.Value.Min);
            Console.WriteLine("Max: {0}", fv.Value.Max);
            Console.WriteLine("Sum of squares: {0}", fv.Value.SumOfSquares);
        }
    }
}
```

For more details see the [Solr wiki](http://wiki.apache.org/solr/StatsComponent).
