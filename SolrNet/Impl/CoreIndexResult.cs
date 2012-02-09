﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl
{
    public class CoreIndexResult
    {
        /// <summary>
        /// Gets or sets the document count.
        /// </summary>
        public long DocumentCount { get; set; }

        /// <summary>
        /// Gets or sets the Index version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The number of Segments that exist for the index.
        /// </summary>
        public int SegmentCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is current.
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is optimized.
        /// </summary>
        public bool IsOptimized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the instance has deletions.
        /// </summary>
        /// <remarks>
        /// If an index has deletions, it may need to undergo a Optimization in order to fully remove any deleted documents.
        /// </remarks>
        public bool HasDeletions { get; set; }

        /// <summary>
        /// Gets or sets the directory implementation being used by Lucene.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Gets or sets the date the index was last modified.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// The physical Size of the Index.
        /// </summary>
        /// <remarks>
        /// Appears in Solr R4.0 and above.
        /// </remarks>
        public string Size { get; set; }

        /// <summary>
        /// the physical Size of the Index in bytes.
        /// </summary>
        /// <remarks>
        /// Appears in Solr R4.0 and above.
        /// </remarks>
        public long SizeInBytes { get; set; }
    }
}
