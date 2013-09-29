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

using System.Xml.Linq;

namespace SolrNet {
	/// <summary>
	/// Serializes a solr document to xml. 
	/// </summary>
	/// <typeparam name="T">document type</typeparam>
	public interface ISolrDocumentSerializer<in T> {
		/// <summary>
        /// Serializes a Solr document to xml, applying an index-time boost
        /// </summary>
		/// <param name="doc">document to serialize</param>
		/// <param name="boost"></param>
		/// <returns>serialized document</returns>
		XElement Serialize(T doc, double? boost);
	}
}