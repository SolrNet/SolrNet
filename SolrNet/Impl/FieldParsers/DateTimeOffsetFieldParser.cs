﻿#region license
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
using System.Xml.Linq;

namespace SolrNet.Impl.FieldParsers {
    /// <summary>
    /// Parses <see cref="DateTimeOffset"/> fields
    /// </summary>
    public class DateTimeOffsetFieldParser : ISolrFieldParser {
        public bool CanHandleSolrType(string solrType) {
            return solrType == "date";
        }

        public bool CanHandleType(Type t) {
            return t == typeof (DateTimeOffset);
        }

        public object Parse(XElement field, Type t) {
            return Parse(field.Value);
        }

        public static DateTimeOffset Parse(string s) {
            var t = DateTimeFieldParser.ParseDate(s);
            return new DateTimeOffset(t, TimeSpan.Zero);
        }
    }
}