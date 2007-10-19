using System;

namespace SolrNet.Exceptions {
	public class CollectionTypeNotSupportedException : BadMappingException {
		private Type collectionType;

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
	}
}