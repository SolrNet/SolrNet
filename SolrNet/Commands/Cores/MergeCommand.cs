using System;
using System.Collections.Generic;
using System.Linq;

namespace SolrNet.Commands.Cores {
    /// <summary>
    /// Merges one or more cores into another core.
    /// See https://wiki.apache.org/solr/MergingSolrIndexes for details.
    /// </summary>
    public class MergeCommand: CoreCommand {
        /// <summary>
        /// Use an index path as merge source
        /// </summary>
        public sealed class IndexDir {
            /// <summary>
            /// Index directory
            /// </summary>
            public readonly string Dir;

            /// <summary>
            /// Use an index path as merge source
            /// </summary>
            /// <param name="dir">Index path</param>
            public IndexDir(string dir) {
                Dir = dir;
            }
        }

        /// <summary>
        /// Use a core name as merge source
        /// </summary>
        public sealed class SrcCore {
            /// <summary>
            /// Core name
            /// </summary>
            public readonly string CoreName;

            /// <summary>
            /// Use a core name as merge source
            /// </summary>
            /// <param name="coreName">Core name</param>
            public SrcCore(string coreName) {
                CoreName = coreName;
            }
        }

        /// <summary>
        /// Merge indexes using their path to identify them.
        /// Requires Solr 1.4+
        /// </summary>
        /// <param name="destinationCore"></param>
        /// <param name="indexDir"></param>
        /// <param name="indexDirs"></param>
        public MergeCommand(string destinationCore, IndexDir indexDir, params IndexDir[] indexDirs) {
            AddParameter("core", destinationCore);
            AddParameter("action", "mergeindexes");
            foreach (var d in new[] {indexDir}.Concat(indexDirs))
                AddParameter("indexDir", d.Dir);
        }

        /// <summary>
        /// Merge indexes using their core names to identify them.
        /// Requires Solr 3.3+
        /// </summary>
        /// <param name="destinationCore"></param>
        /// <param name="srcCore"></param>
        /// <param name="srcCores"></param>
        public MergeCommand(string destinationCore, SrcCore srcCore, params SrcCore[] srcCores) {
            AddParameter("core", destinationCore);
            AddParameter("action", "mergeindexes");
            foreach (var c in new[] {srcCore}.Concat(srcCores))
                AddParameter("srcCore", c.CoreName);
        }
    }
}
