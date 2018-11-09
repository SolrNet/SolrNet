# CursorMark

SolrNet supports [efficient deep pagination via cursormark from Solr 4.7+](https://cwiki.apache.org/confluence/display/solr/Pagination+of+Results#PaginationofResults-FetchingALargeNumberofSortedResults:Cursors).

Please make sure to read the documentation about the constraints when using cursor pagination.

Example: Make an initial Solr query using cursor pagination:

```c#
ISolrOperations<Document> solr = ...
var searchResults = solr.Query(SolrQuery.All, new QueryOptions {
    Rows = 10,
    StartOrCursor = StartOrCursor.Cursor.Start, // Sets initial state of the cursormark
    OrderBy = new[] {
        new SortOrder("uniqueField", Order.DESC), // Must sort on a unique field
        new SortOrder("anyOtherField", Order.ASC), // Optionally add any other fields for sorting
    }
});


// Make a new request while paging using the received cursormark from previous request

var pagedResults = solr.Query(SolrQuery.All, new QueryOptions {
    Rows = 100,
    StartOrCursor = searchResults.NextCursorMark,
    OrderBy = new[] {
        new SortOrder("uniqueField", Order.DESC), // Must sort on a unique field
        new SortOrder("anyOtherField", Order.ASC), // Optionally add any other fields for sorting
    }
});

```
