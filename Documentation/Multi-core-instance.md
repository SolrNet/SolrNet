# Multi-core / multi-instance

This page describes how to configure SolrNet to access (read/write) multiple [Solr cores](http://wiki.apache.org/solr/CoreAdmin) or instances. It assumes you know what Solr cores are, how to configure and use them outside of SolrNet. This page doesn't cover [CoreAdminHandler](http://wiki.apache.org/solr/CoreAdmin#CoreAdminHandler) commands.

How to configure SolrNet for multicore depends on how it's [integrated to your app](Initialization.md), and if your cores [map to different types or the same type](Mapping.md).

### With built-in container

The built-in container (`Startup`) is currently limited to access multiple cores/instances with different mapped types. It's quite simple to configure: assuming you have a core that maps to class Product and another core mapping to class `Person`:

```C#
Startup.Init<Product>("http://localhost:8983/solr/product");
Startup.Init<Person>("http://localhost:8983/solr/person");

ISolrOperations<Product> solrProduct = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
solrProduct.Add(new Product { Name = "Kodak EasyShare" });
solrProduct.Commit();

ISolrOperations<Person> solrPerson = ServiceLocator.Current.GetInstance<ISolrOperations<Person>>();
solrPerson.Add(new Person { FirstName = "Joe", LastName = "Example" });
solrPerson.Commit();
```

### With Windsor facility
Code config:

```C#
var solrFacility = new SolrNetFacility("http://localhost:8983/solr/defaultCore");
solrFacility.AddCore("core0-id", typeof(Product), "http://localhost:8983/solr/product");
solrFacility.AddCore("core1-id", typeof(Product), "http://localhost:8983/solr/product2");
solrFacility.AddCore(typeof(Person), "http://localhost:8983/solr/person"); // no need to set an explicit ID since it's the only core for Person
container.AddFacility("solr", solrFacility);

ISolrOperations<Person> solrPerson = container.Resolve<ISolrOperations<Person>>();
ISolrOperations<Product> solrProduct1 = container.Resolve<ISolrOperations<Product>>("core0-id"); // use proper Windsor service overrides instead of resolving like this
ISolrOperations<Product> solrProduct2 = container.Resolve<ISolrOperations<Product>>("core1-id");
```

Equivalent XML config:

```
<facilities>
    <facility id='solr'>
        <solrURL>http://localhost:8983/solr/defaultCore</solrURL>
        <cores>
            <core id='core0-id'>
                <documentType>Namespace.To.Product, MyAssembly</documentType>
                <url>http://localhost:8983/solr/product</url>
            </core>
            <core id='core1-id'>
                <documentType>Namespace.To.Product, MyAssembly</documentType>
                <url>http://localhost:8983/solr/product2</url>
            </core>
            <core>
                <documentType>Namespace.To.Person, MyAssembly</documentType>
                <url>http://localhost:8983/solr/person</url>
            </core>
        </cores>
    </facility>
</facilities>
```

### With StructureMap registry

```C#
var cores = new SolrServers {
        new SolrServerElement {
                Id = "core0-id",
                DocumentType = typeof(Product).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/product",
        },
        new SolrServerElement {
                Id = "core1-id",
                DocumentType = typeof(Person).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/person",
        },
};
ObjectFactory.Initialize(c => c.IncludeRegistry(new SolrNetRegistry(cores)));
var solr1 = ObjectFactory.GetInstance<ISolrOperations<Product>>();
var solr2 = ObjectFactory.GetInstance<ISolrOperations<Person>>();
```

Equivalent XML config (in App.config):

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
        <configSections>
                <section name="solr" type="StructureMap.SolrNetIntegration.Config.SolrConfigurationSection, StructureMap.SolrNetIntegration" />
        </configSections>
        <solr>
                <server id="core0-id" url="http://localhost:8080/solr/product" 
                                documentType="Namespace.To.Product, MyAssembly" />
                <server id="core1-id" url="http://localhost:8080/solr/person"
                                documentType="Namespace.To.Person, MyAssembly" />
        </solr>
</configuration>
```
