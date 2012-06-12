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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers
{
	/// <summary>
	/// Parses TermVector results from a query response
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class TermVectorResultsParser<T> : ISolrResponseParser<T>
	{
		public void Parse(XDocument xml, AbstractSolrQueryResults<T> results)
		{
			results.Switch(query: r => Parse(xml, r),
						   moreLikeThis: F.DoNothing);
		}

		public void Parse(XDocument xml, SolrQueryResults<T> results)
		{
			var rootNode = xml.XPathSelectElement("response/lst[@name='termVectors']");
			if (rootNode != null)
				results.TermVectorResults = ParseDocuments(rootNode);
		}

		/// <summary>
		/// Parses term vector results
		/// </summary>
		/// <param name="rootNode"></param>
		/// <returns></returns>
		public TermVectorResults ParseDocuments(XElement rootNode)
		{
			var r = new TermVectorResults();
			var docNodes = rootNode.Elements("lst");
			foreach (var docNode in docNodes)
			{
				var docNodeName = docNode.Attribute("name").Value;

				if (docNodeName == "warnings") 
				{
					// TODO: warnings
				}
				if (docNodeName == "uniqueKeyFieldName")
				{
					//TODO: support for unique key field name
				}
				else 
				{
					var doc = ParseDoc(docNode);

					r.Add(doc);
				}
				
			}
			return r;
		}

		private TermVectorDocumentResult ParseDoc(XElement docNode)
		{
			var r = new TermVectorDocumentResult();
			var fieldNodes = docNode.Elements();
			
			foreach (var fieldNode in fieldNodes) {
				string fieldName = fieldNode.Attribute("name").Value;

				if(fieldName == "uniqueKey")
				{
					r.UniqueKey = fieldNode.Value;
				}
				else 
				{
					var fields = ParseField(fieldNode);

					foreach (var fieldVector in fields) {
						r.TermVector.Add(fieldVector);
					}
				}
			}

			return r;
		}
		
		private ICollection<TermVectorResult> ParseField(XElement fieldNode)
		{
			var r = new Collection<TermVectorResult>();
			
			foreach (var termNode in fieldNode.Elements()) {
				var term = ParseTerm(termNode, fieldNode.Attribute("name").Value);

				r.Add(term);
			}

			return r;
		}

		private TermVectorResult ParseTerm(XElement termNode, string fieldName)
		{
			var term = new TermVectorResult();

			foreach (var valueNode in termNode.Elements())
			{

				term.Field = fieldName;
				term.Term = termNode.Attribute("name").Value;

				string key = valueNode.Attribute("name").Value;
				string value = valueNode.Value;
				switch (key)
				{
					case "tf":
						term.Tf = int.Parse(value);
						break;
					case "df":
						term.Df = int.Parse(value);
						break;
					case "tf-idf":
						term.Tf_Idf = double.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
						break;
					case "offsets":
						term.Offsets = ParseOffsets(valueNode);
						break;
					case "positions":
						term.Positions = ParsePositions(valueNode);
						break;
				}
			}

			return term;
		}

		private IList<int> ParsePositions(XElement valueNode)
		{
			var positions = new List<int>();

			foreach(var p in valueNode.Elements())
			{
				positions.Add(int.Parse(p.Value));
			}

			return positions;
		}

		private IList<Offset> ParseOffsets(XElement valueNode)
		{
			var offsets = from e in valueNode.Elements()
					   where e.Attribute("name").Value == "start"
					   select new Offset
					   {
						   Start = int.Parse(e.Value),
						   End = int.Parse(((XElement)e.NextNode).Value)
					   };

			return offsets.ToList();
		}
	}
}