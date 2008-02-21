using System;
using System.Linq.Expressions;

namespace SolrNet.DSL.v3._5 {
	public interface IDSLQuery35<T> : IDSLQuery<T> where T : ISolrDocument, new() {
		IDSLQueryRange<T> ByRange<RT>(Expression<Func<T, object>> f, RT from, RT to);
		IDSLQueryBy<T> By(Expression<Func<T, object>> f);
	}
}