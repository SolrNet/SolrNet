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
using SolrNet.Impl.FieldSerializers;


namespace SolrNet {
	/// <summary>
	/// pivot facet query
	/// <see href="http://wiki.apache.org/solr/SimpleFacetParameters#facet.pivot"/>
	/// </summary>
	public class SolrFacetPivotQuery : ISolrFacetQuery
	{
		/// <summary>
		/// A list of fields to pivot. Multiple values will create multiple sections in the response.
		/// <example>
		/// Example single pivot value  new []{"manu,cat"}
		/// Example multiple pivot values :  new [] {"manu,cat","inStock,cat"}
		/// </example>
		/// </summary>

		public ICollection<string> Fields { get; set; }

		/// <summary>
		/// The minimum number of documents that need to match for the result to show up in the results. Default value is 1
		/// </summary>
		public int? MinCount { get; set; }
	}
}
