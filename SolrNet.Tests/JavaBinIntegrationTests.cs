using System;
using System.Collections;
using System.IO;
using System.Net;
using MbUnit.Framework;
using SolrNet.Impl;

namespace SolrNet.Tests {
    [TestFixture]
    [Category("Integration")]
    public class JavaBinIntegrationTests {
        [Test]
        public void DeserializeFromSolr() {
            var request = WebRequest.Create("http://localhost:8983/solr/select/?q=*%3A*&wt=javabin");
            using (var response = request.GetResponse())
            using (var s = response.GetResponseStream()) {
                var stuff = new JavaBinCodec().Unmarshal(s);
            }
        }

        [Test]
        public void SaveSolrJavaBinToFile() {
            var request = WebRequest.Create("http://localhost:8983/solr/select/?q=*%3A*&wt=javabin");
            var buffer = new byte[32768];
            using (var response = request.GetResponse())
            using (var s = response.GetResponseStream())
            using (var f = new FileStream(@"..\..\solrresponse.bin", FileMode.Create)) {
                int l;
                while ((l = s.Read(buffer, 0, buffer.Length)) > 0)
                    f.Write(buffer, 0, l);
            }
        }

        [Test]
        public void DeserializeFromJavaBinFile() {
            using (var f = new FileStream(@"..\..\solrresponse.bin", FileMode.Open)) {
                var response = new JavaBinCodec().Unmarshal(f);
                Console.WriteLine(response.GetType());
                var d = (IDictionary)response;
                Console.WriteLine(d.Count);
                DumpDict(d);

                Assert.IsTrue(d.Contains("responseHeader"));
                var header = (IDictionary) d["responseHeader"];
                Assert.AreEqual(0, header["status"]);
                Assert.AreEqual(0, header["QTime"]);
                var param = (IDictionary) header["params"];
                Assert.AreEqual("javabin", param["wt"]);
                Assert.AreEqual("*:*", param["q"]);

                Assert.IsTrue(d.Contains("response"));
                var docs = (JavaBinCodec.SolrDocumentList) d["response"];
                Assert.AreEqual(1, docs.NumFound);
                Assert.AreEqual(0, docs.Start);
                Assert.IsNull(docs.MaxScore);
                var doc = docs[0];
                Assert.AreEqual("abcd", doc["id"]);
                Assert.AreEqual("Testing NH-Solr integration", doc["name"]);
                Assert.AreEqual(new DateTime(2009, 9, 5, 21, 17, 52, 691), doc["timestamp"]);
                var categories = (IList) doc["cat"];
                Assert.AreEqual("cat1", categories[0]);
                Assert.AreEqual("aoe", categories[1]);
            }
        }

        public void DumpDict(IDictionary d) {
            foreach (DictionaryEntry e in d) {
                Console.WriteLine(e.Key);
                Console.WriteLine(e.Value.GetType());
                if (e.Value is IDictionary) {
                    DumpDict((IDictionary) e.Value);
                } else if (e.Value is JavaBinCodec.SolrDocumentList) {
                    var list = (JavaBinCodec.SolrDocumentList)e.Value;
                    Console.WriteLine("NumFound: {0}", list.NumFound);
                    Console.WriteLine("Start: {0}", list.Start);
                    Console.WriteLine("MaxScore: {0}", list.MaxScore);
                    Console.WriteLine("Count: {0}", list.Count);
                    foreach (var doc in list) {
                        foreach (var field in doc) {
                            Console.Write("field {0} = ", field.Key);
                            if (field.Value is ICollection) {
                                var multiValued = (ICollection) field.Value;
                                foreach (var v in multiValued)
                                    Console.WriteLine(v);
                            } else {
                                Console.WriteLine(field.Value);
                            }
                        }
                    }
                } else {
                    Console.WriteLine(e.Value);
                }
            }            
        }
    }
}