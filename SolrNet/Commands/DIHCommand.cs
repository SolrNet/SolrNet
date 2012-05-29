using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet.Utils;

namespace SolrNet.Commands
{
    /// <summary>
    /// Request solr to perform a data import.
    /// </summary>
    public class DIHCommand : ISolrCommand
    {
        public enum DIHCommandType
        {
            None,
            FullImport,
            DeltaImport
        }

        /// <summary>
        /// Name of the data import request handler.
        /// Default is dataimport.
        /// </summary>
        public string HandlerName { get; set; }

        /// <summary>
        /// Define what kind of import to perform. 
        /// If no CommandType is set, the handler returns the status (e.g.: idle).
        /// </summary>
        public DIHCommandType CommandType { get; set; }

        public DIHCommand()
        {
            HandlerName = "dataimport";
        }


        public string Execute(ISolrConnection connection)
        {
            string url = string.Format("/{0}", HandlerName);
            var parameters = GetParameters();
            return connection.Get(url, parameters);
        }

        private IEnumerable<KeyValuePair<string, string>> GetParameters()
        {
            if (CommandType != DIHCommandType.None)
                yield return KV.Create("command", CommandValue(CommandType));
        }

        private string CommandValue(DIHCommandType commandType)
        {
            switch (commandType)
            {
                case DIHCommandType.FullImport:
                    return "full-import";
                case DIHCommandType.DeltaImport:
                    return "delta-import";
                default:
                    return "";
            }
        }
    }
}
