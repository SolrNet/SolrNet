using System;
using System.Collections.Generic;
using System.Text;

namespace SolrNet.DIH
{
    ///<Summary>
    /// DIHStatus
    ///</Summary>
    public enum DIHStatus
    {
        ///<Summary>
        /// Idle Status
        ///</Summary>
        IDLE,

        ///<Summary>
        /// Busy Status
        ///</Summary>
        BUSY
    }


    /// <summary>
    /// Represents a Solr DIH Status.
    /// </summary>
    public class SolrDIHStatus
    {
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
