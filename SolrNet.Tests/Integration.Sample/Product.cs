using System;
using System.Collections.Generic;
using SolrNet.Attributes;

namespace SolrNet.Tests.Integration.Sample {
	public class Product : ISolrDocument {
		[SolrUniqueKey("id")]
		public string Id { get; set; }

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
	}
}