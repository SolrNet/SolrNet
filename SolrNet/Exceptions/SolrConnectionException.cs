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
using System.Runtime.Serialization;

namespace SolrNet.Exceptions {
    /// <summary>
    /// Error connecting to Solr. See inner exception for more information.
    /// </summary>
    [Serializable]
	public class SolrConnectionException : SolrNetException {
        private readonly string url;

        /// <summary>
        /// Solr URL
        /// </summary>
        public string Url {
            get { return url; }
        }

        /// <summary>
        /// Error connecting to Solr.
        /// </summary>
        /// <param name="message"></param>
		public SolrConnectionException(string message) : base(message) {}

        /// <summary>
        /// Error connecting to Solr.
        /// </summary>
        /// <param name="innerException"></param>
		public SolrConnectionException(Exception innerException) : base(innerException) {}

        /// <summary>
        /// Error connecting to Solr.
        /// </summary>
        /// <param name="innerException"></param>
        /// <param name="url"></param>
        public SolrConnectionException(Exception innerException, string url) : base(innerException) {
            this.url = url;
        }

        /// <summary>
        /// Error connecting to Solr.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
		public SolrConnectionException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Error connecting to Solr.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="url">Solr URL</param>
        public SolrConnectionException(string message, Exception innerException, string url) : base(message, innerException) {
            this.url = url;
        }

        /// <summary>
        /// Error connecting to Solr.
        /// </summary>
		public SolrConnectionException() {}

        /// <summary>
        /// Error connecting to Solr.
        /// </summary>
        protected SolrConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}
