# Basic usage

First, we have to map the Solr document to a class. Let's use a subset of the default schema that comes with the Solr distribution:

```C#
public class Product {
    [SolrUniqueKey("id")]
    public string Id { get; set; }

    [SolrField("manu_exact")]
    public string Manufacturer { get; set; }

    [SolrField("cat")]
    public ICollection<string> Categories { get; set; }

    [SolrField("price")]
    public decimal Price { get; set; }

    [SolrField("inStock")]
    public bool InStock { get; set; }
}
```

It's just a POCO with some attributes: SolrField maps the attribute to a Solr field and SolrUniqueKey (optional but recommended) maps an attribute to a Solr unique key field.

Now we'll write some tests using this mapped class. Let's initialize the library:

```C#
[TestFixtureSetUp]
public void FixtureSetup() {
    Startup.Init<Product>("http://localhost:8983/solr");
}
```

Let's add a document (make sure you have a running Solr instance before running this test):

```C#
[Test]
public void Add() {
    var p = new Product {
        Id = "SP2514N",
        Manufacturer = "Samsung Electronics Co. Ltd.",
        Categories = new[] {
            "electronics",
            "hard drive",
        },
        Price = 92,
        InStock = true,
    };

    var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
    solr.Add(p);
    solr.Commit();
}
```

Let's see if the document is where we left it:

```C#
[Test]
public void Query() {
    var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
    var results = solr.Query(new SolrQueryByField("id", "SP2514N"));
    Assert.AreEqual(1, results.Count);
    Console.WriteLine(results[0].Price);
}
```
