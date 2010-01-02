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
    /// No unique key found (either mapping or value) when one was required.
    /// </summary>
    [Serializable]
    public class NoUniqueKeyException : SolrNetException {
        private readonly Type _t;

        public Type Type {
            get { return _t; }
        }

        public NoUniqueKeyException(Type t) : this(t, string.Format("Type '{0}' has no unique key defined", t)) {}

        public NoUniqueKeyException(Type t, string message) : base(message) {
            _t = t;
        }

        protected NoUniqueKeyException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}