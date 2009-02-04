using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SampleSolrApp.Helpers {
    public static class ExpressionExtensions {
        public static string MemberName(Expression exp) {
            string r = null;
            var visitor = new ExpressionVisitorV {
                VisitMember = e => { r = e.Member.Name; },
                VisitMethodCall = e => { r = e.Method.Name; },
            };
            visitor.VisitUnary = e => visitor.Visit(e.Operand);
            visitor.Visit(exp);
            return r;
        }

        public static MemberInfo Member(Expression exp) {
            MemberInfo prop = null;
            var visitor = new ExpressionVisitorV {
                VisitMember = e => { prop = e.Member; },
            };
            visitor.VisitUnary = e => visitor.Visit(e.Operand);
            visitor.Visit(exp);
            return prop;
        }

        /// <summary>
        /// Throws <see cref="InvalidCastException"/> if member is not a property
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">if member is not a property</exception>
        public static PropertyInfo Property(Expression exp) {
            return (PropertyInfo) Member(exp);
        }

        public static PropertyInfo Property<T, R>(this Expression<Func<T, R>> f) {
            return Property(f.Body);
        }

        public static string MemberName<T, R>(this Expression<Func<T, R>> f) {
            return MemberName(f.Body);
        }

        public static string MemberName<T>(this Expression<Func<T>> f) {
            return MemberName(f.Body);
        }

        public static string MemberName<T1, T2, T3, T4>(this Expression<Func<T1, T2, T3, T4>> f) {
            return MemberName(f.Body);
        }
    }
}