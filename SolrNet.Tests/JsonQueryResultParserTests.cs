using System;
using System.Collections.Generic;
using NUnit.Framework;
using SolrNet.Attributes;

namespace SolrNet.Tests {
	[TestFixture]
	public class JsonQueryResultParserTests {
		[Test]
		public void NumFound() {
			ISolrQueryResultParser<TestDocument> parser = new JsonQueryResultParser<TestDocument>();
			var r = parser.Parse(basicResponse);
			Assert.AreEqual(318, r.NumFound);
		}

		[Test]
		public void MaxScore() {
			throw new NotImplementedException();
		}

		[Test]
		public void ParseDoc() {
			var parser = new JsonQueryResultParser<TestDocument>();
			var r = parser.Parse(jsonResponse);
			Assert.AreEqual(1, r.Count);
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

		private const string jsonResponse = @"{
 ""responseHeader"":{
  ""status"":0,
  ""QTime"":0,
  ""params"":{
	""indent"":""on"",
	""start"":""0"",
	""q"":""*:*"",
	""wt"":""json"",
	""version"":""2.2"",
	""rows"":""1""}},
 ""response"":{""numFound"":2114,""start"":0,""docs"":[
	{
	 ""id"":74163,
	 ""make"":20,
	 ""makedesc"":""Mercedes"",
	 ""payment"":523.37,
	 ""timestamp"":""2008-08-14T17:39:32.363Z"",
	 ""description"":[
	  ""Mercedes"",
	  ""2004"",
	  ""California"",
	  ""Playa del Rey"",
	  ""Electric Blue""]}
	]
 }}";

		public class TestDocument : ISolrDocument {
			[SolrField("id")]
			public int Id { get; set;}

			[SolrField("make")]
			public int Make { get; set;}

			[SolrField("makedesc")]
			public string Makedesc { get; set;}

			[SolrField("payment")]
			public decimal Payment { get; set;}

			[SolrField("timestamp")]
			public DateTime Timestamp { get; set;}

			[SolrField("description")]
			public ICollection<string> Description { get; set;}
		}
	}
}