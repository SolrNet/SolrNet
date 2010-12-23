using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SolrNet.Exceptions;
using SolrNet.Schema;

namespace SolrNet.DIH
{
    /// <summary>
    /// Parses a Solr DIH Status xml document into a strongly typed
    /// <see cref="SolrDIHStatus"/> object.
    /// </summary>
    public class SolrDIHStatusParser : ISolrDIHStatusParser
    {
        /// <summary>
        /// Parses the specified Solr DIH Status xml.
        /// </summary>
        /// <param name="solrDIHStatusXml">The Solr DIH Status xml to parse.</param>
        /// <returns>A strongly styped representation of the Solr HDI Status xml.</returns>
        public SolrDIHStatus Parse(System.Xml.XmlDocument solrDIHStatusXml)
        {
            var result = new SolrDIHStatus();

            if (solrDIHStatusXml == null) return result;

            foreach (XmlNode fieldNode in solrDIHStatusXml.SelectNodes("/response/str"))
            {
                switch (fieldNode.Attributes["name"].Value)
                {
                    case "status":
                        switch (fieldNode.InnerText)
                        {
                            case "idle":
                                result.Status = DIHStatus.IDLE;
                                break;
                            case "busy":
                                result.Status = DIHStatus.BUSY;
                                break;
                        }
                        break;
                    case "importResponse":
                        result.ImportResponse = fieldNode.InnerText;
                        break;
                }
            }

            foreach (XmlNode fieldNode in solrDIHStatusXml.SelectSingleNode("//response/lst[@name='statusMessages']"))
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
