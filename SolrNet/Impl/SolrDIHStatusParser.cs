using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl {
    /// <summary>
    /// Parses a Solr DIH Status xml document into a strongly typed
    /// <see cref="SolrDIHStatus"/> object.
    /// </summary>
    public class SolrDIHStatusParser : ISolrDIHStatusParser {
        /// <summary>
        /// Parses the specified Solr DIH Status xml.
        /// </summary>
        /// <param name="solrDIHStatusXml">The Solr DIH Status xml to parse.</param>
        /// <returns>A strongly styped representation of the Solr HDI Status xml.</returns>
        public SolrDIHStatus Parse(XDocument solrDIHStatusXml) {
            var result = new SolrDIHStatus();

            if (solrDIHStatusXml == null) 
                return result;

            foreach (var fieldNode in solrDIHStatusXml.Element("response").Elements("str")) {
                switch (fieldNode.Attribute("name").Value) {
                    case "status":
                        switch (fieldNode.Value) {
                            case "idle":
                                result.Status = DIHStatus.IDLE;
                                break;
                            case "busy":
                                result.Status = DIHStatus.BUSY;
                                break;
                        }
                        break;
                    case "importResponse":
                        result.ImportResponse = fieldNode.Value;
                        break;
                }
            }

            foreach (var fieldNode in solrDIHStatusXml.XPathSelectElement("//lst[@name='statusMessages']").Elements()) {
                DateTime tempDate;
                string[] tempTimeSpanSplit;

                switch (fieldNode.Attribute("name").Value) {
                    case "Time Elapsed":
                        tempTimeSpanSplit = fieldNode.Value.Split(':');
                        if (tempTimeSpanSplit.Length == 3)
                            result.TimeElapsed = new TimeSpan(0, Convert.ToInt32(tempTimeSpanSplit[0]), Convert.ToInt32(tempTimeSpanSplit[1]), Convert.ToInt32(tempTimeSpanSplit[2].Split('.')[0]), Convert.ToInt32(tempTimeSpanSplit[2].Split('.')[1]));
                        
                        break;
                    case "Total Requests made to DataSource":
                        result.TotalRequestToDataSource = Convert.ToInt32(fieldNode.Value);

                        break;
                    case "Total Rows Fetched":
                        result.TotalRowsFetched = Convert.ToInt32(fieldNode.Value);

                        break;
                    case "Total Documents Processed":
                        result.TotalDocumentsProcessed = Convert.ToInt32(fieldNode.Value);

                        break;
                    case "Total Documents Skipped":
                        result.TotalDocumentsSkipped = Convert.ToInt32(fieldNode.Value);

                        break;
                    case "Full Dump Started":
                        if (DateTime.TryParse(fieldNode.Value, out tempDate)) 
                            result.FullDumpStarted = tempDate;
                        
                        break;
                    case "Committed":
                        if (DateTime.TryParse(fieldNode.Value, out tempDate)) 
                            result.Committed = tempDate;

                        break;
                    case "Optimized":
                        if (DateTime.TryParse(fieldNode.Value, out tempDate))
                            result.Optimized = tempDate;

                        break;
                    case "Total Documents Failed":
                        result.TotalDocumentsFailed = Convert.ToInt32(fieldNode.Value);

                        break;
                    case "Time taken ":
                        tempTimeSpanSplit = fieldNode.Value.Split(':');
                        if (tempTimeSpanSplit.Length == 3)
                            result.TimeTaken = new TimeSpan(0, Convert.ToInt32(tempTimeSpanSplit[0]), Convert.ToInt32(tempTimeSpanSplit[1]), Convert.ToInt32(tempTimeSpanSplit[2].Split('.')[0]), Convert.ToInt32(tempTimeSpanSplit[2].Split('.')[1]));

                        break;
                    case "":
                        result.Summary = fieldNode.Value;
                        break;
                }
            }

            return result;
        }
    }
}
