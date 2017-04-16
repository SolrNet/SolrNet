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
This is a test branch

### Date facets
Date facet queries create facets from date ranges. Sample code:

```C#
ISolrOperations<Product> solr = ...
var results = solr.Query(SolrQuery.All, new QueryOptions {
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
Pivot faceting allows creating multidimensional facets. You can create a pivot facet with a main category and group by sub-categories. 

In this example I will show you how to create a pivot facet that separates items by inStock but grouping them by in category.

Please look at the example below:
```
//Create a facet Pivot Query
var facetPivotQuery = new SolrFacetPivotQuery()
    {
        //Define 1 pivot, grouping cat by inStock
        //Multiple can be defined as well
        Fields = new[] { new PivotFields("inStock", "cat") },

        //Set the minCount to 1
        MinCount = 1
    };

//Create the facet parameters
//Note that you can use pivotQueries together with other facets queries
var facetParams = new FacetParameters()
{
    Queries = new[] { facetPivotQuery },

    //Limit the amounts of pivotRows to 15
    Limit = 15
};

var queryOptions = new QueryOptions();
queryOptions.Facet = facetParams;
queryOptions.Rows = 0;

var results = solr.Query("*:*", queryOptions);
if (results.FacetPivots.Count > 0)
{
    foreach (var pivotTable in results.FacetPivots)
    {
        System.Diagnostics.Debug.WriteLine("Pivot table for " + pivotTable.Key);
        foreach (var pivot in pivotTable.Value)
        {
            System.Diagnostics.Debug.WriteLine("  Pivot: " + pivot.Field + " with value " + pivot.Value + ". Child Pivots:");
            foreach (var pivotChild in pivot.ChildPivots)
            {
                System.Diagnostics.Debug.WriteLine("    - " + pivotChild.Value + " (" + pivotChild.Count + ")");
            }
        }
    }
}
```

This sample will create two main categories by inStock(true or false) and then broken down by cat (category). It will print out the following:
```
  Pivot: inStock with value true. Child Pivots:
    - electronics (10)
    - memory (3)
    - hard drive (2)
    - monitor (2)
    - search (2)
    - software (2)
    - camera (1)
    - copier (1)
    - multifunction printer (1)
    - music (1)
    - printer (1)
    - scanner (1)
  Pivot: inStock with value false. Child Pivots:
    - electronics (4)
    - connector (2)
    - graphics card (2)
    ```

Additional information to be found in:
http://wiki.apache.org/solr/HierarchicalFaceting#Pivot_Facets

http://wiki.apache.org/solr/SimpleFacetParameters#Pivot_.28ie_Decision_Tree.29_Faceting

