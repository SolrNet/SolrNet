#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Text.RegularExpressions;

namespace SolrNet.Impl.QuerySerializers
{
    public class QueryByFieldSerializer : SingleTypeQuerySerializer<SolrQueryByField>
    {
        public override string Serialize(SolrQueryByField q)
        {
            if (q.FieldName == null || q.FieldValue == null)
            {
                return null;
            }

            return q.Quoted ? string.Format("{0}:({1})", EscapeSpaces(q.FieldName), Quote(q.FieldValue)) : string.Format("{0}:({1})", q.FieldName, q.FieldValue);
        }

        public static readonly Regex SpecialCharactersRx = new Regex("(\\+|\\-|\\&\\&|\\|\\||\\!|\\{|\\}|\\[|\\]|\\^|\\(|\\)|\\\"|\\~|\\:|\\;|\\\\|\\?|\\*|\\/)", RegexOptions.Compiled);

        public static string EscapeSpaces(string value) {
            return value.Replace(" ", @"\ ");
        }

        public static string Quote(string value)
        {
            string r = SpecialCharactersRx.Replace(value, "\\$1");
            if (r.IndexOf(' ') != -1 || r == "")
                r = string.Format("\"{0}\"", r);
            return r;
        }
    }
}