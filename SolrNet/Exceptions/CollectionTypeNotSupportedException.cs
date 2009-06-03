#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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
    /// Thrown when an unknown collection type needed to be mapped to Solr
    /// </summary>
    [Serializable]
	public class CollectionTypeNotSupportedException : BadMappingException {
		private readonly Type collectionType;

		private static readonly string DefaultMessage = "Collection type '{0}' is not supported";

		public Type CollectionType {
			get { return collectionType; }
		}

		public CollectionTypeNotSupportedException(string message, Exception innerException, Type collectionType) : base(message, innerException) {
			this.collectionType = collectionType;
		}

		public CollectionTypeNotSupportedException(Exception innerException, Type collectionType)
			: base(string.Format(DefaultMessage, collectionType), innerException) {
			this.collectionType = collectionType;
		}

		public CollectionTypeNotSupportedException(Type collectionType)
			: base(string.Format(DefaultMessage, collectionType)) {
			this.collectionType = collectionType;
		}

		public CollectionTypeNotSupportedException(string message, Type collectionType) : base(message) {
			this.collectionType = collectionType;
		}

        protected CollectionTypeNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}