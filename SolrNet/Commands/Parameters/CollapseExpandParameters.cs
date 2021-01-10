using System;

namespace SolrNet.Commands.Parameters
{
    /// <summary>
    /// Parameters for CollapsingQParserPlugin / ExpandComponent
    /// </summary>
    public class CollapseExpandParameters 
    {
        private readonly string field;
        private readonly ExpandParameters expand;
        private readonly MinOrMax minOrMaxField;
        private readonly NullPolicyType nullPolicy;

        /// <summary>
        /// Field to group results by
        /// </summary>
        public string Field 
        {
            get { return field; }
        }

        /// <summary>
        /// Used to expand the results.
        /// Null if expansion is not requested.
        /// </summary>
        public ExpandParameters Expand 
        {
            get { return expand; }
        }

        /// <summary>
        /// Field or function to use for min/max collapsing
        /// </summary>
        public MinOrMax MinOrMaxField {
            get { return minOrMaxField; }
        }

        /// <summary>
        /// Determines how to treat nulls while collapsing
        /// </summary>
        public NullPolicyType NullPolicy {
            get { return nullPolicy; }
        }

        /// <summary>
        /// Parameters for CollapsingQParserPlugin / ExpandComponent
        /// </summary>
        /// <param name="field">Field to collapse</param>
        /// <param name="expand"></param>
        /// <param name="minOrMaxField"></param>
        /// <param name="nullPolicy"></param>
        public CollapseExpandParameters(string field, ExpandParameters expand, MinOrMax minOrMaxField, NullPolicyType nullPolicy) {
            if (field == null)
                throw new ArgumentNullException("field");
            this.field = field;
            this.expand = expand;
            this.minOrMaxField = minOrMaxField;
            this.nullPolicy = nullPolicy;
        }

        public abstract class MinOrMax {
            private readonly string field;

            public string Field {
                get { return field; }
            }

            private MinOrMax(string field) {
                this.field = field;
            }

            public abstract T Switch<T>(Func<Min, T> min, Func<Max, T> max);

            public class Min: MinOrMax {
                public Min(string field) : base(field) {}

                public override T Switch<T>(Func<Min, T> min, Func<Max, T> max) {
                    return min(this);
                }
            }

            public class Max: MinOrMax {
                public Max(string field) : base(field) {}

                public override T Switch<T>(Func<Min, T> min, Func<Max, T> max) {
                    return max(this);
                }
            }
        }

        /// <summary>
        /// Determines how to treat nulls while collapsing
        /// </summary>
        public class NullPolicyType : IEquatable<NullPolicyType> {
            private readonly string policy;

            private NullPolicyType(string policy) {
                this.policy = policy;
            }

            /// <summary>
            /// Determines how to treat nulls while collapsing
            /// </summary>
            public string Policy {
                get { return policy; }
            }

            /// <inheritdoc />
            public override string ToString() {
                return policy;
            }

            /// <summary>
            /// Removes documents with a null value in the collapse field. This is the default.
            /// </summary>
            public static readonly NullPolicyType Ignore = new NullPolicyType("ignore");

            /// <summary>
            /// Treats each document with a null value in the collapse field as a separate group.
            /// </summary>
            public static readonly NullPolicyType Expand = new NullPolicyType("expand");

            /// <summary>
            /// Collapses all documents with a null value into a single group using either highest score, or minimum/maximum.
            /// </summary>
            public static readonly NullPolicyType Collapse = new NullPolicyType("collapse");

            /// <inheritdoc />
            public bool Equals(NullPolicyType other) {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return string.Equals(policy, other.policy);
            }

            /// <inheritdoc />
            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != this.GetType())
                    return false;
                return Equals((NullPolicyType) obj);
            }

            /// <inheritdoc />
            public override int GetHashCode() {
                return (policy != null ? policy.GetHashCode() : 0);
            }

            public static bool operator ==(NullPolicyType left, NullPolicyType right) {
                return Equals(left, right);
            }

            public static bool operator !=(NullPolicyType left, NullPolicyType right) {
                return !Equals(left, right);
            }
        }
    }

    /// <summary>
    /// ExpandComponent parameters
    /// </summary>
    public class ExpandParameters {
        private readonly SortOrder sort;
        private readonly int? rows;
        private readonly ISolrQuery query;
        private readonly ISolrQuery filterQuery;

        /// <summary>
        /// Orders the documents with the expanded groups.
        /// </summary>
        public SortOrder Sort {
            get { return sort; }
        }

        /// <summary>
        /// The number of rows to display in each group.
        /// </summary>
        public int? Rows {
            get { return rows; }
        }

        /// <summary>
        /// Overrides the main q parameter, determines which documents to include in the main group.
        /// </summary>
        public ISolrQuery Query {
            get { return query; }
        }

        /// <summary>
        /// Overrides main fq's, determines which documents to include in the main group.
        /// </summary>
        public ISolrQuery FilterQuery {
            get { return filterQuery; }
        }

        /// <summary>
        /// ExpandComponent parameters
        /// </summary>
        /// <param name="sort">Orders the documents with the expanded groups. By default: score desc</param>
        /// <param name="rows">The number of rows to display in each group. By default: 5</param>
        /// <param name="query">(Optional) Overrides the main q parameter, determines which documents to include in the main group.</param>
        /// <param name="filterQuery">(Optional) Overrides main fq, determines which documents to include in the main group.</param>
        public ExpandParameters(SortOrder sort, int? rows, ISolrQuery query, ISolrQuery filterQuery) {
            this.sort = sort;
            this.rows = rows;
            this.query = query;
            this.filterQuery = filterQuery;
        }
    }
}
