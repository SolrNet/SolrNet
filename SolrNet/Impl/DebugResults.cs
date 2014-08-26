using System.Collections.Generic;

namespace SolrNet.Impl
{
    /// <summary>
    /// Debug results model
    /// </summary>
    public class DebugResults
    {
        /// <summary>
        /// Timing results
        /// </summary>
        public TimingResults Timing;

        /// <summary>
        /// Explain results
        /// </summary>
        public IDictionary<string, string> Explain;

        /// <summary>
        /// Structured explain results
        /// </summary>
        public IEnumerable<ExplainationResult> ExplainStructured;

        /// <summary>
        /// Parsed query
        /// </summary>
        public string ParsedQuery;

        /// <summary>
        /// Parsed query string
        /// </summary>
        public string ParsedQueryString;

        /// <summary>
        /// DebugResults initializer
        /// </summary>
        public DebugResults()
        {
            Timing = new TimingResults();
            Explain = new Dictionary<string, string>();
            ParsedQuery = string.Empty;
            ParsedQueryString = string.Empty;
            ExplainStructured = new List<ExplainationResult>();
        }
    }

    /// <summary>
    /// Timing results model
    /// </summary>
    public class TimingResults
    {
        /// <summary>
        /// Elapsed time
        /// </summary>
        public double TotalTime;

        /// <summary>
        /// Time results for preparing stage
        /// </summary>
        public IDictionary<string, double> Prepare;

        /// <summary>
        /// Time results for processing stage
        /// </summary>
        public IDictionary<string, double> Process;

        /// <summary>
        /// TimingResults initializer
        /// </summary>
        public TimingResults()
        {
            Prepare = new Dictionary<string, double>();
            Process = new Dictionary<string, double>();
        }
    }

    /// <summary>
    /// Explaination results model
    /// </summary>
    public class ExplainationResult
    {
        /// <summary>
        /// Explaination key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Explaination details
        /// </summary>
        public ExplainationModel Explaination { get; set; }
    }

    /// <summary>
    /// Explaination details model
    /// </summary>
    public class ExplainationModel
    {
        /// <summary>
        /// Explaination "match" flag
        /// </summary>
        public bool Match { get; set; }

        /// <summary>
        /// Explaination "value" field
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Explaination description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Explaination details collection
        /// </summary>
        public ICollection<ExplainationModel> Details { get; set; }

        /// <summary>
        /// ExplainationModel initializer
        /// </summary>
        public ExplainationModel()
        {
            Details = new List<ExplainationModel>();
        }
    }
}