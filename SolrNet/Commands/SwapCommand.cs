namespace SolrNet.Commands
{
    using System.Collections.Generic;

    public class SwapCommand : CoreCommand
    {
        public SwapCommand(string coreName, string otherName)
        {
            this.keyValuePairs.Add(new KeyValuePair<string, string>("action", "SWAP"));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("core", coreName));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("other", otherName));
        }
    }
}