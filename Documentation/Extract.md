# Binary document upload

SolrNet supports Solr "extract" feature (a.k.a. Solr "Cell") to index data from binary document formats such as Word, PDF, etc.

Here's a simple example showing how to extract text from a PDF file, without indexing it:

```csharp
ISolrOperations<Something> solr = ...
using (var file = File.OpenRead(@"test.pdf")) {
    var response = solr.Extract(new ExtractParameters(file, "some_document_id") {
        ExtractOnly = true,
        ExtractFormat = ExtractFormat.Text,
    });
    Console.WriteLine(response.Content);
}
```

`ExtractOnly = true` tells Solr to just perform text extraction but not index the uploaded document.
If `ExtractOnly = false` you can add more fields with the `Fields` property. 
Other options can be set through the properties of the [`ExtractParameters` class](https://github.com/mausch/SolrNet/blob/master/SolrNet/ExtractParameters.cs).
It's usually recommended to provide the `StreamType` for the content, as auto-detection might fail.

For more details about each option in `ExtractParameters` see the [Solr wiki](https://wiki.apache.org/solr/ExtractingRequestHandler) and the [Solr reference guide](https://cwiki.apache.org/confluence/display/solr/Uploading+Data+with+Solr+Cell+using+Apache+Tika).
