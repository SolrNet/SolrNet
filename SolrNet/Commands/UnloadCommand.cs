namespace SolrNet.Commands
{
    using System.Collections.Generic;

    public class UnloadCommand : CoreCommand
    {
        public UnloadCommand(string coreName)
        {
            this.keyValuePairs.Add(new KeyValuePair<string, string>("action", "UNLOAD"));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("core", coreName));
        }
    }
}