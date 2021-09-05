using System;
using Hedgehog;
using Hedgehog.Linq;
using SolrNet.Utils;
using Xunit;
using Xunit.Abstractions;
using Property = Hedgehog.Linq.Property;

namespace SolrNet.Tests
{
    public class UriValidatorTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public UriValidatorTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }
        
        [Fact(Skip = "Fails with 'ws' scheme. Hangs when trying to filter gen. Prob will need to rewrite in FsCheck")]
        public void UriLength_Equivalent()
        {
            var property =
                from uri in Property.ForAll(GenX.uri.Where(u => u.Scheme is "http" or "https"))
                let ub = new UriBuilder(uri)
                let expected = ub.Uri.ToString().Length
                let actual = UriValidator.UriLength(ub)
                select AssertEqual(expected: expected, actual: actual);

            property.Check();
        }

        void AssertEqual(int expected, int actual)
        {
            // Hedgehog swallows xunit output so we need this
            if (actual != expected)
            {
                testOutputHelper.WriteLine($"Expected {expected} but was {actual}");
                throw new Exception();
            }
        }
    }
}
