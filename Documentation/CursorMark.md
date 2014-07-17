# CursorMark

SolrNet supports [effecient deep pagination via cursormark from Solr 4.7+](https://cwiki.apache.org/confluence/display/solr/Pagination+of+Results).

## Pagingnation using cursormark
Pagination with the cursormark requires a very specific query setup.
 1. ```Start``` in QueryOptions must have the value of 0. ```Start = 0```
 1. A Sortorder must be set and at least one field **must** be a **unique** field in the Solr index.
 1. The sortorder **cannot** change when using the cursormark. The order is encoded into the cursormark. Any sortorder change will cause Solr to fail the query.

Example: Make an initial Solr query using Cursormark for pagination

```c#
ISolrOperations<Document> solr = ...
var searchResults = solr.Query(SolrQuery.All, new QueryOptions {
    Rows = 10,
    Start = 0, // Must be zero when using CursorMark
    OrderBy = new Collection<SortOrder>
                {
                    new SortOrder("uniqueField", Order.DESC), // Must sort on a unique field
                    new SortOrder("anyOtherField", Order.ASC) // Optional add any other fields for sorting
                },
    CursorMark = CursorMark.Start //Sets init state of the cursormark
    }
});


//Make a new request while paging using the recieved cursormark from previous request

var pagedResults = solr.Query(SolrQuery.All, new QueryOptions {
    Rows = 100,
    Start = 0, // Must be zero when using CursorMark
    OrderBy = new Collection<SortOrder>
                {
                    new SortOrder("uniqueField", Order.DESC), // Must sort on a unique field
                    new SortOrder("anyOtherField", Order.ASC) // Optional add any other fields for sorting
                },
    CursorMark = searchResults.NextCursorMark //Sets the cursormark
    }
});

```
