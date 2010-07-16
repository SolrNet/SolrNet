namespace SolrNet.Commands
{
    using System.Collections.Generic;

    public class CreateCommand : CoreCommand
    {
        public CreateCommand(string coreName, string instanceDir, string configFile, string schemaFile, string dataDir)
        {
            this.keyValuePairs.Add(new KeyValuePair<string, string>("action", "SWAP"));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("name", coreName));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("instanceDir", instanceDir));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("config", configFile));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("schema", schemaFile));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("dataDir", dataDir));
        }
    }
}