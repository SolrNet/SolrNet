# Faceting

SolrNet supports [faceted searching](http://wiki.apache.org/solr/SolrFacetingOverview).

There are basically three kinds of facet queries:

 1. querying by field
 1. date facets
 1. arbitrary facet queries

Facet queries are issued through the `FacetQueries` property of `QueryOptions`. Then the `QueryOptions` instance is passed to the server instance.

### Querying by field
Querying by field is handled by the `SolrFacetFieldQuery` class. Results are available through the `FacetFields` property.

Example: print all categories sorted by popularity.

```c#
ISolrOperations<Document> solr = ...
var r = solr.Query(SolrQuery.All, new QueryOptions {
    Rows = 0,
    Facet = new FacetParameters {
        Queries = new[] {new SolrFacetFieldQuery("category")}
    }
});
foreach (var facet in r.FacetFields["category"]) {
  Console.WriteLine("{0}: {1}", facet.Key, facet.Value);
}
```

### Date facets
Date facet queries create facets from date ranges. Sample code:

```C#
ISolrOperations<Product> solr = ...
ISolrQueryResults<Product> results = solr.Query(SolrQuery.All, new QueryOptions {
    Facet = new FacetParameters {
        Queries = new[] {
            new SolrFacetDateQuery("timestamp", new DateTime(2001, 1, 1).AddDays(-1) /* range start */, new DateTime(2001, 1, 1).AddMonths(1) /* range end */, "+1DAY" /* gap */) {
                HardEnd = true,
                Other = new[] {FacetDateOther.After, FacetDateOther.Before}
            },
        }
    }
});
DateFacetingResult dateFacetResult = results.FacetDates["timestamp"];
foreach (KeyValuePair<DateTime, int> dr in dateFacetResult.DateResults) {
    Console.WriteLine(dr.Key);
    Console.WriteLine(dr.Value);
}
```

### Arbitrary facet queries
Arbitrary facet queries are handled by the `SolrFacetQuery` class. Results are available through the `FacetQueries` property.

Example: segregate items by price (less than $500 - more than $500)

```C#
ISolrOperations<Document> solr = ...
var lessThan500 = new SolrQueryByRange<decimal>("price", 0m, 500m);
var moreThan500 = new SolrQueryByRange<string>("price", "500", "*");
var r = solr.Query(SolrQuery.All, new QueryOptions {
    Rows = 0,
    Facet = new FacetParameters {
        Queries = new[] {new SolrFacetQuery(lessThan500), new SolrFacetQuery(moreThan500)}
    }
});
foreach (var facet in r.FacetQueries) {
  Console.WriteLine("{0}: {1}", facet.Key, facet.Value);
}
```

### Pivot faceting
http://wiki.apache.org/solr/HierarchicalFaceting#Pivot_Facets

http://wiki.apache.org/solr/SimpleFacetParameters#Pivot_.28ie_Decision_Tree.29_Faceting

(to be documented)
