using MbUnit.Framework;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;

namespace SolrNet.Tests
{
    [TestFixture]
    public class SolrQueryByDistanceTests
    {

        public class TestDocument { }

        public string Serialize(object q)
        {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }

        [Test]
        public void NullField_yields_null_query()
        {
            var q = new SolrQueryByDistance(null, 45.15, -93.85, 5);
            Assert.IsNull(Serialize(q));
        }

        [Test]
        public void NegativeDistance_yields_null_query()
        {
            var q = new SolrQueryByDistance("store", 45.15, -93.85, -100);
            Assert.IsNull(Serialize(q));
        }

        [Test]
        public void DefaultAccuracy_is_radius() 
        {
            var q = new SolrQueryByDistance("store", 45.15, -93.85, 5);
            Assert.AreEqual(CalculationAccuracy.Radius, q.Accuracy);
        }

        [Test]
        public void Basic() 
        {
            var q = new SolrQueryByDistance("store", 45.15, -93.85, 5);
            Assert.AreEqual("{!geofilt pt=45.15,-93.85 sfield=store d=5}",Serialize(q));
        }

        [Test]
        public void Basic_lower_accuracy()
        {
            var q = new SolrQueryByDistance("store", 45.15, -93.85, 5, CalculationAccuracy.BoundingBox);
            Assert.AreEqual("{!bbox pt=45.15,-93.85 sfield=store d=5}", Serialize(q));
        }
    }
}