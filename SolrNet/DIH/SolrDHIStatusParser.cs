using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SolrNet.Exceptions;
using SolrNet.Schema;

namespace SolrNet.DHI
{
    /// <summary>
    /// Parses a Solr DHI Status xml document into a strongly typed
    /// <see cref="SolrDHIStatus"/> object.
    /// </summary>
    public class SolrDHIStatusParser : ISolrDHIStatusParser
    {
        /// <summary>
        /// Parses the specified Solr DHI Status xml.
        /// </summary>
        /// <param name="solrDHIStatusXml">The Solr DHI Status xml to parse.</param>
        /// <returns>A strongly styped representation of the Solr HDI Status xml.</returns>
        public SolrDHIStatus Parse(System.Xml.XmlDocument solrDHIStatusXml)
        {
            var result = new SolrDHIStatus();

            if (solrDHIStatusXml == null) return result;

            foreach (XmlNode fieldNode in solrDHIStatusXml.SelectNodes("/response/str"))
            {
                switch (fieldNode.Attributes["name"].Value)
                {
                    case "status":
                        switch (fieldNode.InnerText)
                        {
                            case "idle":
                                result.Status = DHIStatus.IDLE;
                                break;
                            case "busy":
                                result.Status = DHIStatus.BUSY;
                                break;
                        }
                        break;
                    case "importResponse":
                        result.ImportResponse = fieldNode.InnerText;
                        break;
                }
            }

            foreach (XmlNode fieldNode in solrDHIStatusXml.SelectSingleNode("//response/lst[@name='statusMessages']"))
            {
                DateTime tempDate;
                string[] tempTimeSpanSplit;

                switch (fieldNode.Attributes["name"].Value)
                {
                    case "Time Elapsed":
                        tempTimeSpanSplit = fieldNode.InnerText.Split(':');
                        if (tempTimeSpanSplit.Length == 3)
                            result.TimeElapsed = new TimeSpan(0, Convert.ToInt32(tempTimeSpanSplit[0]), Convert.ToInt32(tempTimeSpanSplit[1]), Convert.ToInt32(tempTimeSpanSplit[2].Split('.')[0]), Convert.ToInt32(tempTimeSpanSplit[2].Split('.')[1]));
                        
                        break;
                    case "Total Requests made to DataSource":
                        result.TotalRequestToDataSource = Convert.ToInt32(fieldNode.InnerText);

                        break;
                    case "Total Rows Fetched":
                        result.TotalRowsFetched = Convert.ToInt32(fieldNode.InnerText);

                        break;
                    case "Total Documents Processed":
                        result.TotalDocumentsProcessed = Convert.ToInt32(fieldNode.InnerText);

                        break;
                    case "Total Documents Skipped":
                        result.TotalDocumentsSkipped = Convert.ToInt32(fieldNode.InnerText);

                        break;
                    case "Full Dump Started":
                        if (DateTime.TryParse(fieldNode.InnerText, out tempDate)) result.FullDumpStarted = tempDate;
                        
                        break;
                    case "Committed":
                        if (DateTime.TryParse(fieldNode.InnerText, out tempDate)) result.Committed = tempDate;

                        break;
                    case "Optimized":
                        if (DateTime.TryParse(fieldNode.InnerText, out tempDate)) result.Optimized = tempDate;

                        break;
                    case "Total Documents Failed":
                        result.TotalDocumentsFailed = Convert.ToInt32(fieldNode.InnerText);

                        break;
                    case "Time taken ":
                        tempTimeSpanSplit = fieldNode.InnerText.Split(':');
                        if (tempTimeSpanSplit.Length == 3)
                            result.TimeTaken = new TimeSpan(0, Convert.ToInt32(tempTimeSpanSplit[0]), Convert.ToInt32(tempTimeSpanSplit[1]), Convert.ToInt32(tempTimeSpanSplit[2].Split('.')[0]), Convert.ToInt32(tempTimeSpanSplit[2].Split('.')[1]));

                        break;
                    case "":
                        result.Summary = fieldNode.InnerText;
                        break;
                }
            }


            return result;
        }
    }
}
