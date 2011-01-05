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

namespace SolrNet
{
    /// <summary>
    /// Contains parameters that can be specified when adding a document to the index.
    /// </summary>
    /// <remarks>
    /// See http://wiki.apache.org/solr/UpdateXmlMessages#Optional_attributes_for_.22add.22
    /// </remarks>
    public class AddParameters {
        /// <summary>
        /// Gets or sets the time period (in milliseconds) within which the document will be committed to the index.
        /// </summary>
        /// <value>The time period (in milliseconds) within which the document will be committed to the index.</value>
        public int? CommitWithin { get; set; }

        /// <summary>
        /// Gets or sets the document overwrite option.
        /// </summary>
        /// <value>If <c>true</c>, newer documents will replace previously added documents with the same uniqueKey.</value>
        public bool? Overwrite { get; set; }
    }
}
