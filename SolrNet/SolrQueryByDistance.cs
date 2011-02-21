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
using System.Globalization;
using SolrNet.Impl;

namespace SolrNet {

    ///<summary>
    /// [SOLR 4.0 ONLY] Defines the level of accuracy applied for distance calculations.
    ///</summary>
    public enum CalculationAccuracy {
        ///<summary>
        /// Highest accuracy but can have a performance hit.
        ///</summary>
        Radius,
        ///<summary>
        /// Less accurancy (as it draws a bounding box around the points) but faster.
        ///</summary>
        BoundingBox
    }

    ///<summary>
    /// [SOLR 4.0 ONLY] Retreives entries from the index based on distance from a point
    ///</summary>
    public class SolrQueryByDistance : ISelfSerializingQuery
    {
        #region Properties
        
        public string FieldName { get; private set; }
        
        public double PointLatitude { get; private set; }
        
        public double PointLongitude { get; private set; }
        
        public int DistanceFromPoint { get; private set; }

        public CalculationAccuracy Accuracy { get; private set; }
        
        #endregion

        #region Ctor

        public SolrQueryByDistance(string fieldName, double pointLatitude, double pointLongitude, int distance) : this(fieldName, pointLatitude, pointLongitude, distance, CalculationAccuracy.Radius ) {}

        public SolrQueryByDistance(string fieldName, double pointLatitude, double pointLongitude, int distance, CalculationAccuracy accurancy) 
        {
            if (string.IsNullOrEmpty(fieldName)) { throw new ArgumentNullException(); }
            if(distance <= 0) { throw new ArgumentOutOfRangeException("Distance must be greater than zero."); }

            FieldName = fieldName;
            PointLatitude = pointLatitude;
            PointLongitude = pointLongitude;
            DistanceFromPoint = distance;
            Accuracy = accurancy;
        }

        #endregion

        public string Query {
            get {
                var prefix = Accuracy == CalculationAccuracy.Radius ? "{!geofilt" : "{!bbox";
                return (prefix + " pt=" + PointLatitude.ToString(CultureInfo.InvariantCulture) + "," + PointLongitude.ToString(CultureInfo.InvariantCulture) + " sfield=" + FieldName + " d=" + DistanceFromPoint + "}");        
            }
        }
    }
}