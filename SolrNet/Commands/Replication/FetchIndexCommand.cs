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

using System;
using System.Collections.Generic;

namespace SolrNet.Commands.Replication
{
    /// <summary>
    /// Force a fetchindex on slave from master command
    /// http://wiki.apache.org/solr/SolrReplication
    /// https://cwiki.apache.org/confluence/display/solr/Index+Replication
    /// </summary>
	public class FetchIndexCommand : ReplicationCommand 
    {
        /// <summary>
        /// Forces the specified slave to fetch a copy of the index from its master. If you like, you 
        /// can pass an extra attribute such as masterUrl or compression (or any other parameter which 
        /// is specified in the <lst name="slave" /> tag) to do a one time replication from a master. 
        /// This obviates the need for hard-coding the master in the slave. 
        /// </summary>
        /// <param name="parameters">Optional parameters</param>
        public FetchIndexCommand(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            AddParameter("command", "fetchindex");

            if (parameters != null)
                foreach (var kv in parameters)
                    AddParameter(kv.Key, kv.Value);
        }
	}
}
