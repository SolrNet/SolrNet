using SolrNet;

namespace SolrNet.Commands
{
    /// <summary>
    /// LukeCommand 
    /// </summary>
    public class LukeCommand : ISolrCommand
    {
        /// <summary>
        /// Calls the /admin/luke.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string Execute(ISolrConnection connection)
        {
            string lukeXml = connection.Get("/admin/luke", null);
            return lukeXml;
        }
    }
}
