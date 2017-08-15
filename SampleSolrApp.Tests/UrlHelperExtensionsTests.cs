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
using SampleSolrApp.Helpers;

namespace SampleSolrApp.Tests {
    [TestFixture]
    public class UrlHelperExtensionsTests {

        [Test]
        public void ParseQueryString_empty() {
            var d = UrlHelperExtensions.ParseQueryString("");
            Assert.AreEqual(0, d.Count);
        }

        [Test]
        public void ParseQueryString_null() {
            var d = UrlHelperExtensions.ParseQueryString(null);
            Assert.AreEqual(0, d.Count);
        }

        [Test]
        public void ParseQueryString_only_question_mark() {
            var d = UrlHelperExtensions.ParseQueryString("?");
            Assert.AreEqual(0, d.Count);            
        }

        [Test]
        public void ParseQueryString_admits_question_mark() {
            var d = UrlHelperExtensions.ParseQueryString("?pep=1");
            Assert.AreEqual(1, d.Count);
            Assert.AreEqual("1", d["pep"]);
        }

        [Test]
        public void ParseQueryString_admits_no_question_mark() {
            var d = UrlHelperExtensions.ParseQueryString("pep=1");
            Assert.AreEqual(1, d.Count);
            Assert.AreEqual("1", d["pep"]);
        }

        [Test]
        public void ParseQueryString_is_case_insensitive() {
            var d = UrlHelperExtensions.ParseQueryString("?pep=1&Nothing=bla");
            Assert.AreEqual(2, d.Count);
            Assert.AreEqual("1", d["PeP"]);
            Assert.AreEqual("1", d["pep"]);
            Assert.AreEqual("bla", d["nothing"]);
        }

        [Test]
        public void ParseQueryString_url_decodes() {
            var d = UrlHelperExtensions.ParseQueryString("?pep=1&Nothing=%3D%25");
            Assert.AreEqual(2, d.Count);
            Assert.AreEqual("=%", d["nothing"]);
        }

        [Test]
        public void ParseQueryString_admits_empty_parameters() {
            var d = UrlHelperExtensions.ParseQueryString("?pep=&Nothing=");
            Assert.AreEqual(2, d.Count);
            Assert.AreEqual("", d["pep"]);
            Assert.AreEqual("", d["Nothing"]);
        }

        [Test]
        public void ParseQueryString_admits_extra_ampersands() {
            var d = UrlHelperExtensions.ParseQueryString("?&pep=&&&Nothing=&");
            Assert.AreEqual(2, d.Count);
            Assert.AreEqual("", d["pep"]);
            Assert.AreEqual("", d["Nothing"]);
        }

        [Test]
        public void ParseQueryString_admits_duplicate_parameters_but_keeps_last() {
            var d = UrlHelperExtensions.ParseQueryString("pep=1&pep=2");
            Assert.AreEqual(1, d.Count);
            Assert.AreEqual("2", d["pep"]);
        }

        [Test]
        public void DictToQuerystring_empty() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string>());
            Assert.AreEqual("", s);
        }

        [Test]
        public void DictToQuerystring_empty_key() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string> {
                {"", "a"},
            });
            Assert.AreEqual("", s);
        }

        [Test]
        public void DictToQuerystring_url_encodes() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string> {
                {"pp", "=="},
            });
            Assert.AreEqual("pp=%3d%3d", s);
        }

        [Test]
        public void DictToQuerystring_many_params() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string> {
                {"pp", "1"},
                {"two", "two2"},
            });
            Assert.AreEqual("pp=1&two=two2", s);
        }

        [Test]
        public void DictToQuerystring_null_value() {
            var s = UrlHelperExtensions.DictToQuerystring(new Dictionary<string, string> {
                {"pp", null},
            });
            Assert.AreEqual("pp=", s);
        }

    }
}