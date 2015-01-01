# Field collapsing / grouping

This feature can be used to collapse - or group - documents by the unique values of a specified field. Included in the results are the number of records by document key and by field value.

### For Solr 1.4 / 3.1

This feature is not included with the stock Solr 1.4 or 3.1. You need to apply a [patch](https://issues.apache.org/jira/browse/SOLR-236) and recompile Solr.

Sample code:

```C#
ISolrOperations<Product> solr = ...
var results = solr.Query(new SolrQueryByField("features", "noise"), new QueryOptions {
    Collapse = new CollapseParameters {
        Field = "manu"
    }
});
foreach (KeyValuePair<string, int> collapsedDocument in results.Collapsing.DocResults)
    Console.WriteLine("Collapse count for document '{0}': {1}", collapsedDocument.Key, collapsedDocument.Value);
```

For more details see:

 * http://wiki.apache.org/solr/FieldCollapsing
 * https://issues.apache.org/jira/browse/SOLR-236
 * http://blog.jteam.nl/2009/10/20/result-grouping-field-collapsing-with-solr/

### For Solr 3.3+

Use `Grouping` instead of `Collapse` in `QueryOptions` with SolrNet 0.4.0 alpha 1 or later. 
[Sample code](https://github.com/fleetstar/SolrNet/blob/0fbb902f0ae43320dded6b3739748c3692d66b81/SolrNet.Tests/Integration.Sample/Tests.cs#L413-L431)
