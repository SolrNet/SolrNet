namespace SolrNet.Commands
{
    using System.Collections.Generic;

    public class StatusCommand : CoreCommand
    {
        public StatusCommand()
        {
            this.keyValuePairs.Add(new KeyValuePair<string, string>("action", "STATUS"));
        }
        
        public StatusCommand(string coreName) : base()
        {
            this.keyValuePairs.Add(new KeyValuePair<string, string>("core", coreName));
        }
    }
}