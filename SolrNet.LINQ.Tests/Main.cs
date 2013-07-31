using Fuchu;

namespace SolrNet.LINQ.Tests {
    internal class Main1 {
        public static void Main() {
            var test = Fuchu.MbUnit.MbUnitTestToFuchu(typeof (QueryableSolrNetTester));
            test.Run();
        }
    }
}