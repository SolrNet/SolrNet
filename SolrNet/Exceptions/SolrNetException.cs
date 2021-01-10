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
    /// Base exception for all exceptions thrown by SolrNet
    /// </summary>
    [Serializable]
	public class SolrNetException : ApplicationException {
        /// <summary>
        /// Base exception for all exceptions thrown by SolrNet
        /// </summary>
        /// <param name="innerException"></param>
        public SolrNetException(Exception innerException) : base(innerException.Message, innerException) {}

        /// <summary>
        /// Base exception for all exceptions thrown by SolrNet
        /// </summary>
        /// <param name="message"></param>
		public SolrNetException(string message) : base(message) {}

        /// <summary>
        /// Base exception for all exceptions thrown by SolrNet
        /// </summary>
		public SolrNetException() {}

        /// <summary>
        /// Base exception for all exceptions thrown by SolrNet
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
		public SolrNetException(string message, Exception innerException) : base(message, innerException) {}
        
        /// <summary>
        /// Base exception for all exceptions thrown by SolrNet
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SolrNetException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
