## SolrNet is an [Apache Solr](http://solr.apache.org/) and SolrCloud client for .NET


SolrNet does not attempt to abstract much over Solr, it's assumed that you know what Solr is and how to use it, just as you need to know relational databases before using an ORM.

If you're not familiar with Solr, take your time to follow the [Solr tutorial](https://solr.apache.org/resources.html#tutorials), see the [FAQ](http://wiki.apache.org/solr/FAQ) and the [docs](https://solr.apache.org/resources.html#documentation ). Consider getting a [book](https://solr.apache.org/resources.html#solr-books).

### Downloads

The easiest way to get going is to use our NuGet packages:

#### Solr 

| Package | Description | .NET Framework | .NET Standard |
|:-------:|:-----------:|:--------------:|:-------------:|
|[SolrNet.Core](https://www.nuget.org/packages/SolrNet.Core/) | Core library, best used with one of the DI integration packages | 4.6 | 2.0 |
|[SolrNet](https://www.nuget.org/packages/SolrNet/)| [Lightweight DI - commonservicelocator](https://github.com/unitycontainer/commonservicelocator) | 4.6 | 2.0 |
|[SolrNet.Windsor](https://www.nuget.org/packages/SolrNet.Windsor/)| [Castle Windsor](http://www.castleproject.org/projects/windsor/) integration | 4.6 | 2.0 |
|[SolrNet.Microsoft.DependencyInjection](https://www.nuget.org/packages/SolrNet.Microsoft.DependencyInjection/)|[Microsoft Core Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) | 4.6.1 | 2.0 |
|[SolrNet.StructureMap](https://www.nuget.org/packages/SolrNet.StructureMap/)|[StructureMap](http://structuremap.github.io/) | 4.6 | 2.0 |
|[SolrNet.Ninject](https://www.nuget.org/packages/SolrNet.Ninject/)| [Ninject](http://www.ninject.org/)  | 4.6 | 2.0 |
|[SolrNet.Unity](https://www.nuget.org/packages/SolrNet.Unity/)| [Unity](https://github.com/unitycontainer/unity) | 4.6 | 2.0 |
|[SolrNet.Autofac](https://www.nuget.org/packages/SolrNet.Autofac/)| [Autofac](https://autofac.org/) | 4.6 | 2.0 |
|[SolrNet.SimpleInjector](https://www.nuget.org/packages/SimpleInjector.SolrNet/)| [SimpleInjector](https://simpleinjector.org/) | 4.6 | 2.0 |

#### SolrCloud 

| Package | Description | .NET Framework | .NET Standard |
|:-------:|:-----------:|:--------------:|:-------------:|
|[SolrNet.Cloud.Core](https://www.nuget.org/packages/SolrNet.Cloud.Core/) | Core library, best used with one of the DI integration packages | 4.6 | 2.0 |
|[SolrNet.Cloud](https://www.nuget.org/packages/SolrNet.Cloud/)| [Lightweight DI - commonservicelocator](https://github.com/unitycontainer/commonservicelocator) | 4.6 | 2.0 |
|[SolrNet.Cloud.Unity](https://www.nuget.org/packages/SolrNet.Cloud.Unity/)| [Unity](https://github.com/unitycontainer/unity) | 4.6 | 2.0 |
|[SolrNet.Cloud.Autofac](https://www.nuget.org/packages/SolrNet.Cloud.Autofac/)| [Autofac](https://autofac.org) | 4.6 | 2.0 |


### Documentation index

 * [Overview and basic usage](Documentation/Basic-usage.md)
 * [Basic cloud usage](Documentation/Basic-usage-cloud.md)
 * [Mapping](Documentation/Mapping.md)
 * [Initialization](Documentation/Initialization.md)
 * [Create/Update/Delete](Documentation/CRUD.md)
 * [Querying](Documentation/Querying.md)
 * [Faceting](Documentation/Facets.md)
 * [Highlighting](Documentation/Highlighting.md)
 * [More like this](Documentation/More-like-this.md)
 * [Spell checking](Documentation/Spell-checking.md)
 * [Stats](Documentation/Stats.md)
 * [Field collapsing / grouping](Documentation/Field-collapsing.md)
 * [Core admin](Documentation/Core-admin.md)
 * [Overriding the default mapper](Documentation/Overriding-mapper.md)
 * [Accessing multiple Solr cores / instances](Documentation/Multi-core-instance.md)
 * [Mapping validation](Documentation/Schema-Mapping-validation.md)
 * [Binary document upload](Documentation/Extract.md)
 * [Sample web application](Documentation/Sample-application.md)
 * [FAQ](Documentation/FAQ.md)
 * [Websites, products and companies using SolrNet](Documentation/Powered-by-SolrNet.md)


 ### Contributing

See [Contributing](contributing.md)

### Release notes

See [Change Log](changelog.md)
