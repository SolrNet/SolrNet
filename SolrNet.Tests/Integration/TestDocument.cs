using System;
using System.Collections.Generic;

namespace SolrNet.Tests.Integration {
    public class TestDocument : ISolrDocument {
        [SolrUniqueKey]
        [SolrField("id")]
        public int Id { get; set; }

        [SolrField("make")]
        public string Make { get; set; }

        [SolrField("model")]
        public string Model { get; set; }

        [SolrField("series")]
        public ICollection<int> Series { get; set;}

        [SolrField("style")]
        public string Style { get; set; }

        [SolrField("category")]
        public string Category { get; set; }

        [SolrField("year")]
        public int Year { get; set; }

        [SolrField("payment")]
        public decimal Price { get; set; }

        [SolrField("months")]
        public int Months { get; set; }

        [SolrField("region")]
        public int Region { get; set; }

        [SolrField("miles")]
        public int Miles { get; set; }

        [SolrField("state")]
        public string State { get; set; }

        [SolrField("visible")]
        public bool Visible { get; set; }

        [SolrField("photo")]
        public bool Photo { get; set; }

        //[SolrField("transferring")]
        //public bool Transferring { get; set; }

        [SolrField("sponsored")]
        public bool Sponsored { get; set; }

        [SolrField("hotdeal")]
        public bool Hotdeal { get; set; }

        [SolrField("featured")]
        public bool Featured { get; set; }

        [SolrField("basicview")]
        public string BasicView { get; set; }

        [SolrField("advancedview")]
        public string AdvancedView { get; set; }

        [SolrField("fecha")]
        public DateTime? Fecha { get; set; }
    }

}