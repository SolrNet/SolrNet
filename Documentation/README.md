## SolrNet is an [Apache Solr](http://lucene.apache.org/solr/) client for .NET

SolrNet does not attempt to abstract much over Solr, it's assumed that you know what Solr is and how to use it, just as you need to know relational databases before using an ORM.

If you're not familiar with Solr, take your time to follow the [Solr tutorial](http://lucene.apache.org/solr/tutorial.html), see the [FAQ](http://wiki.apache.org/solr/FAQ) and the [docs](http://wiki.apache.org/solr/FrontPage ). Consider getting a [book](http://lucene.apache.org/solr/books.html).

<!-- This page documents SolrNet features in the master branch. For version-specific documentation, see the Documentation directory on the corresponding version branch. For example https://github.com/mausch/SolrNet/blob/0.4.x/Documentation/README.md -->

### Downloads

It's currently recommended to get the latest binaries directly from the build server. [![Build status](https://ci.appveyor.com/api/projects/status/0oj6vqpnoyw08jtq?svg=true)](https://ci.appveyor.com/project/XavierMorera/solrnet-crl26). <!--The build server also has a NuGet feed with these nightly builds: https://ci.appveyor.com/nuget/solrnet-022x5w7kmuba -->

Otherwise, NuGet packages at nuget.org (currently outdated due to lack of documentation) are available:

 * [SolrNet](https://www.nuget.org/packages/SolrNet/) (core library)
 * [SolrNet.Windsor](https://www.nuget.org/packages/SolrNet.Windsor/)
 * [SolrNet.StructureMap](https://www.nuget.org/packages/SolrNet.StructureMap/)
 * [SolrNet.Ninject](https://www.nuget.org/packages/SolrNet.Ninject/)
 * [SolrNet.Unity](https://www.nuget.org/packages/SolrNet.Unity/)
 * [SolrNet.Autofac](https://www.nuget.org/packages/SolrNet.Autofac/)
 * [SolrNet.NHibernate](https://www.nuget.org/packages/SolrNet.NHibernate/)

### Documentation index

 * [Overview and basic usage](Basic-usage.md)
 * [Basic cloud usage](Basic-usage-cloud.md)
 * [Mapping](Mapping.md)
 * [Initialization](Initialization.md)
 * [Create/Update/Delete](CRUD.md)
 * [Querying](Querying.md)
 * [Faceting](Facets.md)
 * [Highlighting](Highlighting.md)
 * [More like this](More-like-this.md)
 * [Spell checking](Spell-checking.md)
 * [Stats](Stats.md)
 * [Field collapsing / grouping](Field-collapsing.md)
 * [Core admin](Core-admin.md)
 * [Fluent API](Fluent-API.md)
 * [Overriding the default mapper](Overriding-mapper.md)
 * [NHibernate integration](NHibernate-integration.md)
 * [Accessing multiple Solr cores / instances](Multi-core-instance.md)
 * [Mapping validation](Schema-Mapping-validation.md)
 * [Binary document upload](Extract.md)
 * [Sample web application](Sample-application.md)
 * [FAQ](FAQ.md)
 * [Websites, products and companies using SolrNet](Powered-by-SolrNet.md)

### Mailing list

If you have any questions about SolrNet, join the [SolrNet google group](http://groups.google.com/group/solrnet) and ask away.  
If you have questions about Solr itself (i.e. not specifically about SolrNet) use the [Solr mailing list](http://lucene.apache.org/solr/discussion.html).

### Contributing

See [Contributing](../contributing.md)

### Release notes

See [Change Log](../changelog.md)


You can also contribute by donating a few bucks:

[![Click here to lend your support to: SolrNet and make a donation at www.pledgie.com !](http://www.pledgie.com/campaigns/11245.png?skin_name=chrome)](http://www.pledgie.com/campaigns/11245)

