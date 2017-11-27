// 

using Xunit;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;

namespace SolrNet.Tests {
    
    public class SolrQueryByFieldRegexTests {

        public string Serialize(object q)
        {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }

        [Fact]
        public void NullField_yields_null_query()
        {
            var q = new SolrQueryByFieldRegex(null, "11(.*?)");
            Assert.Null(Serialize(q));
        }

        [Fact]
        public void NullValue_yields_null_query()
        {
            var q = new SolrQueryByFieldRegex("id", null);
            Assert.Null(Serialize(q));
        }

        [Fact]
        public void Basic_test_creates_correctly()
        {
            var q = new SolrQueryByFieldRegex("id","11(.*?)");
            Assert.Equal("id:/11(.*?)/", Serialize(q));
        } 
        
        [Fact]
        public void Basic_test_brackets_creates_correctly()
        {
            var q = new SolrQueryByFieldRegex("id", "[0-9]{5}");
            Assert.Equal("id:/[0-9]{5}/", Serialize(q));
        }

        [Fact]
        public void Strip_slashes_creates_correctly()
        {
            var q = new SolrQueryByFieldRegex("id", "/11(.*?)/");
            Assert.Equal("id:/11(.*?)/", Serialize(q));
        }

        [Fact]
        public void FieldNameWithSpaces()
        {
            var q = new SolrQueryByFieldRegex("field with spaces", "11(.*?)");
            Assert.Equal(@"field\ with\ spaces:/11(.*?)/", Serialize(q));
        }    
    }
}