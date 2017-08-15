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
using System.IO;
using System.Linq;
using log4net;
using SolrNet;

namespace SampleSolrApp {
    public class LoggingConnection: ISolrConnection {
        private readonly ISolrConnection connection;

        public LoggingConnection(ISolrConnection connection) {
            this.connection = connection;
        }

        public string Post(string relativeUrl, string s) {
            logger.DebugFormat("POSTing '{0}' to '{1}'", s, relativeUrl);
            return connection.Post(relativeUrl, s);
        }

        public string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters) {
            logger.DebugFormat("POSTing to '{0}'", relativeUrl);
            return connection.PostStream(relativeUrl, contentType, content, getParameters);
        }

        public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) {
            var stringParams = string.Join(", ", parameters.Select(p => string.Format("{0}={1}", p.Key, p.Value)).ToArray());
            logger.DebugFormat("GETting '{0}' from '{1}'", stringParams, relativeUrl);
            return connection.Get(relativeUrl, parameters);
        }

        private static readonly ILog logger = LogManager.GetLogger(typeof (LoggingConnection));
    }
}