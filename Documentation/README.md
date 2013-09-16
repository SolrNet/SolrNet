## SolrNet is an [Apache Solr](http://lucene.apache.org/solr/) client for .NET

SolrNet does not attempt to abstract much over Solr, it's assumed that you know what Solr is and how to use it, just as you need to know relational databases before using an ORM.

If you're not familiar with Solr, take your time to follow the [Solr tutorial](http://lucene.apache.org/solr/tutorial.html), see the [FAQ](http://wiki.apache.org/solr/FAQ) and the [docs](http://wiki.apache.org/solr/FrontPage ). Consider getting a [book](http://lucene.apache.org/solr/books.html).

### Downloads

It's currently recommended to get the latest binaries directly from the [build server](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt290&tab=buildTypeHistoryList) (see "Artifacts").  
Otherwise, NuGet packages (currently outdated) are available:

 * [SolrNet](https://www.nuget.org/packages/SolrNet/) (core library)
 * [SolrNet.Windsor](https://www.nuget.org/packages/SolrNet.Windsor/)
 * [SolrNet.StructureMap](https://www.nuget.org/packages/SolrNet.StructureMap/)
 * [SolrNet.Ninject](https://www.nuget.org/packages/SolrNet.Ninject/)
 * [SolrNet.Unity](https://www.nuget.org/packages/SolrNet.Unity/)
 * [SolrNet.Autofac](https://www.nuget.org/packages/SolrNet.Autofac/)
 * [SolrNet.NHibernate](https://www.nuget.org/packages/SolrNet.NHibernate/)

### Documentation index

 * [Overview and basic usage](Basic-usage.md)
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
 * [Fluent API](Fluent-API.md)
 * [Overriding the default mapper](Overriding-mapper.md)
 * [NHibernate integration](NHibernate-integration.md)
 * [Accessing multiple Solr cores / instances](Multi-core-instance.md)
 * [Mapping validation](Schema-Mapping-validation.md)
 * [Sample web application](Sample-application.md)
 * [Websites, products and companies using SolrNet](Powered-by-SolrNet.md)

### Mailing list

If you have any questions about SolrNet, join the [SolrNet google group](http://groups.google.com/group/solrnet) and ask away.  
If you have questions about Solr itself (i.e. not specifically about SolrNet) use the [Solr mailing list](http://lucene.apache.org/solr/discussion.html).

### Contributing

[Paul Bouwer has written an excellent guide for contributors](http://blog.paulbouwer.com/2010/12/27/git-github-and-an-open-source-net-project-introduction/) starting from scratch (no previous Git knowledge required).  
In a nutshell:

 * Don't worry about code formatting, styles, etc.
 * Tests are a must. Without tests, changes will not be merged, except for very specific cases.
 * Whenever possible, favor immutable classes and pure code.

You can also contribute by donating a few bucks:

[![Click here to lend your support to: SolrNet and make a donation at www.pledgie.com !](http://www.pledgie.com/campaigns/11245.png?skin_name=chrome)](http://www.pledgie.com/campaigns/11245)

### Release notes

 * [Initial release](http://bugsquash.blogspot.com/2007/11/introducing-solrnet.html)
 * [0.2.0](http://bugsquash.blogspot.com/2009/02/solrnet-02-released.html)
 * [0.2.1](http://bugsquash.blogspot.com/2009/02/solrnet-021-released.html)
 * [0.2.2](http://bugsquash.blogspot.com/2009/05/solrnet-022-released.html)
 * [0.2.3b1](http://bugsquash.blogspot.com/2009/09/solrnet-023-beta1.html)
 * [0.3.0](http://bugsquash.blogspot.com/2010/06/solr-030-beta1.html)
 * [0.3.1](http://bugsquash.blogspot.com/2011/03/solrnet-031-released.html)
 * [0.4.0a1](http://bugsquash.blogspot.com/2011/06/solrnet-040-alpha-1-released.html)
