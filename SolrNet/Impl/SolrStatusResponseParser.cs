using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl {
    /// <summary>
    /// Parses the results of a Core Status command.
    /// </summary>
    public class SolrStatusResponseParser : ISolrStatusResponseParser {
        /// <summary>
        /// Parses the results of a Core Status command.
        /// </summary>
        /// <param name="xml">The XML Document to parse.</param>
        /// <returns></returns>
        public List<CoreResult> Parse(XDocument xml) {
            var statusNode = xml.XPathSelectElement("response/lst[@name='status']");
            if (statusNode == null || !statusNode.HasElements)
                return new List<CoreResult>();

            var results = statusNode.Elements().Select(ParseCore).ToList();
            return results;
        }

        /// <summary>
        /// Parses the details of a <see cref="CoreResult"/> that could be parsed.
        /// </summary>
        /// <param name="node">The node to inspect.</param>
        /// <returns>The parsed <see cref="CoreResult"/>.</returns>
        private static CoreResult ParseCore(XElement node) {
            var core = new CoreResult();

            foreach (var propNode in node.Elements()) {
                var nodeValue = propNode.Value;

                switch (propNode.Attribute("name").Value.ToLower()) {
                    case "name":
                        if (!string.IsNullOrEmpty(nodeValue))
                            core.Name = nodeValue;
                        break;
                    case "isdefaultcore":
                        core.IsDefaultCore = bool.Parse(nodeValue);
                        break;
                    case "instancedir":
                        if (!string.IsNullOrEmpty(nodeValue))
                            core.InstanceDir = nodeValue;
                        break;
                    case "datadir":
                        if (!string.IsNullOrEmpty(nodeValue))
                            core.DataDir = nodeValue;
                        break;
                    case "starttime":
                        if (!string.IsNullOrEmpty(nodeValue))
                            core.StartTime = DateTime.Parse(nodeValue);
                        break;
                    case "uptime":
                        if (!string.IsNullOrEmpty(nodeValue))
                            core.Uptime = long.Parse(nodeValue);
                        break;
                    case "index":
                        // Parse all Index responses.
                        core.Index = ParseCoreIndex(propNode);
                        break;
                }
            }

            return core;
        }

        /// <summary>
        /// Parses the details of the index for a Core.
        /// </summary>
        /// <param name="node">The node to parse.</param>
        /// <returns>The <see cref="CoreIndexResult"/> that was parsed.</returns>
        private static CoreIndexResult ParseCoreIndex(XElement node) {
            var coreIndex = new CoreIndexResult();

            foreach (var indexNode in node.Elements()) {
                var nodeValue = indexNode.Value;

                switch (indexNode.Attribute("name").Value.ToLower()) {
                    case "numdocs":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.SearchableDocumentCount = long.Parse(nodeValue);
                        break;
                    case "maxdoc":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.TotalDocumentCount = long.Parse(nodeValue);
                        break;
                    case "version":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.Version = long.Parse(nodeValue);
                        break;
                    case "segmentcount":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.SegmentCount = int.Parse(nodeValue);
                        break;
                    case "current":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.IsCurrent = bool.Parse(nodeValue);
                        break;
                    case "hasdeletions":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.HasDeletions = bool.Parse(nodeValue);
                        break;
                    case "optimized":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.IsOptimized = bool.Parse(nodeValue);
                        break;
                    case "directory":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.Directory = nodeValue;
                        break;
                    case "lastmodified":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.LastModified = DateTime.Parse(nodeValue);
                        break;
                    case "sizeinbytes":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.SizeInBytes = long.Parse(nodeValue);
                        break;
                    case "size":
                        if (!string.IsNullOrEmpty(nodeValue))
                            coreIndex.Size = nodeValue;
                        break;
                }
            }

            return coreIndex;
        }
    }
}