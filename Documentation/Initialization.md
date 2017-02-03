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

### Simple Injector
(SolrNet 0.4.0)

```C#
var container = new Container();

var config = new SolrNetConfiguration(_Container);
config.ConfigureContainer();
config.RegisterCore<Entity>("http://localhost:8893/solr/entity");
config.RegisterCore<Entity2>("http://localhost:8893/solr/entity");

var operation = container.GetInstance<ISolrOperations<Entity>>();
```

You can also make it easier by implementing a connection builder:

```C#
class EntityConnectionBuilder : IConnectionBuilder
{
    private string host = "http://localhost:8920/solr/";

    public string Build(Type entityType)
    {
        return host + entityType.Name.ToLower() + "s";
    }

    public string Build<TEntity>()
    {
        return Build(typeof(TEntity));
    }
}

var container = new Container();

var config = new SolrNetConfiguration(_Container);
config.ConfigureContainer();
config.RegisterCores<EntityConnectionBuilder>();

var operation = container.GetInstance<ISolrOperations<Entity>>();
var operation = container.GetInstance<ISolrOperations<Entity2>>();
```

### Multi-core mapping
If you need to map multiple Solr cores/instances, see [this page](Multi-core-instance.md).
