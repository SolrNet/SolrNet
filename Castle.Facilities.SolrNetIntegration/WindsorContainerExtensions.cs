using Castle.Windsor;

namespace Castle.Facilities.SolrNetIntegration {
    /// <summary>
    /// Adds extensions for to improve readability and discoverability
    /// </summary>
    public static class WindsorContainerExtensions {
        /// <summary>
        /// /// Fetches an existing facility from windsor or adds a new one if none exists yet.
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static SolrNetFacility GetOrAddSolrNetFacility(this IWindsorContainer @this) {
            return SolrNetFacility.GetFromOrAddToContainer(@this);
        }
    }
}