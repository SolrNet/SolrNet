using System;
using Xunit;

namespace SolrNet.Tests {
    
    public class SolrQueryByDistanceTests {
        [Fact]
        public void NullField_should_throw() {
            Assert.Throws<ArgumentNullException>(() => new SolrQueryByDistance(null, 45.15, -93.85, 5));
        }

        [Fact]
           public void NegativeDistance_should_throw() {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SolrQueryByDistance("store", 45.15, -93.85, -100));
        }

        [Fact]
        public void DefaultAccuracy_is_radius() {
            var q = new SolrQueryByDistance("store", 45.15, -93.85, 5);
            Assert.Equal(CalculationAccuracy.Radius, q.Accuracy);
        }

        [Fact]
        public void Basic() {
            var q = new SolrQueryByDistance("store", 45.15, -93.85, 5.2);
            Assert.Equal("{!geofilt pt=45.15,-93.85 sfield=store d=5.2}", q.Query);
        }

        [Fact]
        public void Basic_lower_accuracy() {
            var q = new SolrQueryByDistance("store", 45.15, -93.85, 5, CalculationAccuracy.BoundingBox);
            Assert.Equal("{!bbox pt=45.15,-93.85 sfield=store d=5}", q.Query);
        }
    }
}