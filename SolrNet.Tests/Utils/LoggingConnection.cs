using System;
using System.Collections.Generic;

namespace SolrNet.Tests.Utils {
    public class LoggingConnection : ISolrConnection {
        private readonly ISolrConnection conn;

        public LoggingConnection(ISolrConnection conn) {
            this.conn = conn;
        }

        public string ServerURL {
            get { return conn.ServerURL; }
            set { conn.ServerURL = value; }
        }

        public string Version {
            get { return conn.Version; }
            set { conn.Version = value; }
        }

        public string Post(string relativeUrl, string s) {
            Console.WriteLine("Posting {0}", s);
            return conn.Post(relativeUrl, s);
        }

        public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) {
            Console.WriteLine("Getting");
            var r = conn.Get(relativeUrl, parameters);
            Console.WriteLine("Result is:\n" + r);
            return r;
        }
    }
}