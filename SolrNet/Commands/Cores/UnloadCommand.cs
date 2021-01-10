using System;

namespace SolrNet.Commands.Cores {
    /// <summary>
    /// Removes a core from Solr. Existing requests will continue to be processed, but no new requests can be sent to this core by the name. 
    /// If a core is registered under more than one name, only that specific mapping is removed.
    /// </summary>
    public class UnloadCommand : CoreCommand {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnloadCommand"/> class.
        /// </summary>
        /// <remarks>
        /// Only available in Solr 3.3 and above.
        /// </remarks>
        /// <param name="coreName">Name of the core.</param>
        /// <param name="delete">If not null, deletes the index once the core is unloaded.  (Only available in 3.3 and above).</param>
        public UnloadCommand(string coreName, Delete delete) {
            if (string.IsNullOrEmpty(coreName))
                throw new ArgumentException("Core Name must be specified.", "coreName");

            AddParameter("action", "UNLOAD");
            AddParameter("core", coreName);
            if (delete != null) {
                var deleteKey = delete.Match<string>(
                    index: () => "deleteIndex",
                    data: () => "deleteDataDir",
                    instance: () => "deleteInstanceDir");
                AddParameter(deleteKey, true.ToString().ToLower());
            }
        }

        /// <summary>
        /// Remove index data on core unload
        /// </summary>
        public sealed class Delete : IEquatable<Delete> {
            private readonly int tag;

            private Delete(int tag) {
                this.tag = tag;
            }

            /// <inheritdoc />
            public bool Equals(Delete other) {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return tag == other.tag;
            }

            /// <inheritdoc />
            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                return obj is Delete && Equals((Delete) obj);
            }

            /// <inheritdoc />
            public override int GetHashCode() {
                return tag;
            }

            public static bool operator ==(Delete left, Delete right) {
                return Equals(left, right);
            }

            public static bool operator !=(Delete left, Delete right) {
                return !Equals(left, right);
            }

            private const int IndexTag = 0;
            private const int DataTag = 1;
            private const int InstanceTag = 2;

            public T Match<T>(Func<T> index, Func<T> data, Func<T> instance) {
                if (tag == IndexTag)
                    return index();
                if (tag == DataTag)
                    return data();
                if (tag == InstanceTag)
                    return instance();
                throw new Exception("Invalid delete tag " + tag);
            }

            /// <summary>
            /// Delete the index on core unload.
            /// Requires Solr 3.3+
            /// </summary>
            public static readonly Delete Index = new Delete(IndexTag);

            /// <summary>
            /// Remove "data" and all sub-directories.
            /// Requires Solr 4.0+
            /// </summary>
            public static readonly Delete Data = new Delete(DataTag);

            /// <summary>
            /// Remove everything related to the core, the index directory, the configuration files, etc.
            /// Requires Solr 4.0+
            /// </summary>
            /// <remarks>
            /// There is a bug in 4.0 (SOLR-3984) that prevents this from working 
            /// unless you specify the absolute path in your core element.
            /// </remarks>
            public static readonly Delete Instance = new Delete(InstanceTag);
        }
    }
}
