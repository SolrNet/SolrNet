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
using SolrNet.Attributes;

namespace SolrNet.Tests.Integration.Sample {
	public class Product {
		[SolrUniqueKey("id")]
		public string Id { get; set; }

        [SolrField("guid")]
        public Guid Guid { get; set; }

		[SolrField("sku")]
		public string SKU { get; set; }

		[SolrField("name")]
		public string Name { get; set; }

		[SolrField("manu")]
		public string Manufacturer { get; set; }

		[SolrField("cat")]
		public ICollection<string> Categories { get; set; }

		[SolrField("features")]
		public ICollection<string> Features { get; set; }

		[SolrField("includes")]
		public string Includes { get; set; }

		[SolrField("weight")]
		public float Weight { get; set; }

		[SolrField("price")]
		public decimal Price { get; set; }

		[SolrField("popularity")]
		public int Popularity { get; set; }

		[SolrField("inStock")]
		public bool InStock { get; set; }

		[SolrField("word")]
		public string Word { get; set; }

		[SolrField("timestamp")]
		public DateTime Timestamp { get; set; }

        [SolrField("price_")]
        public IDictionary<string, decimal> Prices { get; set; }

        [SolrField("ca")]
        public IDictionary<string, ICollection<string>> DynCategories { get; set; }
	}
}