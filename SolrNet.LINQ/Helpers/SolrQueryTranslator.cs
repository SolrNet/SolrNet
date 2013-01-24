﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using SolrNet;
using SolrNet.Impl;


namespace SolrNet.LINQ.Helpers
{
    internal class SolrQueryTranslator : ExpressionVisitor
    {

        StringBuilder sb;
        private Type _resultType;
        private bool _inRangeQuery = false;
        public List<SortOrder> SortItems { get; set; }
        private readonly IDictionary<string,string> solrFields;
        private readonly IReadOnlyMappingManager mapper;


        internal SolrQueryTranslator(Type resultType, IReadOnlyMappingManager mapper)
        {
            _resultType = resultType;
            this.mapper = mapper;
            solrFields = mapper.GetFields(resultType).Values.ToDictionary(x => x.Property.Name, x => x.FieldName);
            SortItems = new List<SortOrder>();
        }



        internal string Translate(Expression expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);
            return this.sb.ToString();
        }



        private static Expression StripQuotes(Expression e)
        {

            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }



        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            #region Queryable methods
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                this.Visit(lambda.Body);

                return m;

            }


            if (m.Method.DeclaringType == typeof(Queryable) && (m.Method.Name == "OrderBy" || m.Method.Name == "ThenBy"))
            {

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                Visit(m.Arguments[0]);

                SolrQueryTranslator solrQueryTranslator = new SolrQueryTranslator(_resultType, mapper);
                var fieldName = solrQueryTranslator.Translate(lambda.Body);

                SortItems.Add(new SortOrder(fieldName, Order.ASC));


                return m;

            }

            if (m.Method.DeclaringType == typeof(Queryable) && (m.Method.Name == "OrderByDescending" || m.Method.Name == "ThenByDescending"))
            {

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                Visit(m.Arguments[0]);

                SolrQueryTranslator solrQueryTranslator = new SolrQueryTranslator(_resultType, mapper);
                var fieldName = solrQueryTranslator.Translate(lambda.Body);

                SortItems.Add(new SortOrder(fieldName, Order.DESC));


                return m;

            }

            #endregion

            #region SolrNetLinq specific
            if (m.Method.DeclaringType == typeof(SolrNetLinqExt) && m.Method.Name == "DefaultFieldEquals")
            {
                this.Visit(m.Arguments[1]);
                return m;
            }


            if (m.Method.DeclaringType == typeof(SolrNetLinqExt) && m.Method.Name == "Boost")
            {
                this.Visit(m.Arguments[0]);
                sb.Append("^");
                this.Visit(m.Arguments[1]);

                return m;

            }


            if (m.Method.DeclaringType == typeof(SolrNetLinqExt) && m.Method.Name == "DynamicField")
            {
                this.Visit(m.Arguments[1]);
                return m;
            }



            if (m.Method.DeclaringType == typeof(SolrNetLinqExt) && m.Method.Name == "AnyItem")
            {
                this.Visit(m.Arguments[0]);
                return m;
            }

            #endregion

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));

        }



        protected override Expression VisitUnary(UnaryExpression u)
        {

            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append("-");
                    this.Visit(u.Operand);
                    break;

                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    break;
                default:

                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));

            }

            return u;

        }


        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");

            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.Equal:

                    sb.Append(":");

                    break;

                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(":[");
                    _inRangeQuery = true;
                    break;

                case ExpressionType.LessThanOrEqual:
                    sb.Append(":[*");
                    _inRangeQuery = true;
                    break;

                default:

                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

            }

            this.Visit(b.Right);

            sb.Append(")");

            return b;

        }



        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q != null)
            {
                sb.Append(q.ElementType.Name);

            }
            else
            {
                //handle in range query
                if (_inRangeQuery)
                {
                    if (sb[sb.Length - 1] == '*')
                    {
                        sb.Append(" TO  ");
                        AppendConstValue(c.Value);
                    }
                    else
                    {
                        AppendConstValue(c.Value);
                        sb.Append(" TO *");
                    }
                    sb.Append("]");
                    _inRangeQuery = false;
                }
                
                else
                {
                    AppendConstValue(c.Value);
                }

            }

            return c;

        }

        private void AppendConstValue(object val)
        {
            //Set date format of Solr 1995-12-31T23:59:59.999Z
            if (val.GetType() == typeof(DateTime))
            {
                sb.Append(((DateTime)val).ToString("yyyy-MM-ddThh:mm:ss.fffZ"));
            }
            else
            {
                sb.Append(val.ToString());
            }
        }


        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter) {
                string fieldName = solrFields[m.Member.Name];
                sb.Append(fieldName);

                return m;

            }
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Constant)
            {
                ConstantExpression ce = (ConstantExpression)m.Expression;
                sb.Append(ce.Value.ToString());

                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

    }
}
