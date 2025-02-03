# Observability

[OpenTelemetry](https://opentelemetry.io/docs/what-is-opentelemetry/) is the open-source standard for distributed tracing.
SolrNet offers [OpenTelemetry Tracing](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing) support out-of-the-box.

## Tracing

### ASP.NET Core application

This example is using the following packages:

- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Exporter.Console`

```C#
var builder = WebApplication.CreateBuilder(args);

void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}

builder.Services.AddOpenTelemetry()
    .ConfigureResource(ConfigureResource)
    .WithTracing(b => b
        .AddSource(SolrNet.DiagnosticHeaders.DefaultSourceName) // SolrNet ActivitySource
        .AddConsoleExporter() // Any OTEL supportable exporter can be used here
    );
```

### Console application

This example is using the following packages:

- `OpenTelemetry`
- `OpenTelemetry.Exporter.Console`

```C#
void ConfigureResource(ResourceBuilder r)
{
    r.AddService("Service Name",
        serviceVersion: "Version",
        serviceInstanceId: Environment.MachineName);
}

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .ConfigureResource(ConfigureResource)
    .AddSource(SolrNet.DiagnosticHeaders.DefaultSourceName) // SolrNet ActivitySource
    .AddConsoleExporter() // Any OTEL supportable exporter can be used here
    .Build();
```

SolrNet spans contain the following attributes:

Attribute | Description                                           | Examples 
--------|-------------------------------------------------------|----------------|
`db.system`	| The database product identifier                       | `solr`
`db.collection.name` | The solr core being targeted (_optional_)            | `techproducts`
`db.query.text`	| The request parameters as a json string (_optional_)  | `[{"q":"ipad"},{"rows":"100"},{"fq":"price:[0 TO 8]"},{"facet":"true"},{"facet.field":"cat"},{"sort":"price asc"}]`
`db.operation.name`	| The name of the operation or command being executed   | `query`, `commit`, `add`, `delete`, ...
`http.request.method` | The underlying HTTP request method                    | `GET`, `POST`
`server.address`	| Host name of the solr server                          | `localhost`
`server.port`	| Port of the solr server                               | `8983`                               
`url.full`	| The full underlying request URL                       | `http://localhost:8983/solr/techproducts/select?q=ipad&rows=100&fq=price%3A%5B0+TO+8%5D&facet=true&facet.field=cat&sort=price+asc&version=2.2&wt=xml`
`solr.status` | Status header from the solr response (_optional_)     | `0` means success
`solr.qtime` | Query time header from the solr response (_optional_) | `42`
`error.type` | Error message (_optional_) | `The remote server returned an error: (404) Not Found.`
