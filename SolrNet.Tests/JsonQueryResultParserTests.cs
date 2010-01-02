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
using MbUnit.Framework;
using SolrNet.Attributes;

namespace SolrNet.Tests {
    [TestFixture]
    public class JsonQueryResultParserTests {
        [Test]
        [Ignore("not ready yet")]
        public void NumFound() {
            ISolrQueryResultParser<TestDocument> parser = new JsonQueryResultParser<TestDocument>();
            var r = parser.Parse(basicResponse);
            Assert.AreEqual(318, r.NumFound);
        }

        [Test]
        [Ignore("not ready yet")]
        public void MaxScore() {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("not ready yet")]
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

        private const string jsonResponse =
            @"{
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
            public int Id { get; set; }

            [SolrField("make")]
            public int Make { get; set; }

            [SolrField("makedesc")]
            public string Makedesc { get; set; }

            [SolrField("payment")]
            public decimal Payment { get; set; }

            [SolrField("timestamp")]
            public DateTime Timestamp { get; set; }

            [SolrField("description")]
            public ICollection<string> Description { get; set; }
        }
    }
}