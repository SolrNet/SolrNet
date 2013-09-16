# Overriding the default mapper

By default SolrNet maps Solr fields using attributes. However you might want to use [another mapper](Mapping.md). Replacing the default mapper depends on how you set up the library:

### Built-in container
If you're using the default built-in container, you can replace it like this before calling `Startup.Init()`:

```C#
var mapper = new MappingManager();
/* Here come your mappings */
var container = new Container(Startup.Container);
container.RemoveAll<IReadOnlyMappingManager>();
container.Register<IReadOnlyMappingManager>(c => mapper);
Startup.Init<Document>("http://localhost:8983/solr");
```

### Windsor facility
If you're using the Windsor facility, you can override the mapper like this:

```C#
var mapper = new MappingManager();
/* Here come your mappings */
var solrFacility = new SolrNetFacility("http://localhost:8983/solr") {Mapper = mapper};
var container = new WindsorContainer();
container.AddFacility("solr", solrFacility);
```

### Ninject module

```C#
var mapper = new MappingManager();
/* Here come your mappings */
var c = new StandardKernel();
c.Load(new SolrNetModule("http://localhost:8983/solr") {Mapper = mapper});
```
