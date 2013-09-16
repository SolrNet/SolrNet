Solr fields defined in your schema.xml must be mapped to properties in a .NET class. There are currently three built-in ways to map fields:

### Attributes (default)
With this method you decorate the properties you want to map with the `SolrField` and `SolrUniqueKey` attributes. The attribute parameter indicates the corresponding Solr field name.

Example:

```csharp
public class Product {
    [SolrUniqueKey("id")]
    public string Id { get; set; }

    [SolrField("manu_exact")]
    public string Manufacturer { get; set; }

    [SolrField("cat")] // cat is a multiValued field
    public ICollection<string> Categories { get; set; }

    [SolrField("price")]
    public decimal Price { get; set; }

    [SolrField("inStock")]
    public bool InStock { get; set; }

    [SolrField("timestamp")]
    public DateTime Timestamp { get; set; }

    [SolrField("weight")]
    public double? Weight { get; set;} // nullable property, it might not be defined on all documents.
}
```

This way of mapping is implemented by the `AttributesMappingManager` class.

### Index-time field boosting
You can also use the mapping attribute to apply a boost to a specific field at index-time.

```C#
[SolrField("inStock", Boost = 10.5)]
public bool InStock { get; set; }
```

.. this will add a boost of 10.5 to the InStock field each time the document is indexed.

### All-properties
This maps each property of the class to a field of the exact same name as the property (note that Solr field names are case-sensitive). It's implemented by the `AllPropertiesMappingManager` class. Note that unique keys cannot be inferred, and therefore have to be explicitly mapped. The same mapping as above could be accomplished like this:

```csharp
public class Product {
    public string id { get; set; }
    public string manu_exact { get; set; }
    public ICollection<string> cat { get; set; }
    public decimal price { get; set; }
    public bool inStock { get; set; }
    public DateTime timestamp { get; set; }
    public double? weight { get; set; }
}
```

Then to add the unique key:

```csharp
var mapper = new AllPropertiesMappingManager();
mapper.SetUniqueKey(typeof(Product).GetProperty("id"));
```

### Manual mapping
This allows you to programmatically define the field for each property:

```csharp
public class Product {
    public string Id { get; set; }
    public string Manufacturer { get; set; }
    public ICollection<string> Categories { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; }
    public DateTime Timestamp { get; set; }
    public double? Weight { get; set; }
}
var mgr = new MappingManager();
var property = typeof (Product).GetProperty("Id");
mgr.Add(property, "id");
mgr.SetUniqueKey(property);
mgr.Add(typeof(Product).GetProperty("Manufacturer"), "manu_exact");
mgr.Add(typeof(Product).GetProperty("Categories"), "cat_exact");
mgr.Add(typeof(Product).GetProperty("Price"), "price");
mgr.Add(typeof(Product).GetProperty("InStock"), "inStock");
mgr.Add(typeof(Product).GetProperty("Timestamp"), "timestamp");
mgr.Add(typeof(Product).GetProperty("Weight"), "weight");
```

### Dictionary mappings and dynamic fields
Solr dynamicFields can be mapped differently depending on the use case. They can be mapped "statically", e.g, given:

```csharp
<dynamicField name="price_*"  type="integer"  indexed="true"  stored="true"/>
```

a particular dynamicField instance can be mapped as:

```csharp
[SolrField("price_i")]
public decimal? Price {get;set;}
```

However, it's often necessary to have more flexibility. You can also map dynamicFields as a dictionary, with a field name prefix:

```csharp
[SolrField("price_")]
public IDictionary<string, decimal> Price {get;set;}
```

In this case, price_ is used as a prefix to the actual Solr field name, e.g. with this mapping, `Price["regular"]` maps to a Solr field named price_regular.

Another, even more flexible mapping:

```csharp
[SolrField("*")]
public IDictionary<string, object> OtherFields {get;set;}
```

This acts as a catch-all container for any fields that are otherwise unmapped. E.g. `OtherFields["price_i"]` maps to a Solr field named `price_i`.

### Fully loose mapping
An even more "dynamic" mapping can be achieved by using a `Dictionary<string,object>` as document type. In this document type, the dictionary key corresponds to the Solr field name and the value to the Solr field value. Statically typing the fields is obviously lost in this case, though.

When adding documents as `Dictionary<string,object>` SolrNet will recognize field value types as usual, e.g. you can use strings, int, collections, arrays, etc. Example:

```csharp
Startup.Init<Dictionary<string, object>>(serverUrl);
var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Dictionary<string, object>>>();
solr.Add(new Dictionary<string, object> {
  {"field1", 1},
  {"field2", "something else"},
  {"field3", new DateTime(2010, 5, 5, 12, 23, 34)},
  {"field4", new[] {1,2,3}},
});
```

When fetching documents as `Dictionary<string,object>` SolrNet will automatically map each field value to a .NET type, but it's up to you to downcast the field value to a properly typed variable. Example:

```csharp
ISolrOperations<Dictionary<string, object>> solr = ...
ICollection<Dictionary<string, object>> results = solr.Query(SolrQuery.All);
bool inStock = (bool) results[0]["inStock"];
```

### Custom mapping
You can code your own mapping mechanism by implementing the [`IReadOnlyMappingManager`](https://github.com/mausch/SolrNet/blob/master/SolrNet/IReadOnlyMappingManager.cs) interface.

To override the default mapping mechanism, see [this page](Overriding-mapper.md).
