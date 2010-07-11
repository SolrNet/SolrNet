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
    /// Invalid URL specified
    /// </summary>
    [Serializable]
	public class InvalidURLException : SolrNetException {
        /// <summary>
        /// Invalid URL specified
        /// </summary>
        /// <param name="innerException"></param>
		public InvalidURLException(Exception innerException) : base(innerException) {}

        /// <summary>
        /// Invalid URL specified
        /// </summary>
        /// <param name="message"></param>
		public InvalidURLException(string message) : base(message) {}

        /// <summary>
        /// Invalid URL specified
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
		public InvalidURLException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Invalid URL specified
        /// </summary>
		public InvalidURLException() {}

        protected InvalidURLException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}