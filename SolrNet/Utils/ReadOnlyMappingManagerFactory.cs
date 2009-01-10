namespace SolrNet.Utils {
    public static class ReadOnlyMappingManagerFactory {
        /// <summary>
        /// Default factory method.
        /// Returns a singleton of <see cref="MemoizingMappingManager"/> wrapping a <see cref="AttributesMappingManager"/>
        /// </summary>
        public static readonly Func.Function<IReadOnlyMappingManager> DefaultCreate = () => DefaultReadOnlyMappingManager.Instance;
        public static Func.Function<IReadOnlyMappingManager> Create = DefaultCreate;

        /// <summary>
        /// Default <see cref="IReadOnlyMappingManager"/> singleton
        /// Implementation: <see cref="MemoizingMappingManager"/> wrapping a <see cref="AttributesMappingManager"/>
        /// </summary>
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