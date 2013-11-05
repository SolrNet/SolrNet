using System;
using System.Collections.Generic;

namespace SolrNet {
    public class SolrDocument {
        private readonly IEnumerable<SolrDocumentOrField> fields;
        private readonly double? boost;

        public SolrDocument(IEnumerable<SolrDocumentOrField> fields, double? boost) {
            this.fields = fields;
            this.boost = boost;
        }

        public IEnumerable<SolrDocumentOrField> Fields {
            get { return fields; }
        }

        public double? Boost {
            get { return boost; }
        }
    }

    public class SolrField {
        private readonly string name;
        private readonly string value;
        private readonly double? boost;

        public SolrField(string name, string value, double? boost) {
            this.name = name;
            this.value = value;
            this.boost = boost;
        }

        public string Name {
            get { return name; }
        }

        public string Value {
            get { return value; }
        }

        public double? Boost {
            get { return boost; }
        }
    }

    public abstract class SolrDocumentOrField {
        private SolrDocumentOrField() {}

        public abstract T Match<T>(Func<SolrDocument, T> document, Func<SolrField, T> field);

        private class SolrDocumentOrField_Document : SolrDocumentOrField {
            private readonly SolrDocument document;

            public SolrDocumentOrField_Document(SolrDocument document) {
                this.document = document;
            }

            public override T Match<T>(Func<SolrDocument, T> document, Func<SolrField, T> field) {
                return document(this.document);
            }
        }

        private class SolrDocumentOrField_Field : SolrDocumentOrField {
            private readonly SolrField field;

            public SolrDocumentOrField_Field(SolrField field) {
                this.field = field;
            }

            public override T Match<T>(Func<SolrDocument, T> document, Func<SolrField, T> field) {
                return field(this.field);
            }
        }

        public static SolrDocumentOrField Document(SolrDocument document) {
            return new SolrDocumentOrField_Document(document);
        }

        public static SolrDocumentOrField Field(SolrField field) {
            return new SolrDocumentOrField_Field(field);
        }
    }
}