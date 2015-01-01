# Fluent API

### Query-building

Some examples:

```C#
Query.Simple("name:solr"); // name:solr
Query.Field("name").Is("solr"); // name:solr
Query.Field("price").From(10).To(20); // price:[10 TO 20]
Query.Field("price").In(10, 20, 30); // price:10 OR price:20 OR price:30
Query.Field("name").HasAnyValue(); // name:[* TO *]
```

These can be used anywhere where a `ISolrQuery` is accepted, e.g.:

```C#
ISolrOperations<Product> solr = ...
solr.Query(Query.Field("name").HasAnyValue());
```

They can also be mixed with boolean operators:

```C#
ISolrOperations<Product> solr = ...
solr.Query(Query.Field("name").HasAnyValue() && Query.Field("price").Is(0));
```

### Querying

This API is deprecated. Please don't use it. If you're using it, be aware that it will be removed in a future release of SolrNet.

Example:

```C#
[SetUp]
public void setup() {
    Solr.Connection = new SolrConnection("http://localhost:8983/solr");
}

[Test]
public void QueryById() {    
    ISolrQueryResults<TestDocument> r = Solr.Query<TestDocument>().By("id").Is("123456").Run();
}

[Test]
public void QueryByRange() {
    ISolrQueryResults<TestDocument> r = Solr.Query<TestDocument>().By("id").Between(123).And(456).OrderBy("id", Order.ASC).Run();
}

[Test]
public void DeleteByQuery() {
    Solr.Delete.ByQuery<TestDocument>("id:123456");
}
```

`Run()` is the explicit kicker method. There are some more examples in the [tests](https://github.com/mausch/SolrNet/blob/master/SolrNet.DSL.Tests/DSLTests.cs).
