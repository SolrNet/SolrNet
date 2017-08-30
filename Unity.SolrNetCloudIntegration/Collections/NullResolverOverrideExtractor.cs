using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.SolrNetCloudIntegration.Collections
{
    public class NullResolverOverrideExtractor : ResolverOverrideExtractor
    {
        public override ResolverOverride[] ExtractResolverOverrides(IBuilderContext context)
        {
            return new ResolverOverride[0];
        }
    }
}
