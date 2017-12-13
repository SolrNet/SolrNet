## SolrNet is an [Apache Solr](http://lucene.apache.org/solr/) client for .NET Test 2

SolrNet does not attempt to abstract much over Solr, it's assumed that you know what Solr is and how to use it, just as you need to know relational databases before using an ORM.

If you're not familiar with Solr, take your time to follow the [Solr tutorial](http://lucene.apache.org/solr/tutorial.html), see the [FAQ](http://wiki.apache.org/solr/FAQ) and the [docs](http://wiki.apache.org/solr/FrontPage). Consider getting a [book](http://lucene.apache.org/solr/books.html).

<!-- This page documents SolrNet features in the master branch. For version-specific documentation, see the Documentation directory on the corresponding version branch. For example https://github.com/mausch/SolrNet/blob/0.4.x/Documentation/README.md -->

### Downloads

It's currently recommended to get the latest binaries directly from the build server. [![Build status](https://ci.appveyor.com/api/projects/status/0oj6vqpnoyw08jtq?svg=true)](https://ci.appveyor.com/project/XavierMorera/solrnet-crl26). <!--The build server also has a NuGet feed with these nightly builds: https://ci.appveyor.com/nuget/solrnet-022x5w7kmuba -->

Otherwise, NuGet packages at nuget.org are available:

 * [SolrNet](https://www.nuget.org/packages/SolrNet/) (core library)
 * [SolrNet.Windsor](https://www.nuget.org/packages/SolrNet.Windsor/)
 * [SolrNet.StructureMap](https://www.nuget.org/packages/SolrNet.StructureMap/)
 * [SolrNet.Ninject](https://www.nuget.org/packages/SolrNet.Ninject/)
 * [SolrNet.Unity](https://www.nuget.org/packages/SolrNet.Unity/)
 * [SolrNet.Autofac](https://www.nuget.org/packages/SolrNet.Autofac/)
 * [SolrNet.NHibernate](https://www.nuget.org/packages/SolrNet.NHibernate/)

### Documentation index

 * [Overview and basic usage](Documentation/Basic-usage.md)
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
 * [Fluent API](Documentation/Fluent-API.md)
 * [Overriding the default mapper](Documentation/Overriding-mapper.md)
 * [NHibernate integration](Documentation/NHibernate-integration.md)
 * [Accessing multiple Solr cores / instances](Documentation/Multi-core-instance.md)
 * [Mapping validation](Documentation/Schema-Mapping-validation.md)
 * [Binary document upload](Documentation/Extract.md)
 * [Sample web application](Documentation/Sample-application.md)
 * [FAQ](Documentation/FAQ.md)
 * [Websites, products and companies using SolrNet](Documentation/Powered-by-SolrNet.md)

### Mailing list

If you have any questions about SolrNet, please create an issue and ask away.  
If you have questions about Solr itself (i.e. not specifically about SolrNet) use the [Solr mailing list](http://lucene.apache.org/solr/discussion.html).

### Contributing

[Paul Bouwer has written an excellent guide for contributors](http://blog.paulbouwer.com/2010/12/27/git-github-and-an-open-source-net-project-introduction/) starting from scratch (no previous Git knowledge required).  
In a nutshell:

 * Don't worry about code formatting, styles, etc.
 * Tests are a must. Without tests, changes will not be merged, except for very specific cases.
 * Whenever possible, favor immutable classes and pure code.
 * If you're adding a new feature or making a breaking change, update the corresponding documentation.
 * SolrNet has recently adopted using [Gitflow workflow](http://nvie.com/posts/a-successful-git-branching-model/) as its branching strategy, whenever you contribute please from your fork create a pull request with a branch that is up to date. Once confirmed, it will be merged and a new version will be released.

You can also contribute by donating a few bucks:

[![Click here to lend your support to: SolrNet and make a donation at www.pledgie.com !](http://www.pledgie.com/campaigns/11245.png?skin_name=chrome)](http://www.pledgie.com/campaigns/11245)

### Releases and Release Notes

 * [Initial release](http://bugsquash.blogspot.com/2007/11/introducing-solrnet.html)
 * [0.2.0](http://bugsquash.blogspot.com/2009/02/solrnet-02-released.html)
 * [0.2.1](http://bugsquash.blogspot.com/2009/02/solrnet-021-released.html)
 * [0.2.2](http://bugsquash.blogspot.com/2009/05/solrnet-022-released.html)
 * [0.2.3b1](http://bugsquash.blogspot.com/2009/09/solrnet-023-beta1.html)
 * [0.3.0](http://bugsquash.blogspot.com/2010/06/solr-030-beta1.html)
 * [0.3.1](http://bugsquash.blogspot.com/2011/03/solrnet-031-released.html)
 * [0.4.0a1](http://bugsquash.blogspot.com/2011/06/solrnet-040-alpha-1-released.html)
 * [0.5.1] Includes spellchecker parser issue
 * [0.7.1] .NET 4.6 support, replace Gallio with XUnit, NuGet and .NET Core support
 * [0.9.1](https://github.com/SolrNet/SolrNet/releases/tag/0.9.1) Additional updates for .NET Core support among others 

