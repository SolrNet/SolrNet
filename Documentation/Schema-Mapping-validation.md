# Schema/mapping validation

SolrNet aims to be flexible to map a Solr schema in different ways depending on the project needs. A single schema may be mapped in several different ways even within a single project. However, there are invalid mappings that will end up in runtime errors, and it may not be evident from these errors that the problem lies in the mapping.

SolrNet offers a facility to aid with the detection of mapping mismatches. Example:

```C#
ISolrOperations<Document> solr = ...
IList<ValidationResult> mismatches = solr.EnumerateValidationResults().ToList();
var errors = mismatches.OfType<ValidationError>();
var warnings = mismatches.OfType<ValidationWarning>();
foreach (var error in errors)
  Console.WriteLine("Mapping error: " + error.Message);
foreach (var warning in warnings)
  Console.WriteLine("Mapping warning: " + warning.Message);
if (errors.Any())
  throw new Exception("Mapping errors, aborting");
```
