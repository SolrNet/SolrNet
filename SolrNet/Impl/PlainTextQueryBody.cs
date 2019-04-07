using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace SolrNet.Impl
{
    /// <summary>
    /// A plain text query body implementation, for sending text to
    /// MoreLikeThis handler if necessary.
    /// </summary>
    public class PlainTextQueryBody : ISolrQueryBody
    {

        private string Text { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">The text to send in the query.</param>
        public PlainTextQueryBody(string text)
        {
            this.Text = text;
        }


        public string serialize()
        {
            return Text;
        }

        public string mimeType => MediaTypeNames.Text.Plain;
    }
}
