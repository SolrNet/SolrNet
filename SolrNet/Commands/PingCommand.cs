#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolrNet.Commands {
    /// <summary>
    /// Pings the Solr server.
    /// It can be used by a load balancer in front of a set of Solr servers to check response time of all the Solr servers in order to do response time based load balancing.
    /// </summary>
    /// <seealso href="http://wiki.apache.org/solr/SolrConfigXml"/>
	public class PingCommand : ISolrCommand {
        /// <inheritdoc />        
		public string Execute(ISolrConnection connection) {
			return connection.Get("/admin/ping", new Dictionary<string, string>());
		}
        
        /// <inheritdoc />
        public Task<string> ExecuteAsync(ISolrConnection connection)
        {
            return connection.GetAsync("/admin/ping", new Dictionary<string, string>());
        }
    }
}
