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
    /// Document field type not supported by configured serializers
    /// </summary>
    [Serializable]
    public class TypeNotSupportedException : SolrNetException {
        /// <summary>
        /// Document field type not supported by configured serializers
        /// </summary>
        public TypeNotSupportedException(Exception innerException) : base(innerException) {}
        
        /// <summary>
        /// Document field type not supported by configured serializers
        /// </summary>
        public TypeNotSupportedException(string message) : base(message) {}
        
        /// <summary>
        /// Document field type not supported by configured serializers
        /// </summary>
        public TypeNotSupportedException() {}
        
        /// <summary>
        /// Document field type not supported by configured serializers
        /// </summary>
        public TypeNotSupportedException(string message, Exception innerException) : base(message, innerException) {}
        
        /// <summary>
        /// Document field type not supported by configured serializers
        /// </summary>
        protected TypeNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
