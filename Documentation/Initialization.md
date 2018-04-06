# Initialization

Once you have created your document class, you have to initialize the library in order to operate against the Solr instance. This is usually done once at application startup:

```C#
Startup.Init<Product>("http://localhost:8983/solr");
```

Then you ask the service locator for the SolrNet service instance which allows you to issue any supported operation:

```C#
var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
await solr.DeleteAsync(SolrQuery.All);
await solr.AddAsync(p);
await solr.CommitAsync();
var products = await solr.QueryAsync(new SolrQueryByRange<decimal>("price", 10m, 100m));
```

### Microsoft.Extensions.DependencyInjection (ASP.NET core)

``` PowerShell
Install-Package SolrNet.Microsoft.DependencyInjection
```
![https://www.nuget.org/packages/SolrNet.Microsoft.DependencyInjection/](https://img.shields.io/nuget/v/SolrNet.Microsoft.DependencyInjection.svg)

To use, with an `IServiceCollection` instance and one or more assemblies:

``` C#
services.AddSolrNet("http://localhost:8983/solr");
```


### Castle Windsor

``` PowerShell
Install-Package SolrNet.Windsor
```
![https://www.nuget.org/packages/SolrNet.Windsor/](https://img.shields.io/nuget/v/SolrNet.Windsor.svg)


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

``` PowerShell
Install-Package SolrNet.Ninject
```
![https://www.nuget.org/packages/SolrNet.Ninject/](https://img.shields.io/nuget/v/SolrNet.Ninject.svg)

```C#
kernel.Load(new SolrNetModule("http://localhost:8983/solr"));
```

### StructureMap
``` PowerShell
Install-Package SolrNet.StructureMap
```
![https://www.nuget.org/packages/SolrNet.StructureMap/](https://img.shields.io/nuget/v/SolrNet.StructureMap.svg)


If you are using StructureMap, you can use the StructureMap module (`StructureMap.SolrNetIntegration`):

```C#
IEnumerable<SolrServer> solrServers = new[]
{
	new SolrServer(id: "test", url: "http://localhost:8893", documentType: "testDocumentType")
};

var container = new StructureMap.Container(SolrNetRegistry.Create(solrServers));
```

### Autofac
``` PowerShell
Install-Package SolrNet.Autofac
```
![https://www.nuget.org/packages/SolrNet.Autofac/](https://img.shields.io/nuget/v/SolrNet.Autofac.svg)


```C#
var builder = new ContainerBuilder();
builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
var container = builder.Build();
```

### Unity
``` PowerShell
Install-Package SolrNet.Autofac
```
![https://www.nuget.org/packages/SolrNet.Autofac/](https://img.shields.io/nuget/v/SolrNet.Autofac.svg)


```C#
var solrServers = new SolrServers {
        new SolrServerElement {
          Id = "test",
          Url = "http://localhost:8893",
          DocumentType = typeof (Entity).AssemblyQualifiedName,
        }
};
container = new UnityContainer();
new SolrNetContainerConfiguration().ConfigureContainer(solrServers, container);
```

### Multi-core mapping
If you need to map multiple Solr cores/instances, see [this page](Multi-core-instance.md).
