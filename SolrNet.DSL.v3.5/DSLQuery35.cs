using System;
using System.Linq.Expressions;
using SolrNet.Exceptions;

namespace SolrNet.DSL.v3._5 {
	public class DSLQuery35<T> : DSLQuery<T>, IDSLQuery35<T> where T : ISolrDocument, new() {
		public DSLQuery35(ISolrConnection connection) : base(connection) {}

		public IDSLQueryRange<T> ByRange<RT>(Expression<Func<T, object>> f, RT from, RT to) {
			try {
				var exp = f.Body as UnaryExpression;
				var member = exp.Operand as MemberExpression;
				return ByRange(member.Member.Name, from, to);
			} catch (Exception e) {
				throw new SolrNetException("Please specify a field or property to query by range", e);
			}
		}

		public IDSLQueryRange<T> ByRange(Expression<Func<T, object>> f) {
			throw new NotImplementedException();
		}

		public IDSLQueryBy<T> By(Expression<Func<T, object>> f) {
			try {
				var exp = f.Body as UnaryExpression;
				var member = exp.Operand as MemberExpression;
				return By(member.Member.Name);
			} catch (Exception e) {
				throw new SolrNetException("Please specify a field or property to query by range", e);
			}
		}
	}
}