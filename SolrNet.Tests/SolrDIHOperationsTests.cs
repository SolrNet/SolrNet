using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MbUnit.Framework;
using Moroco;
using SolrNet.Commands;
using SolrNet.Impl;
using SolrNet.Tests.Mocks;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrDIHOperationsTests {
        [Test]
        public void FullImportCommandTest() {
            var connection = new MSolrConnection();
            var solr = MakeSolrDihOperations(connection);
            ExpectGet(connection, "/dataimport", new[] {
                KV.Create("command", "full-import")
            });
            solr.FullImport(new DIHOptions());
        }

        [Test]
        public void FullImportOptionsTest() {
            var connection = new MSolrConnection();
            var solr = MakeSolrDihOperations(connection);
            var options = new DIHOptions("custom") {
                Clean = true,
                Commit = true,
                Debug = false,
                Optimize = true
            };
            ExpectGet(connection, "/custom", new[] {
                KV.Create("command", "full-import"),
                KV.Create("clean", "true"),
                KV.Create("commit", "true"),
                KV.Create("debug", "false"),
                KV.Create("optimize", "true")
            });
            solr.FullImport(options);
        }

        [Test]
        public void DeltaImportCommandTest() {
            var connection = new MSolrConnection();
            var solr = MakeSolrDihOperations(connection);
            ExpectGet(connection, "/dataimport", new[] {
                KV.Create("command", "delta-import")
            });
            solr.DeltaImport(new DIHOptions());
        }

        [Test]
        public void AbortCommandTest() {
            var connection = new MSolrConnection();
            var solr = MakeSolrDihOperations(connection);
            ExpectGet(connection, "/dataimport", new[] {
                KV.Create("command", "abort")
            });
            solr.Abort();
        }

        [Test]
        public void ReloadConfigCommandTest() {
            var connection = new MSolrConnection();
            var solr = MakeSolrDihOperations(connection);
            ExpectGet(connection, "/dataimport", new[] {
                KV.Create("command", "reload-config")
            });
            solr.ReloadConfig();
        }

        [Test]
        public void StatusCommandTest() {
            var connection = new MSolrConnection();
            var solr = MakeSolrDihOperations(connection);
            ExpectGet(connection, "/dataimport", new[] {
                KV.Create("command", "status")
            });
            solr.Status();
        }

        [Test]
        public void ParseActualResponseTest() {
            string responseText =
                "<response>" +
                "<lst name=\"responseHeader\">" +
                "<int name=\"status\">0</int>" +
                "<int name=\"QTime\">0</int>" +
                "</lst>" +
                "<lst name=\"initArgs\">" +
                "<lst name=\"defaults\">" +
                "<str name=\"config\">./data-config.xml</str>" +
                "</lst>" +
                "</lst>" +
                "<str name=\"command\">status</str>" +
                "<str name=\"status\">idle</str>" +
                "<str name=\"importResponse\"/>" +
                "<lst name=\"statusMessages\"/>" +
                "<str name=\"WARNING\">" +
                "This response format is experimental. It is likely to change in the future." +
                "</str>" +
                "</response>";
            Func<string, IEnumerable<KeyValuePair<string, string>>, string> getResponse = (url, param) => responseText;
            var connection = new MSolrConnection() {
                get = getResponse
            };
            var parser = new SolrDIHStatusParserFake();
            var solr = new SolrDIHOperations(connection, parser);

            solr.Status();
            Assert.Xml.AreEqual(responseText, parser.receivedXml.ToString());
        }

        private static void ExpectGet(MSolrConnection connection, string url, IEnumerable<KeyValuePair<string, string>> parameters, string response = "<response></response>") {
            connection.get += (getUrl, param) => {
                Assert.AreEqual(url, getUrl);
                Assert.AreElementsEqualIgnoringOrder(parameters, param);
                return response;
            };
        }

        private ISolrDIHOperations MakeSolrDihOperations(ISolrConnection connection) {
            return new SolrDIHOperations(connection, new SolrDIHStatusParserFake());
        }
    }

    internal class SolrDIHStatusParserFake : ISolrDIHStatusParser {
        public XDocument receivedXml;

        public SolrDIHStatus Parse(XDocument solrDIHStatusXml) {
            receivedXml = solrDIHStatusXml;
            return new SolrDIHStatus();
        }
    }
}