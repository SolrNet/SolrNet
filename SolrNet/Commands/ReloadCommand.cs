namespace SolrNet.Commands
{
    using System.Collections.Generic;

    public class ReloadCommand : CoreCommand
    {
        public ReloadCommand(string coreName)
        {
            this.keyValuePairs.Add(new KeyValuePair<string, string>("action", "RELOAD"));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("core", coreName));
        }
    }
}