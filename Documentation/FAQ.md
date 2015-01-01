## Frequently asked questions about SolrNet

#### Is SolrNet a .NET port of Solr?

No, SolrNet is a HTTP client so you can talk to Solr from your .NET application. If you want to run Solr on .NET, take a look at this [blog post](http://bugsquash.blogspot.com/2011/02/running-solr-on-net.html).

#### What version of Solr do I need? Does SolrNet work with the latest release of Solr?

It's very uncommon for Solr to make breaking changes in core functionality between releases, so SolrNet should work with any fairly recent version of Solr (3.1+). Note that it's up to you to use features that are supported by your Solr version. For example, you will get an error if you try to use grouping with Solr 3.1

#### I'm getting a Bad Request error when calling `Commit()`

You're probably using an outdated version. [Upgrade to a recent build](README.md#downloads)

#### I'm getting a 404 (not found) response from Solr when doing any operation with SolrNet

You're probably missing the core name in the URL you're passing to Startup.Init

#### I'm getting an error "'SolrConnection' already registered in container"

Make sure you call `Startup.Init` [only once per document type in your application](Initialization.md).

#### I created my SolrNet document type, but when I add an instance to Solr I get an 'unknown field' error.

You need to edit the schema.xml in Solr to reflect the fields mapped in your .NET document type.
Schema reference:
 * http://wiki.apache.org/solr/SchemaXml
 * https://cwiki.apache.org/confluence/display/solr/Overview+of+Documents,+Fields,+and+Schema+Design

You can also use the [schema/mapping validation](Schema-Mapping-validation.md) for guidance.

#### How do I get the `score` pseudo-field?

Solr does not return the score by default, so you have to fetch it explicitly. In SolrNet this translates to `Fields = new[] {"*","score"}` in your `QueryOptions` instance. Also, you have to map it to a property in your document type. For example:

```C#
[SolrField("score")]
double? Score { get; set; }
```

#### I'm getting a "URI too long" error

You're hitting the GET request limit of the Solr web container (i.e. Jetty or Tomcat). You can either:
 * edit this limit in the web container configuration, or 
 * make SolrNet use POST instead of GET. You can do this by installing the SolrNet.Impl.PostSolrConnection decorator ([here's an example](http://stackoverflow.com/a/7584526/21239))
