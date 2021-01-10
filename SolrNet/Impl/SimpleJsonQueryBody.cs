using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace SolrNet.Impl
{
    /// <summary>
    /// A simple JSON query body implementation. Takes the JSON as a string
    /// at construction time, returns it when serializing.
    /// </summary>
    public class SimpleJsonQueryBody : ISolrQueryBody
    {
        public static readonly string ApplicationJson = "application/json";

        private string Json { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="json">The JSON string to send in the query.</param>
        public SimpleJsonQueryBody(string json)
        {
            this.Json = json;
        }

        /// <inheritdoc />
        public string Serialize()
        {
            return Json;
        }

        /// <inheritdoc />
        public string MimeType => ApplicationJson;
    }
}
