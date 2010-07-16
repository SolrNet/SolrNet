namespace SolrNet.Commands
{
    using System.Collections.Generic;

    public class CoreCommand : ISolrCommand
    {
        protected readonly List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();

        public string Execute(ISolrConnection connection)
        {
            return connection.Get("/admin/cores", this.keyValuePairs.ToArray());
        }
    }
}