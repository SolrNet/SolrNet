using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace SolrNet.Tests.Integration {
	[TestFixture]
	public class AddCommandTests {

		public class TestDocument: ISolrDocument {
			private int id;
			private string make;
			private string model;
			private string style;
			private string category;
			private int year;
			private decimal price;
			private int months;
			private int region;
			private int miles;
			private string state;
			private bool visible;
			private bool photo;
			private bool transferring;
			private bool sponsored;
			private bool hotdeal;
			private bool featured;
			private string basicView;
			private string advancedView;

			public string Key {
				get { return id.ToString(); }
			}

			[SolrField("id")]
			public int Id {
				get { return id; }
				set { id = value; }
			}

			[SolrField("make")]
			public string Make {
				get { return make; }
				set { make = value; }
			}

			[SolrField("model")]
			public string Model {
				get { return model; }
				set { model = value; }
			}

			[SolrField("style")]
			public string Style {
				get { return style; }
				set { style = value; }
			}

			[SolrField("category")]
			public string Category {
				get { return category; }
				set { category = value; }
			}

			[SolrField("year")]
			public int Year {
				get { return year; }
				set { year = value; }
			}

			[SolrField("price")]
			public decimal Price {
				get { return price; }
				set { price = value; }
			}

			[SolrField("months")]
			public int Months {
				get { return months; }
				set { months = value; }
			}

			[SolrField("region")]
			public int Region {
				get { return region; }
				set { region = value; }
			}

			[SolrField("miles")]
			public int Miles {
				get { return miles; }
				set { miles = value; }
			}

			[SolrField("state")]
			public string State {
				get { return state; }
				set { state = value; }
			}

			[SolrField("visible")]
			public bool Visible {
				get { return visible; }
				set { visible = value; }
			}

			[SolrField("photo")]
			public bool Photo {
				get { return photo; }
				set { photo = value; }
			}

			[SolrField("transferring")]
			public bool Transferring {
				get { return transferring; }
				set { transferring = value; }
			}

			[SolrField("sponsored")]
			public bool Sponsored {
				get { return sponsored; }
				set { sponsored = value; }
			}

			[SolrField("hotdeal")]
			public bool Hotdeal {
				get { return hotdeal; }
				set { hotdeal = value; }
			}

			[SolrField("featured")]
			public bool Featured {
				get { return featured; }
				set { featured = value; }
			}

			[SolrField("basicview")]
			public string BasicView {
				get { return basicView; }
				set { basicView = value; }
			}

			[SolrField("advancedview")]
			public string AdvancedView {
				get { return advancedView; }
				set { advancedView = value; }
			}
		}

		[Test]
		[Category("Integration")]
		public void AddOne() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(conn);
			TestDocument doc = new TestDocument();
			doc.Category = "cat";
			doc.Id = 123456;
			solr.Add(doc);
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		public void AddMany() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(conn);
			TestDocument doc = new TestDocument();
			doc.Category = "cat";
			solr.Add(new TestDocument[] {doc, doc});
			Console.WriteLine(solr.Commit());
		}

	}
}
