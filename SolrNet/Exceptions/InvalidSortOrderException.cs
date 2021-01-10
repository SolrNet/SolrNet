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
    /// Error parsing <see cref="SortOrder"/>
    /// </summary>
    [Serializable]
    public class InvalidSortOrderException : SolrNetException {
        /// <summary>
        /// Error parsing <see cref="SortOrder"/> 
        /// </summary>
        public InvalidSortOrderException() {}

        /// <summary>
        /// Error parsing <see cref="SortOrder"/> 
        /// </summary>
        public InvalidSortOrderException(string message) : base(message) {}

        /// <summary>
        /// Error parsing <see cref="SortOrder"/> 
        /// </summary>
        public InvalidSortOrderException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Error parsing <see cref="SortOrder"/> 
        /// </summary>
        public InvalidSortOrderException(Exception innerException) : base(innerException) {}
        
        /// <summary>
        /// Error parsing <see cref="SortOrder"/> 
        /// </summary>
        protected InvalidSortOrderException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
