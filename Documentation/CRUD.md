# CRUD operations

### Create/Update
Create and update are the same operation in Solr (and SolrNet). It's mapped to the `Add()` method in the `ISolrOperations<T>` interface.

There are two overloads of `Add()`: one takes a single document as parameter, the other takes a `IEnumerable` of documents. Keep in mind that one call to any of these `Add()` methods will end up in one HTTP request to Solr. So for example this:

```
ISolrOperations<MyDocument> solr = ...
for (var i = 0; i < 100; i++)
  solr.Add(new MyDocument(i));
```

is not the same as this:

```
var documents = Enumerable.Range(0, 100).Select(i => new MyDocument(i));
solr.Add(documents);
```

If you `Add()` a document that already exists in the Solr index, it's replaced (using the unique key as equality)

There's also `AddWithBoost()` (also with single-document and `IEnumerable` overloads) that you can use to apply an index-time boosting to the added documents.

`Add()` supports the `commitWithin` and `overwrite` parameters. See the [Solr wiki](http://wiki.apache.org/solr/UpdateXmlMessages#Optional_attributes_for_.22add.22) for more information about them.

### Retrieve
See Querying

### Delete
The ISolrOperations<T> interface has several `Delete()` overloads:

 * `Delete(T doc)`: deletes a single document using its unique key to identify it.
 * `Delete(string id)`: deletes a single document with a unique key
 * `Delete(IEnumerable<T> docs)`: deletes a batch of documents in a single shot.
 * `Delete(IEnumerable<string> ids)`: deletes a batch of documents in a single shot.
 * `Delete(ISolrQuery q)`: deletes all documents that match a query.
 * `Delete(IEnumerable<string> ids, ISolrQuery q)`: deletes a batch of documents and all documents that match a query in a single shot.

### Commit and Optimize
After issuing any number of `Add()` or `Delete()` operations, be sure to call `Commit()`:

```
solr.Add(myDocument);
solr.Add(anotherDocument);
solr.Delete(oldDocument);
solr.Commit();
```

... this tells Solr to finalise the changes you have made and to start rebuilding indexes and related data.

Obviously this has a performance penalty on the Solr instance as all query caches are cleared and repopulated due to the new index.

Alternatively, you can let Solr manage commits, enabling the [autoCommit options](http://wiki.apache.org/solr/SolrConfigXml#Update_Handler_Section) in the Solr configuration.

The `Optimize()` method:

```
solr.Optimize();
```

... issues a command to tell Solr to begin optimizing its indexes. Again this is an expensive operation in terms of the Solr instance and shouldn't be called too frequently.

Additional information on committing and optimization considerations can be found [here](http://stackoverflow.com/a/3737972/21239)

### Rollback
(SolrNet 0.4.0)

`Rollback()` rollbacks all add/deletes made to the index since the last commit. Note that this is nothing like the rollback of relational databases. See the [Solr wiki](http://wiki.apache.org/solr/UpdateXmlMessages#A.22rollback.22) for more information.
