﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace SolrNet.Impl.ResponseParsers
{
    public class InterestingTermsResponseParser<T> : ISolrMoreLikeThisHandlerResponseParser<T>
    {
        private static readonly Func<XElement, KeyValuePair<string, float>> extractList = 
            x => new KeyValuePair<string, float>(x.Value.Trim(), 0.0f);

        private static readonly Func<XElement, KeyValuePair<string, float>> extractDetails =
            x => new KeyValuePair<string, float>((string)x.Attribute("name"), float.Parse(x.Value, CultureInfo.InvariantCulture.NumberFormat));

        public void Parse(System.Xml.Linq.XDocument xml, IAbstractSolrQueryResults<T> results)
        {
            if (results is ISolrMoreLikeThisQueryResults<T>)
            {
                this.Parse(xml, (ISolrMoreLikeThisQueryResults<T>)results);
            }
        }

        public void Parse(System.Xml.Linq.XDocument xml, ISolrMoreLikeThisQueryResults<T> results)
        {
            Func<XElement, KeyValuePair<string, float>> extract;

            var it = xml.Element("response").Elements("arr").FirstOrDefault(e => (string)e.Attribute("name") == "interestingTerms");

            if (it == null)
            {
                it = xml.Element("response").Elements("lst").FirstOrDefault(e => (string)e.Attribute("name") == "interestingTerms");
                
                if (it == null)
                {
                    results.InterestingTerms = new List<KeyValuePair<string, float>>();
                    return;
                }

                extract = extractDetails;
            }
            else
            {
                extract = extractList;
            }

            results.InterestingTerms = it.Elements().Select(x => extract(x)).ToList();
        }
    }
}
