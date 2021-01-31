using System;
using Hedgehog;
using SolrNet.Utils;
using Xunit;
using Xunit.Abstractions;

namespace SolrNet.Tests
{
    public class UriValidatorTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public UriValidatorTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }
        
        [Fact]
        public void UriLength_Equivalent()
        {
            var property =
                from uri in Property.ForAll(GenX.uri)
                let ub = new UriBuilder(uri)
                let expected = ub.Uri.ToString().Length
                let actual = UriValidator.UriLength(ub)
                select AssertEqual(expected: expected, actual: actual);

            Property.Check(property);
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
