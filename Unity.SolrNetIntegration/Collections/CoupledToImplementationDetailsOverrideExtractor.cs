
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.ObjectBuilder;
using Unity.Resolution;

namespace Unity.SolrNetIntegration.Collections
{
    public class CoupledToImplementationDetailsOverrideExtractor : ResolverOverrideExtractor
    {
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
            Justification = "Reviewed. Suppression is OK here.")]
        private static class FieldNames
        {
            /// <summary>resolverOverrides</summary>
            public const string ResolverOverrides = "resolverOverrides";
        }

        public override ResolverOverride[] ExtractResolverOverrides(IBuilderContext context)
        {
            // this method is tightly coupled to the implementation of IBuilderContext. It assumes that the 
            // class BuilderContext is used and that a field of type CompositeResolverOverride named 'resolverOverrides'
            // exists in that class
            ResolverOverride[] resolverOverrides = new ResolverOverride[0];

            if (!(context is BuilderContext))
            {
                return resolverOverrides;
            }

            FieldInfo field = typeof(BuilderContext).GetField(
                FieldNames.ResolverOverrides, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field == null)
            {
                return resolverOverrides;
            }

            var overrides = field.GetValue(context) as IEnumerable<ResolverOverride>;

            if (overrides != null)
            {
                resolverOverrides = overrides.ToArray();
            }

            return resolverOverrides;
        }
    }
}
