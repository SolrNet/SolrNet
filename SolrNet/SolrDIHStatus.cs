using System;

namespace SolrNet {
    ///<Summary>
    /// Data import handler status
    ///</Summary>
    public enum DIHStatus {
        ///<Summary>
        /// Idle 
        ///</Summary>
        IDLE,

        ///<Summary>
        /// Busy
        ///</Summary>
        BUSY
    }


    /// <summary>
    /// Represents a Solr data import handler status.
    /// </summary>
    public class SolrDIHStatus {
        /// <summary>
        /// Busy or idle
        /// </summary>
        public DIHStatus Status { get; set; }
        public string ImportResponse { get; set;}
        public TimeSpan TimeElapsed { get; set; }
        public int TotalRequestToDataSource { get; set; }
        public int TotalRowsFetched { get; set; }
        public int TotalDocumentsProcessed { get; set; }
        public int TotalDocumentsSkipped { get; set; }
        public int TotalDocumentsFailed { get; set; }
        public DateTime FullDumpStarted { get; set; }
        public string Summary { get; set; }
        public DateTime Committed { get; set; }
        public DateTime Optimized { get; set; }
        public TimeSpan TimeTaken { get; set; }
    }
}
