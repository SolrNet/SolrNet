# Atomic updates
Atomic updates can be used update only specific fields in document. This can help speed up indexing in certain use cases. With SolrNet, atomic updates can be done using `AtomicUpdate()` methods.

Each individual update is defined by the `AtomicUpdateSpec`, which has below fields
- `field` - Name of field
- `type` - Type of operation to be done
- `value` - Value to be updated

`AtomicUpdate()` method requests update on a single document, which can be identified by the first parameter to the method.

### Example

Consider a sample document of type `Product` as below,

```C#
public class Product {
    [SolrUniqueKey("id")]
    public string Id { get; set; }

    [SolrField("cat")]
    public ICollection<string> Categories { get; set; }

    [SolrField("sold")]
    public int SoldCount { get; set; }

    [SolrField("inStock")]
    public int InStock { get; set; }
}
```

Below code can be used to update a document with id `mydoc` by adding a new category `toys` and to increment the `SoldCount` by 14 and decrement `InStock` count by 6.

```
ISolrOperations<Product> solr = ...
solr.AtomicUpdate("mydoc",
  new[]
  {
      new AtomicUpdateSpec("cat", AtomicUpdateType.Add, "toys"),
      new AtomicUpdateSpec("sold", AtomicUpdateType.Inc, 14),
      new AtomicUpdateSpec("inStock", AtomicUpdateType.Inc, -6),
  });
```

### Reference

See the [Solr wiki](https://solr.apache.org/guide/solr/latest/indexing-guide/partial-document-updates.html#atomic-updates) for more information.
