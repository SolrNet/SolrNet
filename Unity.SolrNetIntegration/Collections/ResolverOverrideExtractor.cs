using Unity.Builder;
using Unity.Resolution;

namespace Unity.SolrNetIntegration.Collections
{
    public abstract class ResolverOverrideExtractor
    {
        public abstract ResolverOverride[] ExtractResolverOverrides(IBuilderContext context);
    }
}