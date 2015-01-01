# NHibernate-SolrNet integration

A NHibernate-SolrNet module is included in SolrNet, with these features:

 * Automatic database -> Solr synchronization
 * Issue Solr queries from NHibernate's ISession

This is intended to be used on entities that are similarly mapped on both NHibernate and SolrNet.

This integration is deprecated. It is not generic enough to be truly reusable for most non-trivial cases. It will be removed in a future release of SolrNet.

### Setup
[Configure SolrNet](Initialization) and NHibernate as usual. Then apply SolrNet's integration to NHibernate's configuration like this:

```C#
NHibernate.Cfg.Configuration cfg = SetupNHibernate();
var cfgHelper = new NHibernate.SolrNet.CfgHelper();
cfgHelper.Configure(cfg, true); // true -> autocommit Solr after every operation (not really recommended)
```

If you're not using the default built-in container, you have to tell `CfgHelper` to use it, e.g.:

```C#
IWindsorContainer container = new WindsorContainer();
...
var cfgHelper = new NHibernate.SolrNet.CfgHelper(container);
...
```

### Usage

Whenever a NHibernate entity that is also mapped in SolrNet is created, updated, or deleted, it will be automatically updated on Solr. NHibernate transactions are honored: entities are updated on Solr only when the NHibernate transaction is committed.

This synchronization goes only in the direction NHibernate -> SolrNet, not the other way around, i.e. operations issued directly through ISolrOperations will not be reflected on NHibernate.

In order to issue Solr queries through NHibernate, the ISession needs to be wrapped. Sample:

```C#
ISessionFactory sessionFactory = ...
using (var session = cfgHelper.OpenSession(sessionFactory)) {
   ICollection<Entity> entities = session.CreateSolrQuery("this is a full-text query").SetMaxResults(10).List<Entity>();
}
```
