# Querying

### Simple query

This is the simplest 'query object' in SolrNet. Whatever you give it is passed straight to [Solr's q parameter](http://wiki.apache.org/solr/SearchHandler#q)

```
ISolrOperations<Product> solr = ...
var products1 = solr.Query(new SolrQuery("lucene")); // search for "lucene" in the default field
var products2 = solr.Query(new SolrQuery("name:solr")); // search for "solr" in the "name" field
```

### Query by field
This allows you to define field name and value separately:

```
ISolrOperations<Product> solr = ...
var products = solr.Query(new SolrQueryByField("name", "solr")); // search for "solr" in the "name" field
```

It also has the benefit that it handles special character escaping for you.

(SolrNet 0.4.0) You can disable character escaping by setting `Quoted = false`:

```
var q = new SolrQueryByField("name", "John*") { Quoted = false };
```

### Query by range
Creates a [range query](https://cwiki.apache.org/confluence/display/solr/The+Standard+Query+Parser#TheStandardQueryParser-RangeSearches):

```
ISolrOperations<Product> solr = ...
var products = solr.Query(new SolrQueryByRange<decimal>("price", 100m, 250.50m)); // search for price between 100 and 250.50
```

### Query by list of values
```
var q = new SolrQueryInList("name", "solr", "samsung", "maxtor");
```
is the same as `name:solr OR name:samsung OR name:maxtor`

### "Any value" query
It's often convenient to see what documents have a field defined or not:

```
var q = new SolrHasValueQuery("name");
```

is equivalent to the Solr query `name:[* TO *]`

### Query by distance
(SolrNet 0.4.0)

Creates a `geofilt` or `bbox` filter on a LatLonType field.

Examples:

```
// default accuracy is CalculationAccuracy.Radius (higher accuracy)
var q = new SolrQueryByDistance("store", pointLatitude = 45.15, pointLongitude = -93.85, distance = 5);
```

```
var q = new SolrQueryByDistance("store", pointLatitude = 45.15, pointLongitude = -93.85, distance = 5, accuracy = CalculationAccuracy.BoundingBox);
```

See the [Solr wiki](http://wiki.apache.org/solr/SpatialSearch) for more information.

### Query operators
You can use the `&&` and `||` operators to connect queries, with the expected results:

```
var q = new SolrQuery("solr") && new SolrQuery("name:desc");
```

generates the query `solr AND name:desc`

The plus (+) operator is also overloaded. It concatenates the queries and leaves the actual operator to the [default as specified in Solr's configuration](http://wiki.apache.org/solr/SchemaXml#Default_query_parser_operator).

```
var q = new SolrQuery("solr") + new SolrQuery("name:desc");
```

creates the query `solr name:desc`

To negate a query, you can call `Not()` on it or just use the `!` operator:

```
var q = !new SolrQuery("solr");
```

creates the query `-solr`

Finally, the minus (-) operator:

```
var q = new SolrQuery("solr") - new SolrQuery("name:desc"); // solr - name:desc
```

which is equivalent to (and more intuitive than):

```
var q = new SolrQuery("solr") + !new SolrQuery("name:desc"); // solr - name:desc
```

Alternatively, if you have a list of queries you want to aggregate you can use `SolrMultipleCriteriaQuery`. For example:

```
new SolrMultipleCriteriaQuery(new[] {new SolrQuery("1"), new SolrQuery("2")})
```

is the same as:

```
new SolrQuery("1") + new SolrQuery("2")
```

You can also define what operators to use to join these queries, e.g:

```
new SolrMultipleCriteriaQuery(new[] {new SolrQuery("1"), new SolrQuery("2")}, "AND")
```

### Boosting
You can boost particular queries by calling `Boost()`, for example:

```
var q = new SolrQuery("name:desc").Boost(2); // (name:desc)^2
```

See the [Lucene docs](http://lucene.apache.org/core/2_9_4/queryparsersyntax.html#Boosting%20a%20Term) for more information about boosting.

### Filter queries
Filter queries can be used to specify a query that can be used to restrict the super set of documents that can be returned, without influencing score.

```
ISolrOperations<Product> solr = ...
var products = solr.Query(SolrQuery.All, new QueryOptions {
        FilterQueries = new ISolrQuery[] {
                new SolrQueryByField("manu", "apache"),
                new SolrQueryByRange<decimal>("price", 100m, 200m),
        }
});
```

More information in the [Solr wiki](http://wiki.apache.org/solr/CommonQueryParameters#fq).

### Fields
By default Solr returns all stored fields. You can retrieve only selected fields instead:

```
ISolrOperations<Product> solr = ...
var products = solr.Query(SolrQuery.All, new QueryOptions {
        Fields = new[] {"id", "manu"}
});
```

### Sorting
By default Solr returns search results ordered by "score desc". You can sort the results by any field(s):

```
ISolrOperations<Product> solr = ...
var products = solr.Query(SolrQuery.All, new QueryOptions {
        OrderBy = new[] {new SortOrder("manu", Order.DESC), SortOrder.Parse("id asc")}
});
```

You can random sort using `RandomSortOrder`:

```
solr.Query(SolrQuery.All, new QueryOptions {
        OrderBy = new[] {new RandomSortOrder("randomF")},
});
```

where `randomF` is a [random sort field](http://lucene.apache.org/solr/4_4_0/solr-core/org/apache/solr/schema/RandomSortField.html). `RandomSortOrder` has various constructors to generate a random seed (as in the example above) or use a predefined seed.

### Pagination
In Solr you can't retrieve all your documents in single query. However, by default SolrNet will try to retrieve a large amount of documents, trying to mimic the behavior of a RDBMS without a TOP clause. **It's not recommended to rely on this behavior**. Instead, always define pagination parameters, for example:

```
ISolrOperations<Product> solr = ...
solr.Query("somequery", new QueryOptions{
  StartOrCursor = new StartOrCursor.Start(10),
  Rows = 25
});
```

This will fetch at most 25 documents, starting from the 10th document in the total result set.

If you're planning to paginate beyond the first few pages of results, take a look at [CursorMark](CursorMark.md) instead.

### Additional parameters
Solr has lots of features that aren't directly mapped in SolrNet, but you can enable and use most of them with the `ExtraParams` dictionary. Parameters defined in `ExtraParams` are directly passed to the Solr querystring. For example you can [restrict the maximum time allowed for a query](http://wiki.apache.org/solr/CommonQueryParameters#timeAllowed):

```
ISolrOperations<Product> solr = ...
var products = solr.Query(SolrQuery.All, new QueryOptions {
        ExtraParams = new Dictionary<string, string> {
                {"timeAllowed", "100"}
        }
});
```

Or enable DisMax instead of the standard request handler:

```
ISolrOperations<Product> solr = ...
var products = solr.Query(SolrQuery.All, new QueryOptions {
        ExtraParams = new Dictionary<string, string> {
                {"qt", "dismax"}
        }
});
```

### LocalParams
LocalParams provide a way to add certain metadata to a piece of query. It's used among other things to change the default operator type on the fly for a particular query.

In SolrNet, LocalParams are represented by the `LocalParams` class, which is basically a `Dictionary<string, string>`. LocalParams are attached to a query using the "+" operator. Here's an example:

```
solr.Query(new LocalParams {{"type", "dismax"},{"qf", "myfield"}} + new SolrQuery("solr rocks"));
```

This will generate: `q={!type=dismax qf=myfield}solr rocks`

More information about LocalParams in the [Solr wiki](https://cwiki.apache.org/confluence/display/solr/Local+Parameters+in+Queries).

### ExtraParams
ExtraParams provides a way to add extra arbitrary parameters in the request query string. 

Differently from LocalParams which is an IDictionary, ExtraParams is an IEnumerable<KeyValuePair<string, string>> therefore it does not have an issue with repeated keys. A sample scenario could be that you want to apply multiple boost queries (bq). 

Please look at the example below:
```
var extraParams = new List<KeyValuePair<string, string>>();

extraParams.Add(new KeyValuePair<string, string>("bq", "SomeQuery^10"));
extraParams.Add(new KeyValuePair<string, string>("bq", "SomeOtherQuery^10"));

var options new new QueryOptions();
options.ExtraParams = extraParams; //Since my List implements the right interface

solr.Query(myQuery, options)
```


