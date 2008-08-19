using System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class JsonQueryResultParserTests {
		[Test]
		public void NumFound() {
			ISolrQueryResultParser<TestDocument> parser = new JsonQueryResultParser<TestDocument>();
			var r = parser.Parse(basicResponse);
			Assert.AreEqual(318, r.NumFound);
		}

		private const string basicResponse =
			@"{
 ""responseHeader"":{
	  ""status"":0,
	  ""QTime"":0,
		""params"":{
			""indent"":""on"",
			""start"":""0"",
			""q"":""makedesc:bmw"",
			""wt"":""json"",
			""version"":""2.2"",
			""rows"":""0""}},
 ""response"":{""numFound"":318,""start"":0,""docs"":[]
 }}";

		public class TestDocument : ISolrDocument {}
	}

	public class JsonQueryResultParser<T> : ISolrQueryResultParser<T> where T : ISolrDocument {
		public ISolrQueryResults<T> Parse(string r) {
			var o = JavaScriptConvert.DeserializeObject(r);
			throw new NotImplementedException();
		}
	}
}