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

namespace SolrNet.Commands.Replication
{
    /// <summary>
    /// Abort copying index from master to slave command.
    /// http://wiki.apache.org/solr/SolrReplication
    /// https://cwiki.apache.org/confluence/display/solr/Index+Replication
    /// </summary>
    public class AbortFetchCommand : ReplicationCommand 
    {
        /// <summary>
        /// Aborts copying an index from a master to the specified slave.
        /// </summary>
        public AbortFetchCommand()
        {
            AddParameter("command", "abortfetch");
        }
	}
}
