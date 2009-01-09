namespace SolrNet.Utils {
    public static class ReadOnlyMappingManagerFactory {
        public static Func.Function<IReadOnlyMappingManager> Create = () => DefaultReadOnlyMappingManager.Instance;

        private sealed class DefaultReadOnlyMappingManager {
            private static readonly IReadOnlyMappingManager instance = new MemoizingMappingManager(new AttributesMappingManager());

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static DefaultReadOnlyMappingManager() { }

            private DefaultReadOnlyMappingManager() { }

            public static IReadOnlyMappingManager Instance {
                get { return instance; }
            }
        }
    }
}