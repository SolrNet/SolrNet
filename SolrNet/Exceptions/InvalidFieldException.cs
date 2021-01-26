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
    /// Solr did not understand one the specified fields
    /// </summary>
    [Serializable]
    [Obsolete("No longer thrown, catch SolrNetException or SolrConnectionException instead")]
	public class InvalidFieldException : SolrNetException {
        /// <summary>
        /// Solr did not understand one the specified fields
        /// </summary>
        /// <param name="innerException"></param>
		public InvalidFieldException(Exception innerException) : base(innerException) {}

        /// <summary>
        /// Solr did not understand one the specified fields
        /// </summary>
        /// <param name="message"></param>
		public InvalidFieldException(string message) : base(message) {}

        /// <summary>
        /// Solr did not understand one the specified fields
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
		public InvalidFieldException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Solr did not understand one the specified fields
        /// </summary>
		public InvalidFieldException() {}
        
        /// <summary>
        /// Solr did not understand one the specified fields
        /// </summary>
        protected InvalidFieldException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}
