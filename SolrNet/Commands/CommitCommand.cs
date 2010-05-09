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
using System.Xml;

namespace SolrNet.Commands {
    /// <summary>
    /// Commits updates
    /// </summary>
	public class CommitCommand : ISolrCommand {

		/// <summary>
		/// Block until index changes are flushed to disk
		/// Default is true
		/// </summary>
		public bool? WaitFlush { get; set; }

		/// <summary>
		/// Block until a new searcher is opened and registered as the main query searcher, making the changes visible. 
		/// Default is true
		/// </summary>
		public bool? WaitSearcher { get; set; }

        /// <summary>
        /// Merge segments with deletes away
        /// Default is false
        /// </summary>
        public bool? ExpungeDeletes { get; set; }

        /// <summary>
        /// Optimizes down to, at most, this number of segments
        /// Default is 1
        /// </summary>
        public int? MaxSegments { get; set; }

		public string Execute(ISolrConnection connection) {
			var xml = new XmlDocument();
			var node = xml.CreateElement("commit");

            foreach (var p in new[] { new KeyValuePair<bool?, string>(WaitSearcher, "waitSearcher"), new KeyValuePair<bool?, string>(WaitFlush, "waitFlush"), new KeyValuePair<bool?, string>(ExpungeDeletes, "expungeDeletes") })
            {
                if (!p.Key.HasValue) continue;

                var att = xml.CreateAttribute(p.Value);
                att.InnerText = p.Key.Value.ToString().ToLower();
                node.Attributes.Append(att);
            }

            if (MaxSegments.HasValue)
            {
                var att = xml.CreateAttribute("maxSegments");
                att.InnerText = MaxSegments.ToString();
                node.Attributes.Append(att);    
            }

			return connection.Post("/update", node.OuterXml);
		}
	}
}