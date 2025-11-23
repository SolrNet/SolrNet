using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace SolrNet.Tests.Common;

public static class TestContainers
{
    public static Uri BaseUrl => _solr.Value.GetUrl();
    public static Uri BaseUrlWithAuth => _solrWithAuth.Value.GetUrl();
    
    public static string SolrVersion => Environment.GetEnvironmentVariable("SOLR_VERSION") ?? "9.6.1";

    static Uri GetUrl(this IContainer container) =>
        new UriBuilder("http", container.Hostname, container.GetMappedPublicPort(8983)).Uri;
    
    static Lazy<IContainer> _solr = new(() => BuildSolrContainer("").GetAwaiter().GetResult());

    private static Lazy<IContainer> _solrWithAuth = new(() =>
    {
        var container = BuildSolrContainer("_auth").GetAwaiter().GetResult();
        AddAuth(container).GetAwaiter().GetResult();
        return container;
    });

    static async Task AddAuth(IContainer solrContainer)
    {
        const string securityJson = @"{
		    ""authentication"":{ 
		    ""blockUnknown"": true, 
		    ""class"":""solr.BasicAuthPlugin"",
		    ""credentials"":{""solr"":""IV0EHq1OnNrj6gvRCwvFwTrZ1+z1oBbnQdiVC3otuq0= Ndd7LKvVBAaZIF0QAVi1ekCfAJXr1GGfLtRUXhgrF8c=""}, 
		    ""realm"":""My Solr users"", 
		    ""forwardCredentials"": false 
		    },
		    ""authorization"":{
		    ""class"":""solr.RuleBasedAuthorizationPlugin"",
		    ""permissions"":[{""name"":""security-edit"",
			    ""role"":""admin""}], 
		    ""user-role"":{""solr"":""admin""} 
		    }
	    }";
        await solrContainer.CopyAsync(Encoding.UTF8.GetBytes(securityJson), "/security.json");
        if (SolrVersion.StartsWith("5"))
        {
            await solrContainer.ExecWithCheck("Error setting up security.json",
                "server/scripts/cloud-scripts/zkcli.sh", 
                "-zkhost", "localhost:9983", 
                "-cmd", "putfile", "/security.json", "/security.json"
            );
        }
        else
        {
            await solrContainer.ExecWithCheck("Error setting up security.json",
                "bin/solr", "zk", "cp", "file:/security.json", "zk:/security.json", "-z", "localhost:9983"
            );
        }
    }

    static async Task<IContainer> BuildSolrContainer(string nameSuffix)
    {
        var container = new ContainerBuilder()
            .WithImage($"solr:{SolrVersion}")
            .WithName($"solr_cloud{nameSuffix}_{Guid.NewGuid()}")
            .WithCommand("/opt/solr/bin/solr", "start", "-cloud", "-f")
            .WithPortBinding(8983, assignRandomHostPort: true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(x => x.ForPort(8983)))
            .Build();
        await container.StartAsync();

        var cores = new[]
        {
            "techproducts",
            "core0",
            "core1",
            "entity1",
            "entity2",
        };
        foreach (var core in cores)
        {
            await container.ExecWithCheck($"Error creating Solr collection for core '{core}'",
                "/opt/solr/bin/solr", "create_collection", 
                "-c", core, 
                "-d", "sample_techproducts_configs"
            );

            await new HttpClient().PostAsJsonAsync($"{container.GetUrl()}solr/{core}/config", new Dictionary<string, object>
            {
                {"update-requesthandler", new Dictionary<string, object>
                {
                    {"name", "/select"},
                    {"class", "solr.SearchHandler"},
                    {"last-components", new[] {"spellcheck"}}
                }}
            });
            
            await container.ExecWithCheck($"Error populating Solr core '{core}'",
                "/opt/solr/bin/post",
                "-c", core,
                "example/exampledocs/"
            );
        }
        return container;
    }

    static async Task<ExecResult> ExecWithCheck(this IContainer container, string error, params string[] command)
    {
        var result = await container.ExecAsync(command);
        if (result.ExitCode != 0)
            throw new Exception(error);
        return result;
    }
}
