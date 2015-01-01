# Initialization

Once you have created your document class, you have to initialize the library in order to operate against the Solr instance. This is usually done once at application startup:

```C#
Startup.Init<Product>("http://localhost:8983/solr");
```

Then you ask the service locator for the SolrNet service instance which allows you to issue any supported operation:

```C#
var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
solr.Delete(SolrQuery.All);
solr.Add(p);
solr.Commit();
var products = solr.Query(new SolrQueryByRange<decimal>("price", 10m, 100m));
```

### Castle Windsor
Alternatively, if your app uses Castle Windsor you can set up SolrNet using the included facility:

```C#
container.AddFacility("solr", new SolrNetFacility("http://localhost:8983/solr"));
```

Or using Windsor's xml config:

```
<castle> 
    <facilities> 
        <facility id="solr" type="Castle.Facilities.SolrNetIntegration.SolrNetFacility, SolrNet">
            <solrURL>http://localhost:8983/solr</solrURL> 
        </facility> 
    </facilities> 
</castle>
```

### Ninject
If you're using Ninject, you can use the Ninject module:

```C#
kernel.Load(new SolrNetModule("http://localhost:8983/solr"));
```

### StructureMap
If you are using StructureMap, you can use the StructureMap module (`StructureMap.SolrNetIntegration`):

```C#
ObjectFactory.Initialize(
    x => x.AddRegistry(
        new SolrNetRegistry("http://localhost:8893/solr")
    )
);
```

### Autofac
(SolrNet 0.4.0)

```C#
var builder = new ContainerBuilder();
builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
var container = builder.Build();
```

### Unity
(SolrNet 0.4.0)

```C#
var solrServers = new SolrServers {
        new SolrServerElement {
          Id = "test",
          Url = "htp://localhost:8893",
          DocumentType = typeof (Entity).AssemblyQualifiedName,
        }
};
container = new UnityContainer();
new SolrNetContainerConfiguration().ConfigureContainer(solrServers, container);
```

### Multi-core mapping
If you need to map multiple Solr cores/instances, see [this page](Multi-core-instance.md).
