using System;
using System.Linq;
using System.Linq.Expressions;
using SolrNet;

namespace SampleSolrApp.Helpers {
    public static class IReadOnlyMappingManagerExtensions {
        public static string FieldName<T>(this IReadOnlyMappingManager mapper, Expression<Func<T, object>> property) {
            var propertyName = property.MemberName();
            return mapper.GetFields(typeof (T)).First(p => p.Key.Name == propertyName).Value;
        }
    }
}