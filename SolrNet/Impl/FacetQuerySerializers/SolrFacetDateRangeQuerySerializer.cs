using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SolrNet.Utils;

namespace SolrNet.Impl.FacetQuerySerializers
{
    public class SolrFacetDateRangeQuerySerializer : SingleTypeFacetQuerySerializer<SolrFacetDateRangeQuery>
    {
        private static readonly Regex localParamsRx = new Regex(@"\{![^\}]+\}", RegexOptions.Compiled);

        private readonly ISolrFieldSerializer fieldSerializer;

        public SolrFacetDateRangeQuerySerializer(ISolrFieldSerializer fieldSerializer)
        {
            this.fieldSerializer = fieldSerializer;
        }

        public string SerializeSingle(object o)
        {
            return fieldSerializer.Serialize(o).First().FieldValue;
        }

        public override IEnumerable<KeyValuePair<string, string>> Serialize(SolrFacetDateRangeQuery q)
        {
            var fieldWithoutLocalParams = localParamsRx.Replace(q.Field, "");
            yield return KV.Create("facet.range", q.Field);
            yield return KV.Create(string.Format("f.{0}.facet.range.start", fieldWithoutLocalParams), SerializeSingle(q.Start));
            yield return KV.Create(string.Format("f.{0}.facet.range.end", fieldWithoutLocalParams), SerializeSingle(q.End));
            yield return KV.Create(string.Format("f.{0}.facet.range.gap", fieldWithoutLocalParams), q.Gap);
            if (q.HardEnd.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.range.hardend", fieldWithoutLocalParams), SerializeSingle(q.HardEnd.Value));
            if (q.MinCount.HasValue)
                yield return KV.Create(string.Format("f.{0}.facet.mincount", fieldWithoutLocalParams), q.MinCount.ToString());
            if (q.Other != null && q.Other.Count > 0)
                foreach (var o in q.Other)
                    yield return KV.Create(string.Format("f.{0}.facet.range.other", fieldWithoutLocalParams), o.ToString());
            if (q.Include != null && q.Include.Count > 0)
                foreach (var i in q.Include)
                    yield return KV.Create(string.Format("f.{0}.facet.range.include", fieldWithoutLocalParams), i.ToString());
        }
    }
}