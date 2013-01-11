// 

using MbUnit.Framework;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrQueryByFieldRegexTests {

        public string Serialize(object q)
        {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }

        [Test]
        public void NullField_yields_null_query()
        {
            var q = new SolrQueryByFieldRegex(null, "11(.*?)");
            Assert.IsNull(Serialize(q));
        }

        [Test]
        public void NullValue_yields_null_query()
        {
            var q = new SolrQueryByFieldRegex("id", null);
            Assert.IsNull(Serialize(q));
        }

        [Test]
        public void Basic_test_creates_correctly()
        {
            var q = new SolrQueryByFieldRegex("id","11(.*?)");
            Assert.AreEqual("id:/11(.*?)/", Serialize(q));
        } 
        
        [Test]
        public void Basic_test_brackets_creates_correctly()
        {
            var q = new SolrQueryByFieldRegex("id", "[0-9]{5}");
            Assert.AreEqual("id:/[0-9]{5}/", Serialize(q));
        }

        [Test]
        public void Strip_slashes_creates_correctly()
        {
            var q = new SolrQueryByFieldRegex("id", "/11(.*?)/");
            Assert.AreEqual("id:/11(.*?)/", Serialize(q));
        }

        [Test]
        public void FieldNameWithSpaces()
        {
            var q = new SolrQueryByFieldRegex("field with spaces", "11(.*?)");
            Assert.AreEqual(@"field\ with\ spaces:/11(.*?)/", Serialize(q));
        }    
    }
}