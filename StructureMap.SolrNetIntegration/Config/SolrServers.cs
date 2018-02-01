using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructureMap.SolrNetIntegration.Config
{
    /// <summary>
    /// List of ISolrServer, backward compatible usage.
    /// </summary>
    [Obsolete("Just use List<ISolrServer>")]
    public class SolrServers : List<ISolrServer>
    {
    }
}
