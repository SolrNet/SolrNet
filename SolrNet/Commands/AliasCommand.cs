namespace SolrNet.Commands
{
    using System.Collections.Generic;

    public class AliasCommand : CoreCommand
    {
        public AliasCommand(string coreName, string otherName)
        {
            this.keyValuePairs.Add(new KeyValuePair<string, string>("action", "ALIAS"));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("core", coreName));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("other", otherName));
        }
    }
}