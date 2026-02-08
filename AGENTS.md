## SolrNet is an [Apache Solr](http://solr.apache.org/) and SolrCloud client for .NET

SolrNet does not attempt to abstract much over Solr, it's assumed that you know what Solr is and how to use it, just as you need to know relational databases before using an ORM.

CI is implemented with Github actions in the `.github/workflows` directory.

Some projects in the SolrNet.sln solution:

## SolrNet

This implements the core functionality.

## SolrNet.Tests

Implements unit tests with Xunit for the core functionality.

## SolrNet.Tests.Common

Implements code common to all test projects.

## SolrNet.Tests.Integration

Implements integration tests

## SolrNet.Cloud

Implements a client for SolrCloud

## DI integrations

SolrNet integrates with multiple DI containers. 
Every DI integration is implemented in its own project e.g.  the AutofacContrib.SolrNet project integrates with Autofac.
Also every DI integration has its own test project e.g. AutofacContrib.SolrNet.Tests