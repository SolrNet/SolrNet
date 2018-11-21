# Basic usage of cloud mode

The usage of solrnet in cloud mode is very similar to basic mode. 

1. If you don't use external zookeeper, you have to start you solr instance with "-DzkRun" flag (default zookeeper port is 9983).
or
1. If you use external zookeeper, you have to start zookeeper server (default external zookeeper port is 2181), after that start solr instance with flag "-DzkHost=127.0.0.1:2181".
2. Initialize the library. You need to use SolrCloudStateProvider to get the last actual cluster state from zookeeper instead of direct solr url. To do that, use the following code:

```C#
[TestFixtureSetUp]
public void FixtureSetup() {
	var zookeeperConnectionString = "127.0.0.1:2181";
	var collectionName = "collection_name";
	Startup.Init<Product>(new SolrCloudStateProvider(zookeeperConnectionString), collectionName);
}
```

3. Perform any operation you want in old unchanged style:

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

    var solrCloudMode = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
    solrCloudMode.Add(p);
    solrCloudMode.Commit();
}
```

```C#
[Test]
public void Query() {
    var solrCloudMode = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
    var results = solrCloudMode.Query(new SolrQueryByField("id", "SP2514N"));
    Assert.AreEqual(1, results.Count);
    Console.WriteLine(results[0].Price);
}
```
