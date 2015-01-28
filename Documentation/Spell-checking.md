# Spell checking

You'll need to install the [`SpellCheckComponent`](http://wiki.apache.org/solr/SpellCheckComponent) in the standard request handler in order to use this.

Next, a spellcheck dictionary must be provided. Normally a default dictionary is created by invoking `BuildSpellCheckDictionary()` at commit/optimize time (you can also [configure Solr to automatically rebuild spellchecking indices](http://wiki.apache.org/solr/SpellCheckComponent#Building_on_Commits)):

```C#
ISolrOperations<Product> solr = ...
solr.BuildSpellCheckDictionary();
```

Now you can start issuing spellchecking queries by defining the `SpellCheck` parameter in the `QueryOptions`:

```C#
ISolrOperations<Product> solr = ...
var results = solr.Query("ipo appl", new QueryOptions {
  SpellCheck = new SpellCheckingParameters {Collate = true}
});
```

Then you get the suggestions from `results.SpellChecking`, i.e.:

```C#
foreach (var sc in results.SpellChecking) {
    Console.WriteLine("Query: {0}", sc.Query);
    foreach (var s in sc.Suggestions) {
        Console.WriteLine("Suggestion: {0}", s);                    
    }
}
```

which would print something like:

```
Query: ipo
Suggestion: ipod
Query: appl
Suggestion: apple
```

All of the SpellCheckComponent parameters are supported, except for the `extendedResults` option.
