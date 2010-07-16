namespace SolrNet.Commands
{
    using System.Collections.Generic;

    public class RenameCommand : CoreCommand
    {
        public RenameCommand(string coreName, string otherName)
        {
            this.keyValuePairs.Add(new KeyValuePair<string, string>("action", "RENAME"));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("core", coreName));
            this.keyValuePairs.Add(new KeyValuePair<string, string>("other", otherName));
        }
    }
}